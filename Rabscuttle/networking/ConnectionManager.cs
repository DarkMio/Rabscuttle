using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
            var msg = client.Receive(waitResponse);
            if (msg != null) {
                Handle(msg);
            }
            return msg;
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

        private void Handle(NetworkMessage message) {
            if(Enum.IsDefined(typeof(ReplyCodes), message.type)) {
                // We've received server info.
                Debug.WriteLine("?" + message);
            }

            if (Enum.IsDefined(typeof(CommandCodes), message.type)) {
                // We've received something to act upon.
                Debug.WriteLine("!" + message);
                CommandCodes thing = (CommandCodes) Enum.Parse(typeof(CommandCodes), message.type, true);
                if (thing == CommandCodes.PING) {
                    Send(Pong.Instance.Generate(false, null, message.message));
                } else if (thing == CommandCodes.PRIVMSG && message.prefix =="DarkMio!~Mio@DarkMio.user.gamesurge") {
                    if(message.message.StartsWith(">RAW")) {
                        client.RawSend(message.message.Replace(">RAW ", "") + "\r\n");
                    }
                    if (message.message.StartsWith(">rejoin")) {
                        Send(Part.Instance.Generate(false, null, "#w3x-to-vmf", "Cya"));
                        Send(Join.Instance.Generate(false, null, null, "#w3x-to-vmf"));
                    }
                }
            }
        }
    }
}
