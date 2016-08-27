using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugin;

namespace GithubUpdatedPlugin{
    public class GithubUpdatedPlugin : IPluginContract {
        /// <summary> Gets or sets the name of the command. </summary>
        /// <value> The name of the command should be short and descriptive, usually a single word. </value>
        public string CommandName => "updated";

        /// <summary> Gets or sets the back reference. </summary>
        /// <value> The back reference is a the plugin handler reference, to search for other plugins, for example. </value>
        public PluginHandler BackReference { get; set; }

        /// <summary> Gets or sets the rank. </summary>
        /// <value> The rank is the least amount of channel rights he has to have, otherwise the command execute will be ignored. </value>
        public MemberCode Rank => MemberCode.DEFAULT;

        /// <summary> Gets or sets the message prefix, which will be given by the bot. </summary>
        /// <value> The prefix is the kind of message prefix the bot usually listens to. </value>
        public string MessagePrefix { get; set; }

        /// <summary> Get or sets the help file, which the bot will use on request. </summary>
        /// <value> A helpfile, describing the plugins function, which might be sent by the bot on request. </value>
        public string HelpFile => "This gets you the most recent update of the most popular repo with that name";

        /// <summary> Gets or sets the sender, which will be given by the bot. </summary>
        /// <value> A sender, which the plugin can send NetworkMessage to. </value>
        public ISender Sender { get; set; }

        /// <summary> Let the plugin subscribe to any of these Handlers - totally optional. </summary>
        /// <param name="handler">Any of the handlers, which will be given by the bot.</param>
        public void SubscribeTo(ObservableHandler handler) {
            // throw new NotImplementedException();
        }

        private int rateLimitRemainder = 10;
        private DateTime rateLimitReset = DateTime.Now;
        private static readonly DateTime unixTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);

        /// <summary> Called when a private message was received by the bot. Careful: This can be in a channel too! </summary>
        /// <param name="message">The network message received.</param>
        public void OnPrivMsg(CommandMessage message) {


            double remaining = rateLimitReset.Subtract(DateTime.UtcNow).TotalSeconds;
            if (rateLimitRemainder <= 0 && remaining >= 0) {
                int seconds = (int) remaining;
                Sender.Send(RawPrivMsg.Generate(message.origin, $"Rate limit exceeded - please try again in {seconds} seconds."));
                return;
            }

            var repo = message.parameters.Split()[0];
            var uri = "https://api.github.com/search/repositories?q=" + repo;

            HttpWebRequest request = HttpWebRequest.Create(uri) as HttpWebRequest;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.UserAgent = "Rabscootle, an IRC Bot.";

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            string remainder = response.Headers?["X-RateLimit-Remaining"];
            string time = response.Headers?["X-RateLimit-Reset"];

            Int32.TryParse(remainder, out rateLimitRemainder);
            long timer;
            Int64.TryParse(time, out timer);
            rateLimitReset = unixTime.AddSeconds(timer);

            string responseContent = new StreamReader(response.GetResponseStream()).ReadToEnd();
            JObject jObject = JsonConvert.DeserializeObject(responseContent) as JObject;
            long totalCount = (long) (jObject["total_count"] as JValue).Value;
            if (totalCount <= 0) {
                Sender.Send(RawPrivMsg.Generate(message.origin, $"Could not find any repos named '{repo}'."));
                return;
            }

            string fullName = (jObject["items"][0]["full_name"] as JValue).Value as string;
            DateTime pushedAt = (DateTime) (jObject["items"][0]["pushed_at"] as JValue)?.Value;
            DateTime updateAt = (DateTime) (jObject["items"][0]["updated_at"] as JValue)?.Value;
            DateTime finalTime = pushedAt.CompareTo(updateAt) > 0 ? pushedAt : updateAt;
            TimeSpan offset = DateTime.UtcNow.Subtract(finalTime);
            string dateString = "";
            if (offset.Days > 0) {
                dateString += $" {offset.Days}d";
            }
            Sender.Send(RawPrivMsg.Generate(message.origin, $"[{fullName}] Last update was{dateString} {offset.Hours}h {offset.Minutes}m ago."));
        }

        /// <summary> Called when a notice message was received by the bot. </summary>
        /// <param name="message">The network message received.</param>
        public void OnNotice(CommandMessage message) {
            // throw new NotImplementedException();
        }
    }
}
