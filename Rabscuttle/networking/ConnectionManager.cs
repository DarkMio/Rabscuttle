using System.Diagnostics;
using System.Threading;
using Rabscuttle.networking.commands;

namespace Rabscuttle.networking {
    public class ConnectionManager {
        private readonly BotClient client;
        private readonly CommandSchedule scheduler;

        public ConnectionManager(string host, int port) {
            client = new BotClient(host, port);
            scheduler = new CommandSchedule(client);
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
            Send(User.Instance.Generate(false, null, "Gabe BotHost AnotherOne", "Rabscuttle"));
            Send(Nick.Instance.Generate(false, null, "Rabscootle"));
            NetworkMessage msg = client.ReceiveUntil("PING"); // The last received message will be a ping.
            Send(Pong.Instance.Generate(false, null, msg.message)); // And the message of the ping needs to be answered.
        }

        public void Send(NetworkMessage message) {
            scheduler.Add(message);
        }

        public NetworkMessage Receive(bool waitResponse=false) {
            return client.Receive(waitResponse);
        }

        public NetworkMessage ReceiveUntil<T>(T command) where T : RawCommand<T>, new() {
            return client.ReceiveUntil(command);
        }

        public NetworkMessage ReceiveLast(bool waitResponse=false) {
            return client.ReceiveLast(waitResponse);
        }
    }
}
