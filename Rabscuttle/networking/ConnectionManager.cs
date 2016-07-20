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
            ReceiveUntil(Ping.Instance); // The last received message will be a ping.
        }

        public void Send(NetworkMessage message) {
            scheduler.Add(message);
        }

        public NetworkMessage Receive(bool waitResponse=false) {
            return client.Receive(waitResponse);
        }

        /**
         * Wait and receive until a certain message is found.
         * This will, if poorly used, stay stuck, since it's waiting for a certain message.
         */
        public NetworkMessage ReceiveUntil<T>(T command, bool waitResponse = false) where T : RawCommand<T>, new() {
            while (true) {
                var message = Receive(true);
                if (message != null && command.type == message.type) {
                    return message;
                }
            }
        }

        public NetworkMessage ReceiveLast(bool waitResponse=false) {
            NetworkMessage lastMessage = Receive(waitResponse);
            while (true) {
                var message = Receive();
                if (message == null) {
                    break;
                }
                lastMessage = message;
            }
            return lastMessage;
        }
    }
}
