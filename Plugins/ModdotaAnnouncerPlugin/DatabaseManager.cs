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
        private const string HOST = "remote.moddota.com";
        private const string USER = "forum_ro_bot";
        private const string DB = "vanilla";
        private const int PORT = 3306;
        private const string PATH = "../Plugins/PluginData/moddota/";

        private readonly string _password;
        private readonly MySqlConnection _connection;
        private readonly JObject _metaStorage;
        private readonly ISender _sender;
        public string channel;

        public DatabaseManager(ISender sender) {
            _sender = sender;
            _metaStorage = ReadAndParseFile("database.json");
            _password = _metaStorage["dbKey"].Value<String>();
            _connection = new MySqlConnection(
                $"Server={HOST};Port={PORT};" +
                $"Database={DB};Uid={USER};Pwd={_password};CHARSET=utf8"
            );
            _connection.Open();
        }

        /// <summary> Thread runner, which queries every 30 seconds the DB. </summary>
        public void Run(string channel) {
            this.channel = channel;
            // validate the channel name first, so it doesn't feed data into the void
            if (!channel.StartsWith("#")) {
                throw new ArgumentException("This does not appear to be a valid ");
            }
            // check the last seen entry first
            long lastID = _metaStorage[channel].Value<long>();
            while (true) {
                // get entries and run through all of them
                List<ThreadEntry> entries = GetThreads(lastID);
                foreach (ThreadEntry entry in entries) {
                    string c = "\x03";
                    _sender.Send(
                        RawPrivMsg.Generate(
                            channel,
                            $"[{c}02{entry.categoryName}{c}] " +
                            $"{c}07{entry.userName}{c} posted: " + 
                            $"{c}03{entry.threadName}{c} | " + 
                            $"{c}06http://d2md.me/d/{entry.discussionId}{c}"
                        )
                    );
                    _metaStorage[channel] = entry.discussionId;
                    lastID = entry.discussionId;
                }
                SerializeAndWriteFile(_metaStorage, "database.json");
                Thread.Sleep(30000); // 30 seconds sleep
            }
        }

        /// <summary> Get the most recent threads after the last ID </summary>
        /// <returns> A list of threads which could be empty if there aren't any. </returns>
        List<ThreadEntry> GetThreads(long lastID) {
            List<ThreadEntry> entries = new List<ThreadEntry>();
            string query = "SELECT GDN_Discussion.DiscussionID, GDN_Discussion.Name, " + 
                           "       GDN_User.Name, GDN_Category.Name FROM GDN_Discussion " +
                           "JOIN (GDN_User, GDN_Category) " +
                           "ON (GDN_User.UserID = GDN_Discussion.InsertUserID AND " + 
                           "    GDN_Category.CategoryID = GDN_Discussion.CategoryID)" +
                          $"WHERE DiscussionID > {lastID}";
            // after building the query, send it
            using (MySqlCommand command = new MySqlCommand(query, _connection)) {
                // preapare reading
                MySqlDataReader reader = command.ExecuteReader();
                // read it and build an object.
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
                using (StreamReader s = new StreamReader(PATH + filename)) {
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
            using (StreamWriter s = new StreamWriter(PATH + filename)) {
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
