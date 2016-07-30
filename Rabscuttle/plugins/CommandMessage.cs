using Rabscuttle.core.channel;
using Rabscuttle.core.commands;
using Rabscuttle.core.io;

namespace PluginContract {
    public class CommandMessage {
        public ChannelUser user;
        public MemberCode permission;
        public string parameters;
        public string command;
        public string origin;
        public NetworkMessage message;
    }
}
