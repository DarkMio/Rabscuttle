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
        private List<ChannelUser> users;
        private ConnectionManager connection;

        public ChannelHandler(ConnectionManager connection) {
            this.connection = connection;
            channels = new List<Channel>();
            users = new List<ChannelUser>();
        }

        private ChannelUser FindUser(string source) {
            var userName = Regex.Match(source, @"^([^!@ ]*)").Value;
            return users.SingleOrDefault(s => s.userName == userName);
        }

        private ChannelUser FindOrCreateUser(string username) {
            ChannelUser user = FindUser(username);
            if (user == null) {
                user = new ChannelUser(username, true);
                users.Add(user);
            }
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
                Channel channel = channels.Single(s => s.channelName == message.typeParams);
                ChannelUser joined = new ChannelUser(message.prefix);
                ChannelUser user = users.SingleOrDefault(s => s.userName == joined.userName);
                if (user == null) {
                    users.Add(joined);
                    user = joined;
                }

                channel.AddUser(user, MemberCode.DEFAULT);
            }
        }

        private void HandlePart(NetworkMessage message) {
            Debug.WriteLine(message);
            if (message.fromServer) { // someone left the channel
                ChannelUser user = FindUser(message.prefix);
                Channel channel = FindChannel(message.typeParams);
                channel.RemoveUser(user);
            } else { // bot left the channel
                channels.Remove(channels.Single(s => s.channelName == message.typeParams));
                Debug.WriteLine("!! Could not handle (ChannelHandler): " + message);
            }
        }

        private void HandleQuit(NetworkMessage message) {
            Debug.WriteLine(message);
            if (message.fromServer) {
                ChannelUser user = users.SingleOrDefault(s => s.userName == message.typeParams);
                if (user == null) {
                    Debug.WriteLine("!! Could not find specified user. " + message);
                    return;
                }
                foreach (Channel channel in channels) {
                    channel.RemoveUser(user);
                }
            } else {
                Debug.WriteLine("!! Could not handle (ChannelHandler): " + message);
            }
        }

        private void HandlePrivMsg(NetworkMessage message) {
            Debug.WriteLine("Should handle: " + message);
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
            }
        }

        private void HandleUserlist(NetworkMessage message) {
            string[] element = message.message.Split(new char[] {' '});
            string[] param = message.typeParams.Split(new string[] {" @ "}, StringSplitOptions.None);
            string channelName = null;
            if (param.Length > 1) {
                channelName = param[1];
            } else {
                Debug.WriteLine("!! Could not handle (ChannelHandler): " + message);
                return;
            }

            Channel channel = FindChannel(channelName);
            foreach (var s in element) {
                ChannelUser user = FindOrCreateUser(s);
                channel.AddUser(user, MemberCode.DEFAULT);
            }
        }
    }
}
