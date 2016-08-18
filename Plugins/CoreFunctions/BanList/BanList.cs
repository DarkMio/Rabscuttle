using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabscuttle.channel;
using Rabscuttle.stuff;

namespace CoreFunctions.BanList {
    public class BanList {
        private const string FILEPATH = "../Plugins/PluginData/bans/";
        private JArray _banlist;
        private readonly string _filename;
        private const string IDENT_STRING = "{0}!{1}@{2}";

        public BanList(string filename = "banlist.json") {
            _filename = filename;
            _banlist = ReadAndParseFile();
        }

        private JObject BuildIdentObject(string user, string ident, string host) {
            JProperty userProp = new JProperty("user", user);
            JProperty identProp = new JProperty("ident", ident);
            JProperty hostProp = new JProperty("host", host);
            return new JObject(userProp, identProp, hostProp);
        }

        public void BanUser(ChannelUser user) {
            BanUser(user.userName, user.ident, user.host);
        }

        public void BanUser(string user = "*", string ident = "*", string host = "*") {
            JObject userObject = BuildIdentObject(user, ident, host);

            // @TODO: This is a really sloppy variant - could need a proper comperator and some point
            if (_banlist.SingleOrDefault(c => c.ToString() == userObject.ToString()) != null) {
                return;
            }
            _banlist.Add(userObject);
            SerializeAndWriteFile();
        }

        public bool UnbanUser(ChannelUser user) {
            return UnbanUser(user.userName, user.ident, user.host);
        }

        public bool UnbanUser(string user = "*", string ident = "*", string host = "*") {
            JObject userObject = BuildIdentObject(user, ident, host);
            // _banlist.Remove(new JValue("*!*@*"));
            JToken token = _banlist.SingleOrDefault(c => c.ToString() == userObject.ToString());
            _banlist.Remove(token);
            SerializeAndWriteFile();
            return token != null;
        }

        public bool CheckBan(ChannelUser user) {
            return CheckBan(user.userName, user.ident, user.host);
        }

        public bool CheckBan(string user = "*", string ident = "*", string host = "*") {
            var banlist = _banlist.Values<JObject>();
            if (user != "*") {
                banlist = banlist.Where(c => c["user"].Value<string>() == user);
            }
            if (ident != "*") {
                banlist = banlist.Where(c => c["ident"].Value<string>() == ident);
            }
            if (host != "*") {
                banlist = banlist.Where(c => c["host"].Value<string>() == ident);
            }
            return banlist.Any();
        }

        public string[] GetBans() {
            var banlist = _banlist.Values<JObject>();
            List<string> strings = new List<string>();
            foreach (JObject jObject in banlist) {
                string user = jObject["user"].Value<string>();
                string ident = jObject["ident"].Value<string>();
                string host = jObject["host"].Value<string>();
                strings.Add(String.Format(IDENT_STRING, user, ident, host));
            }
            return strings.ToArray();
        }

        public void ReloadBanlist() {
            _banlist = ReadAndParseFile();
        }

        private JArray ReadAndParseFile() {
            try {
                using (StreamReader s = new StreamReader(FILEPATH + _filename)) {
                    string json = s.ReadToEnd();
                    JArray jA = JsonConvert.DeserializeObject(json) as JArray;
                    return jA ?? new JArray();
                }
            } catch (IOException e) {
                Logger.WriteWarn("Karma Plugin", $"Something went wrong while reading: {e}");
                return null;
            }
        }

        private void SerializeAndWriteFile() {
            JArray jObject = _banlist ?? new JArray();

            string output = JsonConvert.SerializeObject(_banlist);
            using (StreamWriter s = new StreamWriter(FILEPATH + _filename)) {
                s.Write(output);
            }
        }
    }
}
