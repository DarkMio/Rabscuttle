﻿using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugin;

namespace CoreFunctions {
    public class RawPlugin : IPluginContract {
        public string CommandName => "raw";
        public PluginHandler BackReference { get; set; }
        public MemberCode Rank => MemberCode.BOTOPERATOR;
        public string MessagePrefix { get; set; }
        public string HelpFile => "This sends raw commands to the server from the bot.";
        public ISender Sender { get; set; }
        public void SubscribeTo(ObservableHandler handler) { }
        public void OnPrivMsg(CommandMessage message) {
            Sender.Send(new NetworkMessage(message.parameters, false));
        }
        public void OnNotice(CommandMessage message) { }
    }
}
