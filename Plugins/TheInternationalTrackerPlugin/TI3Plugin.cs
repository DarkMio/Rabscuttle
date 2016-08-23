using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugins;

namespace TheInternationalTrackerPlugin {
    public class TI3Plugin : IPluginContract {
        public string CommandName => "ti3";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.DEFAULT;
        public string MessagePrefix { get; set; }
        public string HelpFile => $"Get's you the prizepool information for {CommandName.ToUpper()}.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }
        public void OnPrivMsg(CommandMessage message) {
            Sender.Send(RawPrivMsg.Generate(message.origin, "Ti3 Prizepool: $2,874,381 | achieved in 97 days | Mainevent: August 7th - 11th 2013 | 27.4 GPM"));
        }
        public void OnNotice(CommandMessage message) { }
    }
}
