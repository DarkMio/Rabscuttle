using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugins;

namespace KarmaPlugin {
    public class CommendPlugin : IPluginContract {
        public string CommandName => "commend";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.DEFAULT;
        public string MessagePrefix { get; set; }
        public string HelpFile => "Commends a user. Call him by his name and he gets commended.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }
        public void OnPrivMsg(CommandMessage message) {
            if (message.parameters.Length == 0) {
                return;
            }
            string username = message.parameters.Split()[0];
            string user = username.ToLowerInvariant();

            KarmaManager.Instance.AddCommend(user);
            long commendScore = KarmaManager.Instance.GetCommends(user);
            long karmaScore = KarmaManager.Instance.GetKarma(user);
            string multiple = karmaScore == 1 ? "commend" : "commends";
            Sender.Send(RawPrivMsg.Generate(message.origin, $"Thank you for helping to improve the Dota 2 community. " +
                                                            $"{username} ({karmaScore} Karma) is currently at {commendScore} {multiple}"));
        }
        public void OnNotice(CommandMessage message) { }
    }
}
