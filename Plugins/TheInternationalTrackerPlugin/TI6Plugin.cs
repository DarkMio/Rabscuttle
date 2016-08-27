using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugin;

namespace TheInternationalTrackerPlugin {
    public class TI6Plugin : IPluginContract {
        public string CommandName => "ti6";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.DEFAULT;
        public string MessagePrefix { get; set; }
        public string HelpFile => $"Get's you the prizepool information for {CommandName.ToUpper()}.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }
        public void OnPrivMsg(CommandMessage message) {
            Sender.Send(RawPrivMsg.Generate(message.origin, "Ti6 Prizepool: $20,770,460 | achieved in 89 days | Mainevent: August 8th - 13th 2016 | 486.2 GPM"));
        }
        public void OnNotice(CommandMessage message) { }
    }
}
