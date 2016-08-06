using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugins;
using Rabscuttle.stuff;

namespace KarmaPlugin {
    [Export(typeof(IPluginContract))]
    public class KarmaPlugin : IPluginContract {

        /// <summary> Gets or sets the name of the command. </summary>
        /// <value> The name of the command should be short and descriptive, usually a single word. </value>
        public string CommandName => "karma";

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
        public string HelpFile => "The Karma Plugin provides an overview over reports and commends of a certain user.";

        /// <summary> Gets or sets the sender, which will be given by the bot. </summary>
        /// <value> A sender, which the plugin can send NetworkMessage to. </value>
        public ISender Sender { get; set; }

        private static readonly Regex USER_REGEX = new Regex(@"[a-z|0-9]*", RegexOptions.None);
        private static readonly string PATH = "../Plugins/PluginData/karma/";


        private string RemoveUserChars(string name) {
            return USER_REGEX.Match(name).Value;
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


        /// <summary> Let the plugin subscribe to any of these Handlers - totally optional. </summary>
        /// <param name="handler">Any of the handlers, which will be given by the bot.</param>
        public void SubscribeTo(ObservableHandler handler) {
            // throw new NotImplementedException();
        }

        /// <summary> Called when a private message was received by the bot. Careful: This can be in a channel too! </summary>
        /// <param name="message">The network message received.</param>
        public void OnPrivMsg(CommandMessage message) {
            if (message.parameters.Length == 0) {
                return;
            }
            string user = message.parameters.Split()[0].ToLowerInvariant();
            string userName = RemoveUserChars(user);

            JToken commends = ReadAndParseFile("commend.json")[userName];
            JToken reports = ReadAndParseFile("report.json")[userName];

            long commendScore = 0;
            long reportScore = 0;
            if (commends != null) {
                commendScore = commends.Value<long>();
            }
            if (reports != null) {
                reportScore = reports.Value<long>();
            }
            long diff = commendScore - reportScore;

            Sender.Send(RawPrivMsg.Generate(message.origin, $"{user} is currently at {diff} Karma. (+{commends} | -{reports})"));
        }

        /// <summary> Called when a notice message was received by the bot. </summary>
        /// <param name="message">The network message received.</param>
        public void OnNotice(CommandMessage message) {
            // throw new NotImplementedException();
        }
    }
}
