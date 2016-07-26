using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Rabscuttle.channel;
using Rabscuttle.core.channel;
using Rabscuttle.core.commands;
using Rabscuttle.core.io;

namespace Rabscuttle.core.handler {

    /**
     * Creates its own observers to notify them individually.
     */
    public class ChannelHandler  : ObservableHandler {
        public List<Channel> channels;
        public HashSet<ChannelUser> users;
        private ISender connection;

        private static Regex userRegex = new Regex(@"^[+%@!.:]?([^!@ ]*)", RegexOptions.Compiled);
        private static Regex permissionRegex = new Regex(@"([+|-][^+-]*)([+|-][^+-]*)?", RegexOptions.Compiled);


        public ChannelHandler(ISender connection){
            this.connection = connection;
            channels = new List<Channel>();
            users = new HashSet<ChannelUser>();
        }

        private ChannelUser FindUser(string source) {
            var userName = userRegex.Matches(source)[0].Groups[1].Value;
            return users.SingleOrDefault(s => s.userName == userName);
        }

        private ChannelUser FindOrCreateUser(string source, bool isUserName=false) {
            ChannelUser user = FindUser(source);
            if (user != null) {
                return user;
            }
            user = new ChannelUser(source, isUserName);
            users.Add(user);
            return user;
        }

        private Channel FindChannel(string channelName) {
            return channels.SingleOrDefault(s => s.channelName == channelName);
        }

        private Channel FindOrCreateChannel(string name) {
            Channel channel = FindChannel(name);
            if (channel != null) {
                return channel;
            }
            channel = new Channel(name);
            channels.Add(channel);
            return channel;
        }

        public override void HandleCommand(NetworkMessage message) {
            switch ((CommandCode) message.typeEnum) {
                case CommandCode.JOIN:
                    HandleJoin(message);
                    break;
                case CommandCode.PRIVMSG:
                    HandlePrivMsg(message);
                    break;
                case CommandCode.PART:
                case CommandCode.KICK:
                    HandlePart(message);
                    break;
                case CommandCode.QUIT:
                    HandleQuit(message);
                    break;
                case CommandCode.AWAY: // not sure if I need to handle it at all.
                    break;
                case CommandCode.NICK:
                    HandleNick(message);
                    break;
                case CommandCode.MODE:
                    HandleMode(message);
                    break;
            }
        }

