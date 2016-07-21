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
            Send(User.Generate("Gabe BotHost AnotherOne", "Rabscuttle"));
            Send(Nick.Generate("Rabscootle"));
            ReceiveUntil(Ping.Instance); // The last received message will be a ping.
        }

        public void Send(NetworkMessage message) {
            scheduler.Add(message);
        }

        public NetworkMessage Receive(bool waitResponse = false) {
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
                if (message != null && command.type + "" == message.type) {
                    return message;
                }
            }
        }

        public NetworkMessage ReceiveLast(bool waitResponse = false) {
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

        /**
         * Handles an incoming message accordingly.
         * I honestly think that it wouldn't be bad to have two subclasses for
         * the two different kinds of messages, but then again this would either
         * require to split up them here, which then is kinda late, or analyze
         * the message beforehand, which doesn't seem nice either.
         * Help is appreciated.
         */

        private void Handle(NetworkMessage message) {
            if (Enum.IsDefined(typeof(ReplyCode), message.type)) {
                // We've received server info.
                Debug.WriteLine("?" + message);
                ReplyCode replyCode = (ReplyCode) Enum.Parse(typeof(ReplyCode), message.type, true);
                HandleReply(message, replyCode);

            }

            if (Enum.IsDefined(typeof(CommandCode), message.type)) {
                // We've received something to act upon.
                Debug.WriteLine("!" + message);
                CommandCode commandCode = (CommandCode) Enum.Parse(typeof(CommandCode), message.type, true);
                HandleCommand(message, commandCode);

            }
        }

        private void HandleReply(NetworkMessage message, ReplyCode code) {

        }

        private void HandleCommand(NetworkMessage message, CommandCode code) {
            switch (code) {
                case CommandCode.PING:
                    Send(Pong.Generate(message.message));
                    break;
                case CommandCode.PRIVMSG:
                    OnPrivMsg(message);
                    break;
                default:
                    return;
            }

            if (code == CommandCode.PING) {
                Send(Pong.Generate(message.message));
            }
        }

        private void OnPrivMsg(NetworkMessage message) {
            if (message.prefix == "DarkMio!~Mio@DarkMio.user.gamesurge") {
                if (message.message.StartsWith(">RAW")) {
                    client.RawSend(message.message.Replace(">RAW ", "") + "\r\n");
                }
                else if (message.message.StartsWith(">rejoin")) {
                    Send(Part.Generate("#w3x-to-vmf", "Cya"));
                    Send(Join.Generate("#w3x-to-vmf"));
                }
            }
        } // End OnPrivMsg
    }
}