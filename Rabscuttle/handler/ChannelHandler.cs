using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Rabscuttle.channel;
using Rabscuttle.networking;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.stuff;

namespace Rabscuttle.handler {

    /**
     * Creates its own observers to notify them individually.
     */
    /// <summary>
    /// The ChannelHandler handles all channel-related messages and generates data structures for channels, user and their relations to channels.
    /// </summary>
    /// <seealso cref="ObservableHandler" />
    /// <seealso cref="Channel"/>
    /// <seealso cref="ChannelUser"/>
    /// <seealso cref="UserRelation"/>
    public class ChannelHandler  : ObservableHandler {
        public HashSet<Channel> channels;
        public HashSet<ChannelUser> users;
        private readonly ISender _connection;
        private readonly string[] _operators;

        private static readonly Regex USER_REGEX = new Regex(@"^[+%@!.:]?([^!@ ]*)", RegexOptions.Compiled);
        private static readonly Regex PERMISSION_REGEX = new Regex(@"([+|-][^+-]*)([+|-][^+-]*)?", RegexOptions.Compiled);


        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelHandler"/> class.
        /// </summary>
        /// <param name="connection">A valid, sendable connection. Preferrably an instance of <see cref="ConnectionManager"/>.</param>
        public ChannelHandler(ISender connection){
            _connection = connection;
            channels = new HashSet<Channel>();
            users = new HashSet<ChannelUser>();
            _operators = ConfigurationProvider.Get("operators").Split(new []{", "}, StringSplitOptions.None);
        }

        /// <summary>
        /// Finds a user based on his username or his source string.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public ChannelUser FindUser(string source) {
            var userName = USER_REGEX.Matches(source)[0].Groups[1].Value;
            return users.SingleOrDefault(s => s.userName == userName);
        }

        /// <summary>
        /// Finds or creates a user based on his username or his source string.
        /// Also adds the user to the user-collection in this handler.
        /// </summary>
        /// <param name="source">The source string in <c>:~name!ident@host</c>.</param>
        /// <param name="isUserName">if set to <c>true</c> and a user has to  be created, .</param>
        /// <returns></returns>
        private ChannelUser FindOrCreateUser(string source, bool isUserName=false) {
            ChannelUser user = FindUser(source); // search
            if (user != null) { // if found, return him
                return user;
            } // otherwise, create a new
            user = new ChannelUser(source, isUserName);
            users.Add(user); // add him
            return user; // return him
        }

        /// <summary> Finds the channel based on its name <c>#name</c>. </summary>
        /// <param name="channelName">Name of the channel.</param>
        /// <returns></returns>
        public Channel FindChannel(string channelName) {
            return channels.SingleOrDefault(s => s.channelName == channelName);
        }

        /// <summary> Finds or creates a channel based on its name <c>#name</c> </summary>
        /// <param name="name">The name, including the <c>#</c></param>
        /// <returns></returns>
        private Channel FindOrCreateChannel(string name) {
            Channel channel = FindChannel(name);
            if (channel != null) {
                return channel;
            }
            channel = new Channel(name);
            channels.Add(channel);
            return channel;
        }

