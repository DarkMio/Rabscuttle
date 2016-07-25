using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.handler;
using Rabscuttle.networking.io;

namespace PluginContract {
    public interface IPluginContract {
        /// <summary> Gets or sets the name of the command. </summary>
        /// <value> The name of the command should be short and descriptive, usually a single word. </value>
        string CommandName { get; }
        /// <summary> Gets or sets the rank. </summary>
        /// <value> The rank is the least amount of channel rights he has to have, otherwise the command execute will be ignored. </value>
        MemberCode Rank { get; }
        /// <summary> Gets or sets the message prefix, which will be given by the bot. </summary>
        /// <value> The prefix is the kind of message prefix the bot usually listens to. </value>
        string MessagePrefix { get; set; }
        /// <summary> Get or sets the help file, which the bot will use on request. </summary>
        /// <value> A helpfile, describing the plugins function, which might be sent by the bot on request. </value>
        string HelpFile { get; }
        /// <summary> Gets or sets the sender, which will be given by the bot. </summary>
        /// <value> A sender, which the plugin can send NetworkMessage to. </value>
        ISender Sender { get; set; }
        /// <summary> Let the plugin subscribe to any of these Handlers - totally optional. </summary>
        /// <param name="handler">Any of the handlers, which will be given by the bot.</param>
        void SubscribeTo(ObservableHandler handler);
        /// <summary> Called when a private message was received by the bot. Careful: This can be in a channel too! </summary>
        /// <param name="message">The network message received.</param>
        void OnPrivMsg(NetworkMessage message);
        /// <summary> Called when a notice message was received by the bot. </summary>
        /// <param name="message">The network message received.</param>
        void OnNotice(NetworkMessage message);
    }
}
