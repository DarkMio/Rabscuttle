using System.ComponentModel.Composition;
using System.Linq;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugins;

namespace CoreFunctions {
    [Export(typeof(IPluginContract))]
    public class CommandsPlugin : IPluginContract {
        /// <summary> Gets or sets the name of the command. </summary>
        /// <value> The name of the command should be short and descriptive, usually a single word. </value>
        public string CommandName => "commands";

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
        public string HelpFile => "Displays all available commands for you.";

        /// <summary> Gets or sets the sender, which will be given by the bot. </summary>
        /// <value> A sender, which the plugin can send NetworkMessage to. </value>
        public ISender Sender { get; set; }

        /// <summary> Let the plugin subscribe to any of these Handlers - totally optional. </summary>
        /// <param name="handler">Any of the handlers, which will be given by the bot.</param>
        public void SubscribeTo(ObservableHandler handler) {

        }

        /// <summary> Called when a private message was received by the bot. Careful: This can be in a channel too! </summary>
        /// <param name="message">The network message received.</param>
        public void OnPrivMsg(CommandMessage message) {
            var plugins = BackReference.plugins.OrderBy(s => s.Rank);
            string commands = "";
            foreach (IPluginContract pluginContract in plugins) {
                if (message.permission >= pluginContract.Rank) {
                    commands += pluginContract.CommandName + ", ";
                } else {
                    break;
                }
            }
            commands = commands.Substring(0, commands.Length - 2);
            Sender.Send(RawNotice.Generate(message.user.userName,
                $"Following commands are available for you: {commands}"));
        }

        /// <summary> Called when a notice message was received by the bot. </summary>
        /// <param name="message">The network message received.</param>
        public void OnNotice(CommandMessage message) {

        }
    }
}
