using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugin;

namespace SteamPlugins {
    public class SteamstatusPlugin : IPluginContract {
        public string CommandName => "steamstatus";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.DEFAULT;
        public string MessagePrefix { get; set; }
        public string HelpFile => "Gets the current status of Steams' servers - data from steamstat.us.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }

        private static readonly Dictionary<string, string> REPLACEMENTS =  new Dictionary<string, string> {
            {"cms", "Steam CMs"}, { "steam","Steam"},
            {"store","Store"}, { "community","Community"}, { "webapi","Web API"},
            {"online","Online"}, { "dota2","Dota2 CG"}, { "tf2","TF2 CG"}, { "csgo","CSGO CG"},
            {"dota_mm_average","Average Wait Time"}, { "dota_mm_regions","Online Regions"},
            {"dota_mm_searching","Players Searching"}
        };

        private static readonly string[] RELEVANT_PODS = {
            "webapi", "tf2", "store", "online", "dota2", "csgo", "community", "cms"
        };

        public void OnPrivMsg(CommandMessage message) {
            var url = "https://crowbar.steamdb.info/Barney";
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.UserAgent = "Rabscuttle, an IRC Bot.";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            string responseContent = new StreamReader(response.GetResponseStream()).ReadToEnd();
            JObject jObject = JsonConvert.DeserializeObject(responseContent) as JObject;

            string output = "";
            JProperty thing;
            foreach(JProperty prop in jObject["services"].Value<JObject>().Properties()) {
                if (!RELEVANT_PODS.Contains(prop.Name)) {
                    continue;
                }
                thing = prop;
                output += FormatProperty(prop) + " | ";
            }
            output = output.Substring(0, output.Length - 2);
            Sender.Send(RawPrivMsg.Generate(message.origin, output));
        }

        private static string FormatProperty(JProperty prop) {
            JToken token = prop.Children().First<JToken>();
            string status = token["status"].Value<string>();
            string title = token["title"].Value<string>();
            string name = REPLACEMENTS.Aggregate(
                prop.Name, (current, replacement) => current.Replace(replacement.Key, replacement.Value)
            );
            string color = "";
            string c = "\x03";
            switch (status) {
                case "good":
                    color += 03;
                    break;
                case "minor":
                    color += 08;
                    break;
                case "major":
                    color += 07;
                    break;
            }

            return $"{name}:{c}{color} {title}{c}";
        }

        public void OnNotice(CommandMessage message) { }
    }
}
