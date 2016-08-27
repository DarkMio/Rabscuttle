using System;
using Rabscuttle.handler;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;
using Rabscuttle.stuff;
using Logger = Rabscuttle.stuff.Logger;

namespace Rabscuttle.networking {
    /// <summary>
    /// Main connection class. It's main task is to send and receive.
    /// Every to-be sent message and every received message gets filtered and dispatched into the public seend handlers.
    /// Each process and plugin can subscribe to these handlers and listen to the same messages.
    /// </summary>
    public class ConnectionManager : ISender, IReceiver, IDisposable {
        private readonly BotClient _client;
        private readonly CommandSchedule _scheduler;

        /// <summary>
        /// The channel handler processes all channel-related messages, like JOIN, PART, MODE and so on.
        /// </summary>
        public readonly ChannelHandler channelHandler;

        public readonly PluginHandler pluginHandler;

        public readonly VoidHandler voidHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionManager"/> class, thereby initilizing <see cref="BotClient"/>, <see cref="CommandSchedule"/>, <see cref="ChannelHandler"/>.
        /// @TODO: Add missing handler into description.
        /// </summary>
        /// <param name="host">Hostname, may be a domain or IP as string</param>
        /// <param name="port">Server port, standard is 6667</param>
        public ConnectionManager(string host, int port = 6667) {
            _client = new BotClient(host, port);
            _scheduler = new CommandSchedule(_client);
            channelHandler = new ChannelHandler(this);
            voidHandler = new VoidHandler();
            pluginHandler = new PluginHandler(this, channelHandler, voidHandler);
            Connect();
        }


        /// <summary>
        /// Connects to a server and waits until any of the connection-related server message is reached.
        /// Server-Related messages are <c>MODE</c>, <c>RPL_ENDOFMOTD</c>, <c>ERR_NOMOTD</c>
        /// </summary>
        private void Connect() {
            Send(RawUser.Generate("Gabe BotHost AnotherOne", ConfigurationProvider.Get("realname")));
            Send(RawNick.Generate(ConfigurationProvider.Get("username")));
            ReceiveUntil(CommandCode.MODE, ReplyCode.RPL_ENDOFMOTD, ReplyCode.ERR_NOMOTD); // The last received message will be any of those
        }

        /// <summary>
        /// Sends a properly aligned plaintext message to the endpoint, alignment looks like:
        /// <c>:Source* Type typeParam* :message*</c>
        /// </summary>
        /// <param name="message">Message contents, which are usually at the end of a string.</param>
        /// <param name="prefix">The Source prefix, _client to server messages can have this missing.</param>
        /// <param name="type">Message Type, usually a <c>CommandCode</c> when sending from _client to server.</param>
        /// <param name="typeParams">Optional, additional parameter for a message Type.</param>
        public void Send(string message, string prefix, string type, string typeParams) {
            Send(new NetworkMessage(prefix, type, typeParams, message, false));
        }

        /// <summary>
        /// Sends the specified message as soon as possible, which is managed by <see cref="CommandSchedule"/>
        /// </summary>
        /// <param name="message">Any instance of NetworkMessage.</param>
        public void Send(NetworkMessage message) {
            Handle(message);
            _scheduler.Add(message);
        }

        public void Send(NetworkMessage[] messages) {
            foreach (NetworkMessage networkMessage in messages) {
                Send(networkMessage);
            }
        }

        /// <summary>
        /// Receives a message instantaneously unless <paramref name="waitResponse"/> is set to <c>true</c>.
        /// </summary>
        /// <param name="waitResponse">if set to <c>true</c> method waits until there is a message to receive.</param>
        /// <returns>
        ///     Most recent NetworkMessage if there is any data, or <c>null</c> if <paramref name="waitResponse" /> is
        ///     <c>false</c>.
        /// </returns>
        public NetworkMessage Receive(bool waitResponse = false) {
            var msg = _client.Receive(waitResponse);
            if (msg != null) {
                Handle(msg);
            }
            return msg;
        }

        /// <summary>
        /// Receives messages until a certain message Type is found.
        /// This will, if poorly used, stay stuck since it's waiting for certain messages.
        /// Receives a message instantaneously.
        /// </summary>
        /// <returns>
        ///     The most recent NetworkMessage with any or the types awaiting.
        /// </returns>
        public NetworkMessage ReceiveUntil(params Enum[] replyOrCommand) {
            while (true) {
                var message = Receive(true);
                foreach (var type in replyOrCommand) {
                    if (message != null && type + "" == message.type) {
                        return message;
                    }
                }
            }
        }

        /// <summary>
        ///     Disposes all incoming messages until the most recent message is reached.
        ///     This is useful if there are multiple messages sent by the server in between of the _client receiving frames.
        /// </summary>
        /// <param name="waitResponse">if set to <c>true</c> method waits until there is a message to receive.</param>
        /// <returns>
        ///     Most recent NetworkMessage if there is any data, or <c>null</c> if <paramref name="waitResponse" /> is
        ///     <c>false</c>.
        /// </returns>
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

        /// <summary>
        /// Handles an incoming message accordingly.
        /// </summary>
        /// <param name="message">Incoming network message.</param>
        private void Handle(NetworkMessage message) {
            try {
                if (message.typeEnum is ReplyCode) {
                    // We've received server info.
                    HandleReply(message);
                    voidHandler.HandleReply(message); // just dump it into the handler
                } else if (message.typeEnum is CommandCode) {
                    HandleCommand(message);
                    voidHandler.HandleCommand(message); // here aswell
                }
            } catch (Exception e) {
                Logger.WriteError("Connection Manager", "Exception catched, thrown by handlers: {0}", e);
            }
        }

        /// <summary>
        /// Handles a reply by sorting it and sending it to the appropiate handler.
        /// </summary>
        /// <param name="message">Incoming message with any reply response.</param>
        private void HandleReply(NetworkMessage message) {
            switch ((ReplyCode) message.typeEnum) {
                case ReplyCode.RPL_NAMREPLY:
                case ReplyCode.RPL_NAMREPLY_:
                case ReplyCode.RPL_WHOREPLY:
                case ReplyCode.RPL_WHOISACCOUNT:
                    channelHandler.HandleReply(message);
                    break;
            }
        }

        /// <summary>
        /// Handles a command by sorting it and sending it to the appropiate handler.
        /// </summary>
        /// <param name="message">Incoming message with any command response.</param>
        private void HandleCommand(NetworkMessage message) {
            if ((CommandCode) message.typeEnum == CommandCode.PRIVMSG) {
                pluginHandler.HandleCommand(message);
            }
            switch ((CommandCode)message.typeEnum) {
                case CommandCode.PING:
                    Send(RawPong.Generate(message.message));
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

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }
            _client?.Dispose();
            pluginHandler?.Dispose();
        }
    }
}
