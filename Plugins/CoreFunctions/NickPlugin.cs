using System;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugins;

namespace CoreFunctions {
    public class NickPlugin : IPluginContract {
        public string CommandName => "nick";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.BOTOPERATOR;
        public string MessagePrefix { get; set; }
        public string HelpFile => $"{MessagePrefix}nick <name> : Sets the given name of the bot.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }
        public void OnPrivMsg(CommandMessage message) {
            if (String.IsNullOrWhiteSpace(message.parameters)) {
                Sender.Send(RawNotice.Generate(message.user.userName, $"You have to give the bot a name! {HelpFile}"));
            }
            string[] parameters = message.parameters.Split();
            Sender.Send(RawNick.Generate(parameters[0]));
        }
        public void OnNotice(CommandMessage message) { }
    }
}
