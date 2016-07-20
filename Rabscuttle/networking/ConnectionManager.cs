using System.Diagnostics;
using System.Threading;

namespace Rabscuttle.networking {
    public class ConnectionManager {
        public readonly BotClient client;

        public ConnectionManager(string host, int port) {
            client = new BotClient(host, port);
            Connect();
        }
        /*
        public ConnectionManager ConnectionManagerFactory(string host, int port) {
            ConnectionManager cmgr = new ConnectionManager(host, port);
            cmgr.Connect();
            return cmgr;
        }
        */

        private void Connect() {
            client.Send("Real Name", null, "USER sessionId irc.foobar.net irc.foobar.net", null);
            client.Send(null, null, "NICK UncreativePersonName", null);
            // client.ReceiveLast(true);
            NetworkMessage msg = client.ReceiveUntil("PING"); // The last received message will be a ping.
            client.Send(msg.message, null, "PONG", null); // And the message of the ping needs to be answered.
        }
    }
}
