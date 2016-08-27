using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugin;

namespace TheInternationalTrackerPlugin {
    public class TI4Plugin : IPluginContract {
        public string CommandName => "ti4";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.DEFAULT;
        public string MessagePrefix { get; set; }
        public string HelpFile => $"Get's you the prizepool information for {CommandName.ToUpper()}.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }
        public void OnPrivMsg(CommandMessage message) {
            Sender.Send(RawPrivMsg.Generate(message.origin, "Ti4 Prizepool: $10,931,105 | achieved in 73 days | Mainevent: July 18th - 21st 2014 | 266.3 GPM"));
        }
        public void OnNotice(CommandMessage message) { }
    }
}

