using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugins;

namespace KarmaPlugin {
    public class ReportPlugin : IPluginContract {
        public string CommandName => "report";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.DEFAULT;
        public string MessagePrefix { get; set; }
        public string HelpFile => "Reports a user. Call him by his name and he gets a report.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }
        public void OnPrivMsg(CommandMessage message) {
            if (message.parameters.Length == 0) {
                return;
            }
            string username = message.parameters.Split()[0];
            string user = username.ToLowerInvariant();

            KarmaManager.Instance.AddReport(user);
            long reportScore = KarmaManager.Instance.GetReports(user);
            long karmaScore = KarmaManager.Instance.GetKarma(user);
            string multiple = karmaScore == 1 ? "report" : "reports";
            Sender.Send(RawPrivMsg.Generate(message.origin, $"Thank you for helping to improve the Dota 2 community. " +
                                                            $"{username} ({karmaScore} Karma) is currently at {reportScore} {multiple}"));
        }
        public void OnNotice(CommandMessage message) { }
    }
}
