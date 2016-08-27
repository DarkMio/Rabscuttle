using System;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugin;

namespace ExamplePlugin {
    public class ExamplePlugin : IPluginContract, IObserver<NetworkMessage> {
        private IDisposable _disposable;

        /// <summary> Gets or sets the name of the command. </summary>
        /// <value> The name of the command should be short and descriptive, usually a single word. </value>
        public string CommandName => "ExamplePlugin";

        /// <summary> Gets or sets the back reference. </summary>
        /// <value> The back reference is a the plugin handler reference, to search for other plugins, for example. </value>
        public PluginHandler BackReference { get; set; }

        /// <summary> Gets the command layer. </summary>
        /// <value> The command layer represents the order of which are plugins invoked. Number has not be unique, but reserve the first 25 layer for system operations. </value>
        public uint CommandLayer => 50;

        /// <summary> Gets or sets the rank. </summary>
        /// <value> The rank is the least amount of channel rights he has to have, otherwise the command execute will be ignored. </value>
        public MemberCode Rank => MemberCode.VOICED;

        /// <summary> Gets or sets the message prefix, which will be given by the bot. </summary>
        /// <value> The prefix is the kind of message prefix the bot usually listens to. </value>
        public string MessagePrefix { get; set; }

        /// <summary> Get or sets the help file, which the bot will use on request. </summary>
        /// <value> A helpfile, describing the plugins function, which might be sent by the bot on request. </value>
        public string HelpFile => CommandName + " has no help file.";

        /// <summary> Gets or sets the sender, which will be given by the bot. </summary>
        /// <value> A sender, which the plugin can send NetworkMessage to. </value>
        public ISender Sender { get; set; }

        /// <summary> Called when a notice message was received by the bot. </summary>
        /// <param name="message">The network message received.</param>
        public void OnNotice(CommandMessage message) {
            Console.WriteLine($"Hello, I'm {CommandName}.");
        }

        /// <summary> Called when a private message was received by the bot. Careful: This can be in a channel too! </summary>
        /// <param name="message">The network message received.</param>
        public void OnPrivMsg(CommandMessage message) {
            Sender.Send(RawPrivMsg.Generate(message.origin, $"Hello {message.user.userName}, this is the example plugin."));
        }

        /// <summary> Let the plugin subscribe to any of these Handlers - totally optional. </summary>
        /// <param name="handler">Any of the handlers, which will be given by the bot.</param>
        public void SubscribeTo(ObservableHandler handler) {
            _disposable = handler.Subscribe(this);
        }

        /// <summary>Provides the observer with new data.</summary>
        /// <param name="value">The current notification information.</param>
        public void OnNext(NetworkMessage value) {
            // throw new NotImplementedException();
            if ((CommandCode) value.typeEnum == CommandCode.PRIVMSG) {
                if (value.message.StartsWith("this is madness")) {
                    Sender.Send(RawPrivMsg.Generate(value.typeParams, "You're right!"));
                }
            }
        }



        /// <summary>Notifies the observer that the provider has experienced an error condition.</summary>
        /// <param name="error">An object that provides additional information about the error.</param>
        public void OnError(Exception error) {
            // throw new NotImplementedException();
        }

        /// <summary>Notifies the observer that the provider has finished sending push-based notifications.</summary>
        public void OnCompleted() {
            //  throw new NotImplementedException();
        }
    }
}
