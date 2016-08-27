using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugin;

namespace TheInternationalTrackerPlugin {
    public class TI5Plugin : IPluginContract {
        public string CommandName => "ti5";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.DEFAULT;
        public string MessagePrefix { get; set; }
        public string HelpFile => $"Get's you the prizepool information for {CommandName.ToUpper()}.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }
        public void OnPrivMsg(CommandMessage message) {
            Sender.Send(RawPrivMsg.Generate(message.origin, "Ti5 Prizepool: $18,429,613 | achieved in 95 days | Mainevent: August 3rd - 8th 2015 | 369.1 GPM"));
        }
        public void OnNotice(CommandMessage message) { }
    }
}

