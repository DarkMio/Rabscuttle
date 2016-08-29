using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugin;

namespace StarCitizenPlugin {
    public class StarCitizenPlugin : IPluginContract {
        public string CommandName => "starcitizen";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.DEFAULT;
        public string MessagePrefix { get; set; }
        public string HelpFile => "Receives the latest statistics about the StarCitizen Crowdfunding.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }
        public void OnPrivMsg(CommandMessage message) {
            string url = "https://robertsspaceindustries.com/api/stats/getCrowdfundStats?fans=true&funds=true&fleet=true";
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.Credentials = CredentialCache.DefaultCredentials;
            request.UserAgent = "Rabscuttle, an IRC Bot.";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string responseContent = new StreamReader(response.GetResponseStream()).ReadToEnd();
            JObject jObject = JsonConvert.DeserializeObject(responseContent) as JObject;
            string fans = GetFormattedAmount(jObject["data"]["fans"].Value<long>());
            string fleet = GetFormattedAmount(jObject["data"]["fleet"].Value<long>());
            string funds = GetFormattedAmount(jObject["data"]["funds"].Value<long>() / 100.0);
            double averagePerShip = (jObject["data"]["funds"].Value<long>() * 0.0093) / jObject["data"]["fleet"].Value<long>();
            double gpm = CalculateGPM(jObject["data"]["funds"].Value<long>() / 100.0);
            Sender.Send(
                RawPrivMsg.Generate(
                    message.origin,
                    $"Total Crowdfunding: ${funds} | Accounts: {fans} | Ships bought: {fleet} | Avg per Ship: {averagePerShip:F} | GPM: ${gpm:F}"
                )
            );
        }
        public void OnNotice(CommandMessage message) { }

        private string GetFormattedAmount(double number) {
            int count = 0;
            string[] formatter = {"", "K", "M", "B", "T", "E"};
            while (number > 1000) {
                number /= 1000.0;
                count++;
            }
            return $"{number:F}{formatter[count]}";
        }

        private double CalculateGPM(double funds) {
            DateTime beginning = new DateTime(2012, 10, 10, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan span = DateTime.UtcNow - beginning;
            return funds / span.TotalSeconds * 60;
        }
    }
}
