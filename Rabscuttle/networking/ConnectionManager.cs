using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.handler;

namespace Rabscuttle.networking {
    public class ConnectionManager {
        private readonly BotClient client;
        private readonly CommandSchedule scheduler;

        private readonly ChannelHandler channelHandler;

        public ConnectionManager(string host, int port) {
            client = new BotClient(host, port);
            scheduler = new CommandSchedule(client);
            channelHandler = new ChannelHandler(this);
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
            ReceiveUntil(Mode.Instance); // The last received message will be a ping.
        }

        public void Send(NetworkMessage message) {
            Handle(message);
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
            try {
                if (message.typeEnum is ReplyCode) {
                    // We've received server info.
                    HandleReply(message);
                } else if (message.typeEnum is CommandCode) {
                    HandleCommand(message);
                }
            } catch (Exception e) {
                Debug.WriteLine("Exception: " + e);
                Debug.WriteLine(e.Source);
                Debug.WriteLine(e.StackTrace);
            }

        }

        private void HandleReply(NetworkMessage message) {
            switch ((ReplyCode) message.typeEnum) {
                case ReplyCode.RPL_NAMREPLY:
                case ReplyCode.RPL_NAMREPLY_:
                    channelHandler.HandleReply(message);
                    break;
            }
            Debug.WriteLine(message);
        }

        /**
         * If the message is a server command, we'll handle it here.
         */
        private void HandleCommand(NetworkMessage message) {
            Debug.WriteLine("HANDLE> " + message);
            switch ((CommandCode)message.typeEnum) {
                case CommandCode.PING:
                    Send(Pong.Generate(message.message));
                    break;
                case CommandCode.PRIVMSG:
                case CommandCode.KICK:
                case CommandCode.JOIN:
                case CommandCode.PART:
                case CommandCode.AWAY:
                case CommandCode.NICK:
                case CommandCode.QUIT:
                case CommandCode.MODE:
                    channelHandler.HandleCommand(message);
                    break;
                default:
                    return;
            }
        }

        private void OnPrivMsg(NetworkMessage message) {
            channelHandler.HandleCommand(message);
            /*
            if (message.prefix != null && message.prefix.StartsWith("DarkMio!~Mio@")) {
                if (message.message.StartsWith(">RAW")) {
                    client.RawSend(message.message.Replace(">RAW ", "") + "\r\n");
                }
                else if (message.message.StartsWith(">rejoin")) {
                    Send(Part.Generate("#w3x-to-vmf", "Cya"));
                    Send(Join.Generate("#w3x-to-vmf"));
                }
            }
            */
        } // End OnPrivMsg
    }
}