        public override void HandleReply(NetworkMessage message) {
            switch ((ReplyCode) message.typeEnum) {
                case ReplyCode.RPL_NAMREPLY:
                case ReplyCode.RPL_NAMREPLY_:
                    HandleUserlist(message);
                    break;
                case ReplyCode.RPL_WHOREPLY:
                    HandleWhoReply(message);
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
            Debug.WriteLine(message);
            Channel channel = FindChannel(message.typeParams);
            if (channel == null) {
                return;
            }

            if (message.fromServer) { // someone left the channel
                ChannelUser user = FindUser(message.prefix);
                channel.RemoveUser(user);
                if (CheckIfLastUser(user)) {
                    users.Remove(user);
                }
            } else { // bot left the channel
                var users = channel.users;
                channels.Remove(channel);

                foreach (UserRelation userRelation in users) {
                    this.users.RemoveWhere(s => CheckIfLastUser(userRelation.user));
                }
            }
        }

        private bool CheckIfLastUser(ChannelUser user) {
            var notLast = false;
            foreach (Channel channel in channels) {
                var hasUsr = channel.users.SingleOrDefault(s => s.user == user);
                notLast = hasUsr != null;
            }
            return !notLast;
        }

        private void HandleQuit(NetworkMessage message) {
            Debug.WriteLine(message);
            if (message.fromServer) {
                ChannelUser user = FindUser(message.prefix);
                if (user == null) {
                    Debug.WriteLine("!! Could not find specified user. " + message);
                    return;
                }
                foreach (Channel channel in channels) {
                    channel.RemoveUser(user);
                }
                users.Remove(user);
            } else { // we quit, which means that we can safely delete all.
                Debug.WriteLine("CHOO CHOO. EMPTYING ALL CHANNEL INFOS");
                channels.Clear();
                users.Clear();
                foreach (IObserver<NetworkMessage> observer in observers) { // and notify all event listener
                    observer.OnCompleted();
                }
            }
        }

        private void HandlePrivMsg(NetworkMessage message) {
            if (message.message.StartsWith(">DUMPCHANS")) {
                foreach (Channel channel in channels) {
                    Debug.WriteLine(channel);
                }
            } else if (message.message.StartsWith(">RAW ")) {
                connection.Send(new NetworkMessage(message.message.Replace(">RAW ", ""), false));
            } else if (message.message.StartsWith(">DUMPUSRS")) {
                foreach (ChannelUser user in users) {
                    Debug.WriteLine(user);
                }
            } else if (message.message.StartsWith(">FINDUSR ")) {
                var x = message.message.Split(new char[] {' '});

                if(x.Length > 1)
                    connection.Send(RawPrivMsg.Generate(message.typeParams, "Result: " + FindUser(x[1])));
            } else if (message.message.StartsWith("fuck sql")) {
                connection.Send(RawPrivMsg.Generate(message.typeParams, "+fuck SQL"));
            }
        }

        private void HandleNick(NetworkMessage message) {
            if (message.fromServer) {
                ChannelUser user = FindUser(message.prefix);
                user.userName = message.message;
            }
            // Debug.WriteLine("Not smart enough for: " + message);
        }

        private void HandleMode(NetworkMessage message) {
            if (!message.fromServer) {
                return;
            }

            string[] parameter = message.typeParams.Split(new char[] {' '});
            if (parameter.Length < 3) {
                Debug.WriteLine("There was no mode parameter info: " + message);
                return;
            }

            // mode sets can only happen in one channel
            Channel channel = FindChannel(parameter[0]);
            var mode = permissionRegex.Matches(parameter[1])[0].Groups;
            // Positive or Negative Group
            string firstGroup = mode[1].Value;
            var firstParameterGroup = parameter.Skip(2).Take(firstGroup.Length - 1);
            UserRanking(firstGroup, firstParameterGroup.ToArray(), channel);

            // The reminder group - could be empty, which will be accounted for by UserRanking(...)
            string secondGroup = mode[2].Value;
            var secondParameterGroup = parameter.Skip(2 + firstGroup.Length);
            UserRanking(secondGroup, secondParameterGroup.ToArray(), channel);
        }

        private void UserRanking(string rankString, string[] users, Channel channel) {
            if (rankString.Length == 0) {
                return;
            }
            bool upranking = rankString.StartsWith("+");
            string rankOrder = rankString.Substring(1);
            for (int i = 0; i < rankOrder.Length; i++) {
                char permissionChar = rankOrder.ElementAt(i);
                MemberCode permission = ParseModePermission(permissionChar);
                ChannelUser user = FindUser(users[i]);
                if (user == null) {
                    Debug.WriteLine("Cannot find user of modeset: " + user + " in Channel: " + channel.channelName);
                    continue;
                }
                if (upranking) {
                    channel.AddRank(user, permission);
                } else {
                    channel.RemoveRank(user, permission);
                }
            }
        }

        private MemberCode ParseModePermission(char permission) {
            if(permission == 'v' || permission == '+') return MemberCode.VOICED;
            if(permission == 'o' || permission == '@') return MemberCode.OPERATOR;
            // Debug.WriteLine("CANNOT FIND PERMISSION: " + permission);
            return MemberCode.DEFAULT;
        }

        private void HandleUserlist(NetworkMessage message) {
            string[] usernames = message.message.Split(new char[] {' '});
            string[] param = message.typeParams.Split(new string[] {" @ ", " = ", " * "}, StringSplitOptions.None);
            string channelName = null;
            if (param.Length > 1) {
                channelName = param[1];
            } else {
                Debug.WriteLine("!! Could not handle (ChannelHandler): " + message);
                return;
            }

            Channel channel = FindChannel(channelName);
            foreach (var username in usernames) {
                ChannelUser user = FindOrCreateUser(username, true);
                channel.AddUser(user, ParseModePermission(username[0]));
                channel.AddRank(user, ParseModePermission(username[0]));
            }

            connection.Send(RawWho.Generate(channelName));
        }

        private void HandleWhoReply(NetworkMessage message) {
            WhoReply data = new WhoReply(message);
            ChannelUser user = FindUser(data.username);
            user.TryAddData(data.ident, data.host, data.server, data.hops, data.realname);
            Channel channel = FindChannel(data.channel);
            foreach (char c in data.modes) {
                channel.AddRank(user, ParseModePermission(c));
            }
        }
    }
}
