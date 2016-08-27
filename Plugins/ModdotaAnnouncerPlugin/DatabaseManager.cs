using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ModdotaAnnouncerPlugin {
    public class DatabaseManager {
        private const string HOST = "remote.moddota.com";
        private const string USER = "forum_ro_bot";
        private const string PASSWORD = "a hash here.";
        private const string DB = "vanilla";
        private const int PORT = 3306;

        private readonly MySqlConnection _connection;

        public DatabaseManager() {
            _connection = new MySqlConnection($"Server={HOST};Port={PORT};Database={DB};Uid={USER};Pwd={PASSWORD};CHARSET=utf8");
            _connection.Open();
        }

        List<ThreadEntry> GetThreads(long lastID) {
             List<ThreadEntry> entries = new List<ThreadEntry>();
            string query = $"SELECT GDN_Discussion.Name, GDN_User.Name, GDN_Category.Name FROM GDN_Discussion " +
                           $"WHERE DiscussionID > {lastID} " +
                           $"JOIN (GDN_User, GDN_Category) " +
                           $"ON (GDN_User.UserID = GDN_Discussion.InsertUserID AND GDN_Category.CategoryID = GDN_Discussion.CategoryID)";
            using (MySqlCommand command = new MySqlCommand(query)) {
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read()) {
                    entries.Add(new ThreadEntry {
                        discussionId = reader.GetInt64(0),
                        categoryId = reader.GetInt64(1),
                        insertUserId = reader.GetInt64(2),
                        threadName = reader.GetString(3)
                    });
                }
                reader.Close();
            }
            return entries;
        }

        public struct ThreadEntry {
            public long discussionId;
            public long categoryId;
            public long insertUserId;
            public string threadName;
        }

    }
}
