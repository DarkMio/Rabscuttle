using System;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugin;

namespace CoreFunctions {
    public class PartPlugin : IPluginContract {
        public string CommandName => "part";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.BOTOPERATOR;
        public string MessagePrefix { get; set; }
        public string HelpFile => $"{MessagePrefix}part [<channel>] : If no channel given, it parts the current channel - otherwise it leaves the given one.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }
        public void OnPrivMsg(CommandMessage message) {
            if (String.IsNullOrWhiteSpace(message.parameters)) {
                Sender.Send(RawPart.Generate(message.origin, ""));
            }
            string[] parameters = message.parameters.Split();
            if (parameters[0].StartsWith("#")) {
                Sender.Send(RawPart.Generate(parameters[0], ""));
                return;
            }
            Sender.Send(RawNotice.Generate(message.user.userName, "Message seems malformed - channel names begin with a #."));
        }
        public void OnNotice(CommandMessage message) { }
    }
}
