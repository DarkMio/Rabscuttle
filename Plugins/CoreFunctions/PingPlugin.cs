using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugin;

namespace CoreFunctions {
    public class PingPlugin : IPluginContract {
        public string CommandName => "ping";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.DEFAULT;
        public string MessagePrefix { get; set; }
        public string HelpFile => "You send a ping, you get a pong.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }
        public void OnPrivMsg(CommandMessage message) {
            Sender.Send(RawPrivMsg.Generate(message.origin, "Pong."));
        }
        public void OnNotice(CommandMessage message) {
            Sender.Send(RawNotice.Generate(message.origin, "Pong."));
        }
    }
}
