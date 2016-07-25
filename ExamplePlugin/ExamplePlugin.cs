﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginContract;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.handler;
using Rabscuttle.networking.io;

namespace ExamplePlugin {
    [Export(typeof(IPluginContract))]
    public class ExamplePlugin : IPluginContract {
        private string _commandName = "ExamplePlugin";

        /// <summary> Gets or sets the name of the command. </summary>
        /// <value> The name of the command should be short and descriptive, usually a single word. </value>
        public string CommandName {
            get { return _commandName; }
        }

        /// <summary> Gets or sets the rank. </summary>
        /// <value> The rank is the least amount of channel rights he has to have, otherwise the command execute will be ignored. </value>
        public MemberCode Rank { get { return MemberCode.VOICED; } }

        /// <summary> Gets or sets the message prefix, which will be given by the bot. </summary>
        /// <value> The prefix is the kind of message prefix the bot usually listens to. </value>
        public string MessagePrefix { get; set; }

        /// <summary> Get or sets the help file, which the bot will use on request. </summary>
        /// <value> A helpfile, describing the plugins function, which might be sent by the bot on request. </value>
        public string HelpFile { get { return _commandName + " has no help file."; } }

        /// <summary> Gets or sets the sender, which will be given by the bot. </summary>
        /// <value> A sender, which the plugin can send NetworkMessage to. </value>
        public ISender Sender { get; set; }

        /// <summary> Called when a notice message was received by the bot. </summary>
        /// <param name="message">The network message received.</param>
        public void OnNotice(NetworkMessage message) {
            Console.WriteLine($"Hello, I'm {_commandName}.");
        }

        /// <summary> Called when a private message was received by the bot. Careful: This can be in a channel too! </summary>
        /// <param name="message">The network message received.</param>
        public void OnPrivMsg(NetworkMessage message) {
            Sender.Send(RawPrivMsg.Generate(message.typeParams, $"Hello {message.prefix}, this is the example plugin."));
        }

        /// <summary> Let the plugin subscribe to any of these Handlers - totally optional. </summary>
        /// <param name="handler">Any of the handlers, which will be given by the bot.</param>
        public void SubscribeTo(ObservableHandler handler) { }
    }
}
