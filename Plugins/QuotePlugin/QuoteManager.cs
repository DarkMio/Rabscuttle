using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabscuttle.channel;
using Rabscuttle.util;

namespace QuotePlugin {
    /// <summary>
    /// The Quote Manager offers all services to get, insert, delete a quote and writing it properly out.
    /// </summary>
    internal class QuoteManager {
        private static readonly object SYNC_LOCK = new Object();
        private static QuoteManager _instance;
        private static readonly string PATH = "../Plugins/PluginData/quotes/";
        private static readonly Regex USER_REGEX = new Regex(@"[\w\d]*", RegexOptions.None);

        private readonly JArray _quotes;
        private long _lastEntryIndex;
        private readonly Random _random;

        /// <summary> Standard singleton instance getter. </summary>
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

        /// <summary> Fetches a random quote from the storage. </summary>
        /// <returns> Either a JArray with <c>[index, quote, submitter, datetime]</c> or null </returns>
        public JArray GetRandomQuote() {
            return _quotes[_random.Next(_quotes.Count)] as JArray;
        }

        /// <summary> Fetches a single quote with the given index id. </summary>
        /// <returns> Either a JArray with <c>[index, quote, submitter, datetime]</c> or null </returns>
        public JArray GetQuote(long index) {
            // compare the ID for each - if you found one, safe cast it and return it.
            return _quotes.SingleOrDefault(c => c[0].Value<long>() == index) as JArray;
        }

        /// <summary> Inserts a quote with the given text, adds the appropiate index and timestamp, too. </summary>
        /// <returns> The new index id of the inserted quote. </returns>
        public long AddQuote(string quote, ChannelUser user) {
            JValue index = new JValue(++_lastEntryIndex);
            JValue quoteValue = new JValue(quote);
            JValue userValue = new JValue(USER_REGEX.Match(user.userName).Value);
            JValue timestamp = new JValue(GetTimestamp());
            // build a new structure, add it, write it, return the index.
            JArray array = new JArray(index, quoteValue, userValue, timestamp);
            _quotes.Add(array);
            WriteOut();
            return _lastEntryIndex;
        }

        /// <summary> Deletes the quote with the given index id. </summary>
        /// <returns> A boolean indicating if it found and deleted a quote or not. </returns>
        public bool RemoveQuote(long index) {
            JArray element = _quotes.SingleOrDefault(c => c[0].Value<long>() == index) as JArray;
            if (element == null) {
                return false;
            }

            _quotes.Remove(element);
            WriteOut();
            return true;
        }

        /// <summary> Reorders the id indices to remove possible fragmentation and empty ids. </summary>
        public void DefragmentQuotes() {
            // run down all quotes
            for (int index = 0; index < _quotes.Count; index++) {
                JToken jToken = _quotes[index];
                JArray element = jToken as JArray;
                if (element == null) {
                    Logger.WriteError("Quote Manager", "While defragmentation there was a null element" +
                                                       $" at position [ {index} ] - that shouldn't have happened.");
                    continue;
                }
                // the new index can just be the loop index
                // no verification of previous order, the iterator runs already like that
                element[0] = new JValue(index);
            }
            WriteOut();
        }

        /// <summary> Gets a Unix Timestamp from the current moment. </summary>
        private static double GetTimestamp() {
            DateTime unixBegin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.UtcNow);
            DateTime now = DateTime.UtcNow;
            return (unixBegin - now).TotalSeconds;
        }

        /// <summary> Reads and parses a JSON file into the appropiate type. </summary>
        /// @TODO: Move this to a utility method inside rabscuttle, please.
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

        /// <summary> Writes the current state of the Quote Storage to the json file </summary>
        //  @TODO: Might be wise to throw it into the destructor and save overhead?
        //         But what would happen with sudden crashes?
        public void WriteOut() {
            SerializeAndWriteFile(_quotes, "quotes.json");
        }

        /// <summary> Write out the JArray. </summary>
        /// @TODO: Same here, move it to utility functions in Rabs
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
