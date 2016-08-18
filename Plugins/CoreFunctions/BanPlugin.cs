using System;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using Rabscuttle.channel;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.plugins;
using Rabscuttle.util;

namespace CoreFunctions {
    [Export(typeof(IPluginContract))]
    public class BanPlugin : IPluginContract {
        /// <summary> Gets or sets the name of the command. </summary>
        /// <value> The name of the command should be short and descriptive, usually a single word. </value>
        public string CommandName => "ban";

        /// <summary> Gets or sets the back reference. </summary>
        /// <value> The back reference is a the plugin handler reference, to search for other plugins, for example. </value>
        public PluginHandler BackReference { get; set; }

        /// <summary> Gets or sets the rank. </summary>
        /// <value> The rank is the least amount of channel rights he has to have, otherwise the command execute will be ignored. </value>
        public MemberCode Rank => MemberCode.BOTOPERATOR;

        /// <summary> Gets or sets the message prefix, which will be given by the bot. </summary>
        /// <value> The prefix is the kind of message prefix the bot usually listens to. </value>
        public string MessagePrefix { get; set; }

        /// <summary> Get or sets the help file, which the bot will use on request. </summary>
        /// <value> A helpfile, describing the plugins function, which might be sent by the bot on request. </value>
        public string HelpFile => "Bans or unbans a user\n" +
                                  $"{MessagePrefix}ban <username>!<ident>@<hostname> - to ban a user.\n" +
                                  $"{MessagePrefix}ban unban <username>!<ident>@<hostname> - to unban a user.\n" +
                                  $"{MessagePrefix}ban list - to list all banned users.\n" +
                                  $"{MessagePrefix}ban reload - reloads the banlist";

        /// <summary> Gets or sets the sender, which will be given by the bot. </summary>
        /// <value> A sender, which the plugin can send NetworkMessage to. </value>
        public ISender Sender { get; set; }
        private BanList.BanList _banlist = new BanList.BanList();

        /// <summary> Let the plugin subscribe to any of these Handlers - totally optional. </summary>
        /// <param name="handler">Any of the handlers, which will be given by the bot.</param>
        public void SubscribeTo(ObservableHandler handler) { }

        /// <summary> Called when a private message was received by the bot. Careful: This can be in a channel too! </summary>
        /// <param name="message">The network message received.</param>
        public void OnPrivMsg(CommandMessage message) {
            if (String.IsNullOrEmpty(message.parameters)) {
                Sender.Send(RawNotice.Generate(message.user.userName, "No user string specified."));
            }

            string[] parameters = message.parameters.Split(' ');
            switch (parameters[0]) {
                case "unban":
                    Unban(parameters, message.user);
                    return;
                case "list":
                    ListBans(message);
                    return;
                case "reload":
                    _banlist.ReloadBanlist();
                    return;
            }
            BanUser(parameters, message.user);
        }

        private void ListBans(CommandMessage message) {
            string[] bans = _banlist.GetBans();
            string banString = String.Join(", ", bans);
            Sender.Send(RawNotice.Generate(message.user.userName, $"Following bans found: {banString}"));
        }

        private void BanUser(string[] parameters, ChannelUser user) {
            MatchCollection matches = Validator.UserOriginMatches(parameters[0]);
            if (matches.Count > 0) {
                var groups = matches[0].Groups;
                _banlist.BanUser(groups["user"].Value, groups["ident"].Value, groups["host"].Value);
                Sender.Send(RawNotice.Generate(user.userName, $"Banned successfully: {parameters[0]}"));
                return;
            }
            Sender.Send(RawNotice.Generate(user.userName, "The ban string seems malformed or missing, try: " +
                                                          $"{MessagePrefix}ban <user>!<ident>@<host>"));
        }

        private void Unban(string[] parameters, ChannelUser user) {
            if (parameters.Length > 1) {
                MatchCollection matches = Validator.UserOriginMatches(parameters[1]);
                if (matches.Count > 0) {
                    GroupCollection groups = matches[0].Groups;
                    if (_banlist.UnbanUser(groups["user"].Value, groups["ident"].Value, groups["host"].Value)) {
                        Sender.Send(RawNotice.Generate(user.userName, $"Unbanned successfully: {parameters[1]}"));
                    } else {
                        Sender.Send(RawNotice.Generate(user.userName, $"Couldn't find the user, misspelled?"));
                    }
                    return;
                }
            }

            Sender.Send(RawNotice.Generate(user.userName, "The ban string seems malformed or missing, try: " +
                                                            $"{MessagePrefix}ban unban <user>!<ident>@<host>"));
        }

        /// <summary> Called when a notice message was received by the bot. </summary>
        /// <param name="message">The network message received.</param>
        public void OnNotice(CommandMessage message) { }
    }
}
