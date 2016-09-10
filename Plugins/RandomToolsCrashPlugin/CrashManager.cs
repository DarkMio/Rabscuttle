using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Rabscuttle.util;

namespace RandomToolsCrashPlugin {
    public class CrashManager {

        private static readonly object SYNC_LOCK = new Object();
        private static CrashManager _instance;
        public  JObject _data;
        public struct Stats {
            public long week;
            public long total;
        }

        public static CrashManager Instance {
            get {
                if (_instance == null) {
                    lock (SYNC_LOCK) {
                        if (_instance == null) {
                            _instance = new CrashManager();
                        }
                    }
                }
                return _instance;
            }
        }

        public CrashManager() {
            _data = JsonUtility.ReadAndParseFile<JObject>(JsonUtility.PluginPath + "rtc/", "rtc.json");
        }

        public void Add() {
            long timestamp = GetWeekTimestamp();
            JToken token;
            if (_data.TryGetValue(timestamp + "", out token)) {
                _data[timestamp + ""] = token.Value<long>() + 1;
                return;
            }
            _data[timestamp + ""] = 1;
            JsonUtility.SerializeAndWriteFile<JObject>(_data, JsonUtility.PluginPath + "rtc/", "rtc.json");
        }

        private void Cleanup() {
            IEnumerable<JProperty> things = _data.Properties().Where(c => c.Name != GetWeekTimestamp() + "" && c.Name != "total");
            JToken data;
            if (_data.TryGetValue("total", out data)) {
                _data["total"] = data.Value<long>() +  things.Sum(c => c.Value.Value<long>());
                return;
            }
            _data["total"] = things.Sum(c => c.Value.Value<long>());
        }

        public Stats GetStats() {
            Cleanup();
            return new Stats {week = _data[GetWeekTimestamp() + ""].Value<long>(), total = _data["total"].Value<long>()};
        }

        private long GetWeekTimestamp() {
            TimeSpan span = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long) span.TotalSeconds / (7 * 24 * 60 * 60) * (7 * 24 * 60 * 60);
        }
    }
}
