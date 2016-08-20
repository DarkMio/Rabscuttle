using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabscuttle.channel;
using Rabscuttle.stuff;

namespace KarmaPlugin {
    public class KarmaManager {
        private static readonly Object SYNC_LOCK = new Object();
        private static KarmaManager _instance;
        private static readonly Regex USER_REGEX = new Regex(@"[a-z|0-9]*", RegexOptions.None);
        private static readonly string PATH = "../Plugins/PluginData/karma/";
        private readonly JObject _commends;
        private readonly JObject _reports;

        private KarmaManager() {
            _commends = ReadAndParseFile("commend.json");
            _reports = ReadAndParseFile("report.json");
        }

        private string RemoveUserChars(string name) {
            return USER_REGEX.Match(name).Value.ToLower();
        }

        public static KarmaManager Instance {
            get {
                if (_instance == null) {
                    lock (SYNC_LOCK) {
                        if (_instance == null) {
                            _instance = new KarmaManager();
                        }
                    }
                }
                return _instance;
            }
        }

        public void AddCommend(string username) {
            username = RemoveUserChars(username).ToLower();
            long commendScore = GetCommends(username) + 1;
            _commends[username] = new JValue(commendScore);
            SerializeAndWriteFile(_commends, "commend.json");
        }

        public void AddReport(string username) {
            username = RemoveUserChars(username).ToLower();
            long reportScore = GetReports(username) + 1;
            _reports[username] = new JValue(reportScore);
            SerializeAndWriteFile(_reports, "report.json");
        }

        public long GetCommends(string username) {
            username = RemoveUserChars(username).ToLower();
            JToken commend = _commends[username];
            return commend?.Value<long>() ?? 0;
        }

        public long GetReports(string username) {
            username = RemoveUserChars(username).ToLower();
            JToken reports = _reports[username];
            return reports?.Value<long>() ?? 0;
        }

        public long GetKarma(string username) {
            return GetCommends(username) - GetReports(username);
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
    }
}
