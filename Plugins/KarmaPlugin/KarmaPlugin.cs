using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugins;
using Rabscuttle.stuff;

namespace KarmaPlugin {
    public class KarmaPlugin : IPluginContract {
        public string CommandName => "karma";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.DEFAULT;
        public string MessagePrefix { get; set; }
        public string HelpFile => "The Karma Plugin provides an overview over reports and commends of a certain user.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }
        public void OnPrivMsg(CommandMessage message) {
            if (message.parameters.Length == 0) {
                return;
            }
            string user = message.parameters.Split()[0].ToLowerInvariant();

            long commendScore = KarmaManager.Instance.GetCommends(user);
            long reportScore = KarmaManager.Instance.GetReports(user);
            long diff = commendScore - reportScore;

            Sender.Send(RawPrivMsg.Generate(message.origin, $"{user} is currently at {diff} Karma. (+{commendScore} | -{reportScore})"));
        }
        public void OnNotice(CommandMessage message) { }
    }
}
