using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TheInternationalTrackerPlugin {
    internal class PrizepoolCalculator {
        private const string Path = "../Plugins/PluginData/";
        private const string Url = "https://api.steampowered.com/IEconDOTA2_570/GetTournamentPrizePool/v1/?";
        private readonly string _steamAPI;
        public PrizepoolCalculator() {
            using (StreamReader s = new StreamReader(Path + "steamapi.key")) {
                _steamAPI = s.ReadLine();
            }
        }

        /// <summary> Gets the raw prizepool number. </summary>
        /// <param name="leagueID">The league identifier number.</param>
        /// <exception cref="WebException">The API doesn't seem to work - it responded with an error code: {status}</exception>
        public long GetRawPrizepool(int leagueID) {
            string url = Url + $"key={_steamAPI}&format=json&leagueid={leagueID}";
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.UserAgent = "Rabscootle, an IRC Bot.";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string responseContent = new StreamReader(response.GetResponseStream()).ReadToEnd();
            JObject jObject = JsonConvert.DeserializeObject(responseContent) as JObject;
            long status = jObject["result"]["status"].Value<long>();
            if (status == 200) {
                return jObject["result"]["prize_pool"].Value<long>();
            }
            throw new WebException($"The API doesn't seem to work - it responded with an error code: {status}");
        }

        /// <summary> Gets the formatted prizepool count, like <c>$2.6M</c> </summary>
        /// <param name="leagueID">The league identifier number.</param>
        public string GetFormattedPrizepool(int leagueID) {
            double prizepool = GetRawPrizepool(leagueID);
            int count = 0;
            string[] formatter = {"", "K", "M", "B", "T", "E"};
            while (prizepool > 1000) {
                prizepool /= 1000.0;
                count++;
            }
            return $"${prizepool}{formatter[count]}";
        }

        /// <summary> Calculates the time span until the time point is reached. </summary>
        /// <param name="endTime">The final time point in UTC format.</param>
        public TimeSpan TimeUntil(DateTime endTime) {
            return endTime - DateTime.UtcNow;
        }
    }
}
