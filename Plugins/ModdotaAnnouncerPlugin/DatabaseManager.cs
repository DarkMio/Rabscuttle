using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.util;

namespace ModdotaAnnouncerPlugin {
    internal class DatabaseManager {
        private const string Host = "remote.moddota.com";
        private const string User = "forum_ro_bot";
        private const string Db = "vanilla";
        private const int Port = 3306;
        private const string Path = "../Plugins/PluginData/moddota/";

        private readonly MySqlConnection _connection;
        private readonly JObject _metaStorage;
        private readonly ISender _sender;

        public DatabaseManager(ISender sender) {
            _sender = sender;
            _metaStorage = ReadAndParseFile("database.json");
            var password = _metaStorage["dbKey"].Value<string>();
            _connection = new MySqlConnection($"Server={Host};Port={Port};Database={Db};Uid={User};Pwd={password};CHARSET=utf8");
            _connection.Open();
        }

        public void Run(string channel) {
            if (!channel.StartsWith("#")) {
                throw new ArgumentException("This does not appear to be a valid ");
            }
            while (true) {
                long lastId = _metaStorage[channel].Value<long>();
                IEnumerable<ThreadEntry> entries = GetThreads(lastId);
                foreach (ThreadEntry entry in entries) {
                    string c = "\x03";
                    _sender.Send(
                        RawPrivMsg.Generate(
                            channel,
                            $"[{c}02{entry.categoryName}{c}] {c}07{entry.userName}{c} posted: {c}03{entry.threadName}{c} | {c}06http://d2md.me/d/{entry.discussionId}{c}"
                        )
                    );
                    _metaStorage[channel] = entry.discussionId;
                }
                SerializeAndWriteFile(_metaStorage, "database.json");
                Thread.Sleep(30000); // 30 seconds sleep
            }
        }

        private IEnumerable<ThreadEntry> GetThreads(long lastId) {
            List<ThreadEntry> entries = new List<ThreadEntry>();
            string query = "SELECT GDN_Discussion.DiscussionID, GDN_Discussion.Name, GDN_User.Name, GDN_Category.Name FROM GDN_Discussion " +
                           "JOIN (GDN_User, GDN_Category) " +
                           "ON (GDN_User.UserID = GDN_Discussion.InsertUserID AND GDN_Category.CategoryID = GDN_Discussion.CategoryID)" +
                          $"WHERE DiscussionID > {lastId}";
            using (MySqlCommand command = new MySqlCommand(query, _connection)) {
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read()) {
                    entries.Add(new ThreadEntry {
                        discussionId = reader.GetInt64(0),
                        threadName = reader.GetString(1),
                        userName = reader.GetString(2),
                        categoryName = reader.GetString(3)
                    });
                }
                reader.Close();
            }
            return entries;
        }

        private static JObject ReadAndParseFile(string filename) {
            try {
                using (StreamReader s = new StreamReader(Path + filename)) {
                    string json = s.ReadToEnd();
                    return (JObject) JsonConvert.DeserializeObject(json);
                }
            } catch (IOException e) {
                Logger.WriteWarn("Karma Plugin", $"Something went wrong while reading: {e}");
                return null;
            }
        }

        private static void SerializeAndWriteFile(JObject jObject, string filename) {
            if (jObject == null) {
                jObject = new JObject();
            }

            string output = JsonConvert.SerializeObject(jObject);
            using (StreamWriter s = new StreamWriter(Path + filename)) {
                s.Write(output);
            }
        }

        public struct ThreadEntry {
            public long discussionId;
            public string categoryName;
            public string userName;
            public string threadName;
        }
    }
}