        /// <summary> Retrieves if the given username left all channels and is thereby not relevant anymore. </summary>
        /// <param name="user">A user which has to be found in any channel.</param>
        /// <returns></returns>
        private bool CheckIfLastUser(ChannelUser user) {
            foreach (Channel channel in channels) { // for all channels, search if he is in a channel
                var hasUsr = channel.users.SingleOrDefault(s => ReferenceEquals(s.user, user));
                if(hasUsr != null) { // if he's in a channel, return false
                    return false;
                }
            } // otherwise return true
            return true;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        /// <summary> Determines whether a given user is logged in or not asynchronously. Might call and wait for a whois-message. </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>A task, which retrieves a bool eventually.</returns>
        public async Task<bool> IsLoggedIn(string userName) {
            var user = FindUser(userName);
            if (user == null) {
                Logger.WriteWarn("Channel Handler", "Tried to search login-status for unknown user: {0}", userName);
                return false;
            }

            // if the login token was already set, we know his login status already
            if (user.loggedIn != ChannelUser.LoginStatus.DEFAULT) {
                return user.loggedIn == ChannelUser.LoginStatus.LOGGED_IN;
            }
            // otherwise we search for it, and wait until we got the response.
            _connection.Send(RawWhois.Generate(userName));
            var receiver = _connection as ConnectionManager;
            if (receiver == null) {
                return false;
            }

            receiver.ReceiveUntil(ReplyCode.RPL_ENDOFWHOIS);
            return user.loggedIn == ChannelUser.LoginStatus.LOGGED_IN;
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        /// <summary> Handles a <see cref="CommandCode"/>. </summary>
        /// <param name="message">The network message, pre-parsed.</param>
        public override void HandleCommand(NetworkMessage message) {
            switch ((CommandCode) message.typeEnum) {
                case CommandCode.JOIN: // someone joinimg
                    HandleJoin(message);
                    break;
                case CommandCode.PRIVMSG: // not sure if we need this one
                    HandlePrivMsg(message);
                    break;
                case CommandCode.PART: // leaving
                case CommandCode.KICK: // forcefully leaving
                    HandlePart(message);
                    break;
                case CommandCode.QUIT: // someone disconnecting
                    HandleQuit(message);
                    break;
                case CommandCode.AWAY: // not sure if I need to handle it at all.
                    break;
                case CommandCode.NICK: // renaming itself
                    HandleNick(message);
                    break;
                case CommandCode.MODE: // gaining / losing rights
                    HandleMode(message);
                    break;
            }
        }

        public override void HandleReply(NetworkMessage message) {
            switch ((ReplyCode) message.typeEnum) {
                case ReplyCode.RPL_NAMREPLY: // a list of users + rights per channel
                case ReplyCode.RPL_NAMREPLY_:
                    HandleUserlist(message);
                    break;
                case ReplyCode.RPL_WHOREPLY: // a full dataset of a single user in a channel
                    HandleWhoReply(message);
                    break;
                case ReplyCode.RPL_WHOISACCOUNT: // login information
                    HandleWhoIsAccount(message);
                    break;
            }
        }

        private void HandleJoin(NetworkMessage message) {
            if (!message.fromServer) {
                // Debug.WriteLine("CREATING NEW CHANNEL: " + message.message);
                var channelName = message.message ?? message.typeParams;
                channels.Add(new Channel(channelName));
            } else {
                Channel channel = FindOrCreateChannel(message.typeParams);
                ChannelUser user = FindOrCreateUser(message.prefix);
                channel.AddUser(user, MemberCode.DEFAULT);
            }
        }

        private void HandlePart(NetworkMessage message) {
            Channel channel = FindChannel(message.typeParams);
            if (channel == null) {
                Logger.WriteWarn("Channel Handler", "There was a part message without having a channel: {0}", message.typeParams);
                return;
            }

            if (message.fromServer) { // someone left the channel
                ChannelUser user = FindUser(message.prefix);
                channel.RemoveUser(user);
                if (CheckIfLastUser(user)) {
                    users.Remove(user);
                }
            } else { // bot left the channel
                var channelUsers = channel.users;
                channels.Remove(channel);

                foreach (UserRelation userRelation in channelUsers) {
                    users.RemoveWhere(s => CheckIfLastUser(userRelation.user));
                }
            }
        }


        private void HandleQuit(NetworkMessage message) {
            if (message.fromServer) { // someone left the server
                ChannelUser user = FindUser(message.prefix);
                if (user == null) {
                    Logger.WriteWarn("Channel Handler", "Could not find a quitting user: {0}", message.prefix);
                    return;
                }
                foreach (Channel channel in channels) {
                    channel.RemoveUser(user);
                }
                users.Remove(user);
            } else { // we quit, which means that we can safely delete all.
                Logger.WriteInfo("Channel Handler", "Emptying all channel infos.");
                channels.Clear();
                users.Clear();
                foreach (IObserver<NetworkMessage> observer in observers) { // and notify all event listener
                    observer.OnCompleted();
                }
            }
        }

        // ReSharper disable once UnusedParameter.Local - still not sure if it will be needed at some point.
        private void HandlePrivMsg(NetworkMessage message) {

        }

        private void HandleNick(NetworkMessage message) {
            if (message.fromServer) { // change the users username
                ChannelUser user = FindUser(message.prefix);
                user.userName = message.message;
            }
        }

        private void HandleMode(NetworkMessage message) {
            if (!message.fromServer) { // nothing of value, bot wants to change modes
                return;
            }

            string[] parameter = message.typeParams.Split(' ');
            if (parameter.Length < 3) {
                Logger.WriteWarn("Channel Handler", "No valid parameter info: {0} {1} :{2}", message.type, message.typeParams, message.message);
                return;
            }

            // mode sets can only happen in one channel
            Channel channel = FindChannel(parameter[0]);
            var mode = PERMISSION_REGEX.Matches(parameter[1])[0].Groups;
            // Positive or Negative Group
            string firstGroup = mode[1].Value;
            var firstParameterGroup = parameter.Skip(2).Take(firstGroup.Length - 1);
            UserRanking(firstGroup, firstParameterGroup.ToArray(), channel);

            // The reminder group - could be empty, which will be accounted for by UserRanking(...)
            string secondGroup = mode[2].Value;
            var secondParameterGroup = parameter.Skip(2 + firstGroup.Length);
            UserRanking(secondGroup, secondParameterGroup.ToArray(), channel);
        }

        /// <summary> Ranks multiple users from a RPL_NAMRPLY to their rights on the network. </summary>
        /// <param name="rankString">The string of mode-changes</param>
        /// <param name="channelUsers">The names of the users to be ranked</param>
        /// <param name="channel">The channel in which the rank-change happened</param>
        private void UserRanking(string rankString, string[] channelUsers, Channel channel) {
            if (rankString.Length == 0) {
                return;
            }
            bool upranking = rankString.StartsWith("+");
            string rankOrder = rankString.Substring(1);
            for (int i = 0; i < rankOrder.Length; i++) {
                char permissionChar = rankOrder.ElementAt(i);
                MemberCode permission = ParseModePermission(permissionChar);
                ChannelUser user = FindUser(channelUsers[i]);
                if (user == null) {
                    Logger.WriteWarn("Channel Handler", "Cannot find user of mode set: {0} in channel: {1}", channelUsers[i], channel.channelName);
                    continue;
                }

                if (_operators.SingleOrDefault(s => s == user.userName) != null) {
                    RawWhois.Generate(user.userName);
                }

                if (upranking) {
                    channel.AddRank(user, permission);
                } else {
                    channel.RemoveRank(user, permission);
                }
            }
        }

        /// <summary> Parses the mode permission for user modes (user ranking). </summary>
        /// <param name="permission">The permission character.</param>
        /// <returns>A <see cref="MemberCode"/> representing the permissions rank</returns>
        private MemberCode ParseModePermission(char permission) { // @TODO: Implement all other values
            if(permission == 'v' || permission == '+') return MemberCode.VOICED;
            if(permission == 'o' || permission == '@') return MemberCode.OPERATOR;
            // Debug.WriteLine("CANNOT FIND PERMISSION: " + permission);
            return MemberCode.DEFAULT;
        }

        /// <summary> Upon joining the server will tell the client which users are connected in this channel and their current, highest rank. </summary>
        /// <param name="message">The network message with RPL_NAMRPLY.</param>
        private void HandleUserlist(NetworkMessage message) {
            string[] usernames = message.message.Split(' ');
            string[] param = message.typeParams.Split(new[] {" @ ", " = ", " * "}, StringSplitOptions.None);
            string channelName;
            if (param.Length > 1) {
                channelName = param[1];
            } else {
                Logger.WriteWarn("Channel Handler", "TypeParams are too short to handle for UserList: {0}", message);
                return;
            }

            Channel channel = FindChannel(channelName);
            foreach (var username in usernames) {
                ChannelUser user = FindOrCreateUser(username, true);
                channel.AddUser(user, ParseModePermission(username[0]));
                channel.AddRank(user, ParseModePermission(username[0]));

                if (_operators.SingleOrDefault(s => s == user.userName) != null) {
                    // Send the whois request to figure out if he is logged in or not
                    // this will later land on the channel handler anyway ("when it's time for it")
                    _connection.Send(RawWhois.Generate(user.userName));
                }
            }
            _connection.Send(RawWho.Generate(channelName));
        }

        private void HandleWhoReply(NetworkMessage message) {
            if (message.FromClient) {
                return;
            }

            WhoReply data = new WhoReply(message);
            ChannelUser user = FindUser(data.username);
            // if(user)
            user.TryAddData(data.ident, data.host, data.server, data.hops, data.realname);
            Channel channel = FindChannel(data.channel);
            foreach (char c in data.modes) {
                channel.AddRank(user, ParseModePermission(c));
            }
        }

        private void HandleWhoIsAccount(NetworkMessage message) {
            string[] parameters = message.typeParams.Split(' ');
            if (parameters.Length < 3) {
                Logger.WriteWarn("Channel Handler",
                    "Misaligned RPL_WHOISACCOUNT message: Not enough Type parameters. Message: {0}", message);
                return;
            }

            var user = FindUser(parameters[1]);
            user.loggedIn = ChannelUser.LoginStatus.LOGGED_IN;
            user.loginUserName = parameters[2];
        }
    }
}
