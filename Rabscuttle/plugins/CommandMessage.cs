using Rabscuttle.core.channel;
using Rabscuttle.core.io;

namespace PluginContract {
    public class CommandMessage {
        public ChannelUser user;
        public string parameters;
        public string command;
        public string origin;
        public NetworkMessage message;
    }
}
