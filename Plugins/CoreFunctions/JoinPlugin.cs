using System;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugins;

namespace CoreFunctions {
    public class JoinPlugin : IPluginContract {
        public string CommandName => "join";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.BOTOPERATOR;
        public string MessagePrefix { get; set; }
        public string HelpFile => $"{MessagePrefix}join <channel> : Lets the bot join a given channel.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }
        public void OnPrivMsg(CommandMessage message) {
            if (String.IsNullOrWhiteSpace(message.parameters)) {
                Sender.Send(RawNotice.Generate(message.user.userName, $"No channel given to join? {HelpFile}"));
            }
            string[] parameters = message.parameters.Split();
            if (parameters[0].StartsWith("#")) {
                Sender.Send(RawJoin.Generate(parameters[0]));
                return;
            }
            Sender.Send(RawNotice.Generate(message.user.userName, "Message seems malformed - channel names begin with a #."));
        }
        public void OnNotice(CommandMessage message) { }
    }
}
