using Rabscuttle.channel;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;

namespace Rabscuttle.plugins {
    public class CommandMessage {
        public ChannelUser user;
        public MemberCode permission;
        public string parameters;
        public string command;
        public string origin;
        public NetworkMessage message;
    }
}
