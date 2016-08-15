﻿using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugins;

namespace CoreFunctions {
    [Export(typeof(IPluginContract))]
    public class HelpPlugin : IPluginContract {
        /// <summary> Gets or sets the name of the command. </summary>
        /// <value> The name of the command should be short and descriptive, usually a single word. </value>
        public string CommandName => "help";

        /// <summary> Gets or sets the back reference. </summary>
        /// <value> The back reference is a the plugin handler reference, to search for other plugins, for example. </value>
        public PluginHandler BackReference { get; set; }

        /// <summary> Gets or sets the rank. </summary>
        /// <value> The rank is the least amount of channel rights he has to have, otherwise the command execute will be ignored. </value>
        public MemberCode Rank => MemberCode.DEFAULT;

        /// <summary> Gets or sets the message prefix, which will be given by the bot. </summary>
        /// <value> The prefix is the kind of message prefix the bot usually listens to. </value>
        public string MessagePrefix { get; set; }

        /// <summary> Get or sets the help file, which the bot will use on request. </summary>
        /// <value> A helpfile, describing the plugins function, which might be sent by the bot on request. </value>
        public string HelpFile =>
                "This is the help file for the 'help' command." +
                $" You can query any help sending: \n{MessagePrefix}help <command>";

        /// <summary> Gets or sets the sender, which will be given by the bot. </summary>
        /// <value> A sender, which the plugin can send NetworkMessage to. </value>
        public ISender Sender { get; set; }

        /// <summary> Let the plugin subscribe to any of these Handlers - totally optional. </summary>
        /// <param name="handler">Any of the handlers, which will be given by the bot.</param>
        public void SubscribeTo(ObservableHandler handler) { }

        /// <summary> Called when a private message was received by the bot. Careful: This can be in a channel too! </summary>
        /// <param name="message">The network message received.</param>
        public void OnPrivMsg(CommandMessage message) {
            FindAndSendHelp(message);
        }

        /// <summary> Called when a notice message was received by the bot. </summary>
        /// <param name="message">The network message received.</param>
        public void OnNotice(CommandMessage message) {
            FindAndSendHelp(message);
        }

        private void FindAndSendHelp(CommandMessage message) {
            if (String.IsNullOrEmpty(message.parameters)) {
                Sender.Send(RawNotice.Generate(message.user.userName, HelpFile));
                return;
            }

            string command = message.parameters.Split(new []{' '}, 2)[0].ToLower();

            foreach (IPluginContract pluginContract in BackReference.plugins) {
                // if we would want to hide command help that the user wouldn't see, we could do that:
                //     message.permission >= pluginContract.Rank
                // but that#s an open design decision
                if (command == pluginContract.CommandName.ToLower() ) {
                    Sender.Send(RawNotice.Generate(message.user.userName, pluginContract.HelpFile));
                    return;
                }
            }

            Sender.Send(RawNotice.Generate(message.user.userName, "I could not find the command you're searching for."));
        }
    }
}
