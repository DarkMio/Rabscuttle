using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Rabscuttle.channel;
using Rabscuttle.core.channel;
using Rabscuttle.networking.commands;

namespace Rabscuttle.networking.handler {

    /**
     * Creates its own observers to notify them individually.
     */
    public class ChannelHandler : ObservableHandler {
        private List<Channel> channels;
        public HashSet<ChannelUser> users;
        private ConnectionManager connection;

        private static Regex userRegex = new Regex(@"^[+%@!.:]?([^!@ ]*)", RegexOptions.Compiled);
        private static Regex permissionRegex = new Regex(@"([+|-][^+-]*)([+|-][^+-]*)?", RegexOptions.Compiled);


        public ChannelHandler(ConnectionManager connection) {
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
            Debug.WriteLine("> Null? > " + user);
            user = new ChannelUser(source, isUserName);
            Debug.WriteLine(">CREATING> " + user);
            users.Add(user);
            return user;
        }

        private Channel FindChannel(string channelName) {
            return channels.SingleOrDefault(s => s.channelName == channelName);
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


            }
        }

        private void HandleJoin(NetworkMessage message) {
            if (!message.fromServer) {
                channels.Add(new Channel(message.message));
            } else {
                Channel channel = FindChannel(message.typeParams);
                ChannelUser user = FindOrCreateUser(message.prefix, true);
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

            } else { // bot left the channel
                channels.Remove(channel);
            }
        }

        private void HandleQuit(NetworkMessage message) {
            Debug.WriteLine(message);
            if (message.fromServer) {
                ChannelUser user = FindUser(message.typeParams);
                if (user == null) {
                    Debug.WriteLine("!! Could not find specified user. " + message);
                    return;
                }
                foreach (Channel channel in channels) {
                    channel.RemoveUser(user);
                }
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
                    connection.Send(PrivMsg.Generate(message.typeParams, "Result: " + FindUser(x[1])));
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
            Debug.WriteLine("Handling Mode...");
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
            string firstGroup = mode[1].Value;
            var firstParameterGroup = parameter.Skip(2).Take(firstGroup.Length - 1);
            Debug.WriteLine(firstParameterGroup);
            UserRanking(firstGroup, firstParameterGroup.ToArray(), channel);

            string secondGroup = mode[2].Value;
            var secondParameterGroup = parameter.Skip(2 + firstGroup.Length);
            Debug.WriteLine(secondParameterGroup);
            UserRanking(secondGroup, secondParameterGroup.ToArray(), channel);

            Debug.WriteLine("Done Ranking.");

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
            Debug.WriteLine("CANNOT FIND PERMISSION: " + permission);
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
        }
    }
}
