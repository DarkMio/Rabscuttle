using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabscuttle.channel;
using Rabscuttle.stuff;

namespace QuotePlugin {
    public class QuoteManager {
        private static readonly object SYNC_LOCK = new Object();
        private static QuoteManager _instance;
        private static readonly string PATH = "../Plugins/PluginData/quotes/";
        private static readonly Regex USER_REGEX = new Regex(@"[\w\d]*", RegexOptions.None);

        private readonly JArray _quotes;
        private long _lastEntryIndex;
        private readonly Random _random;

        public static QuoteManager Instance {
            get {
                if (_instance == null) {
                    lock (SYNC_LOCK) {
                        if (_instance == null) {
                            _instance = new QuoteManager();
                        }
                    }
                }
                return _instance;
            }
        }

        private QuoteManager() {
            _quotes = ReadAndParseFile("quotes.json");
            _lastEntryIndex = _quotes.Last[0].Value<long>();
            _random = new Random();
        }

        public JArray GetRandomQuote() {
            return _quotes[_random.Next(_quotes.Count)] as JArray;
        }

        public JArray GetQuote(long index) {
            return _quotes.SingleOrDefault(c => c[0].Value<long>() == index) as JArray;
        }

        public long AddQuote(string quote, ChannelUser user) {
            JValue index = new JValue(++_lastEntryIndex);
            JValue quoteValue = new JValue(quote);
            JValue userValue = new JValue(USER_REGEX.Match(user.userName).Value);
            JValue timestamp = new JValue(GetTimestamp());

            JArray array = new JArray(index, quoteValue, userValue, timestamp);
            _quotes.Add(array);
            WriteOut();
            return _lastEntryIndex;
        }

        public bool RemoveQuote(long index) {
            JArray element = _quotes.SingleOrDefault(c => c[0].Value<long>() == index) as JArray;
            if (element == null) {
                return false;
            }

            _quotes.Remove(element);
            WriteOut();
            return true;
        }

        public void DefragmentQuotes() {
            for (int index = 0; index < _quotes.Count; index++) {
                JToken jToken = _quotes[index];
                JArray element = jToken as JArray;
                if (element == null) {
                    Logger.WriteError("Quote Manager", "While defragmentation there was a null element" +
                                                       $" at position [ {index} ] - that shouldn't have happened.");
                    continue;
                }
                element[0] = new JValue(index);
            }
            WriteOut();
        }

        private static double GetTimestamp() {
            DateTime unixBegin = new DateTime(1970, 1, 1);
            DateTime now = DateTime.UtcNow;
            return (unixBegin - now).TotalSeconds;
        }

        private static JArray ReadAndParseFile(string filename) {
            try {
                using (StreamReader s = new StreamReader(PATH + filename)) {
                    string json = s.ReadToEnd();
                    return (JArray) JsonConvert.DeserializeObject(json);
                }
            } catch (IOException e) {
                Logger.WriteWarn("Quote Manager", $"Something went wrong while reading: {e}");
                return null;
            }
        }

        public void WriteOut() {
            SerializeAndWriteFile(_quotes, "quotes.json");
        }

        private static void SerializeAndWriteFile(JArray jArray, string filename) {
            if (jArray == null) {
                jArray = new JArray();
            }

            string output = JsonConvert.SerializeObject(jArray);
            using (StreamWriter s = new StreamWriter(PATH + filename)) {
                s.Write(output);
            }
        }
    }
}
