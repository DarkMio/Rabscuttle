#region

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Rabscuttle.networking.io;

#endregion

namespace Rabscuttle.networking {
    /// <summary>
    ///     The Bot Client is the bare TCP Connection from and to the server.
    ///     Contains no hard logic besides some helper methods. No history either.
    /// </summary>
    /// <seealso cref="Rabscuttle.networking.io.ISender" />
    /// <seealso cref="Rabscuttle.networking.io.IReceiver" />
    public class BotClient : ISender, IReceiver {
        /// <summary> TCP Timeout, 180s until timeout, which is the usual IRC timeout until disconnect.</summary>
        public const int Timeout = 180000; // 300s until timeout; normal IRC timeout

        /// <summary>The keep alive interval sends every 5 seconds.</summary>
        public const int KeepAliveInterval = 5000; // 5s to send keep alive

        /// <summary>The keep alive time closes the connection after 180s, which</summary>
        public const int KeepAliveTime = 180000; // 300s until timeout

        private readonly TcpClient client;
        private readonly NetworkStream dataStream;
        private readonly StreamReader input;
        private readonly StreamWriter output;
        private string host;
        private int port;


        /// <summary>
        ///     Initializes a new instance of the <see cref="BotClient" /> class.
        ///     Upon construction a connection gets setup, which may take a while.
        /// </summary>
        /// <param name="host">The connection host, might be hostname or IP.</param>
        /// <param name="port">The hosts connection port.</param>
        public BotClient(string host, int port = 6667) {
            this.host = host;
            this.port = port;
            // @TODO: Maybe offload connection process with async
            client = new TcpClient(host, port);
            client.SendTimeout = Timeout;
            dataStream = client.GetStream();
            input = new StreamReader(dataStream);
            output = new StreamWriter(dataStream);
            SetKeepAlive(client.Client);
        }

        /// <summary>
        ///     Receives a message.
        /// </summary>
        /// <param name="waitResponse">if set to <c>true</c> method waits until there is a message to receive.</param>
        /// <returns>NetworkMessage if there is any data, or <c>null</c> if <paramref name="waitResponse" /> is <c>false</c>.</returns>
        public NetworkMessage Receive(bool waitResponse = false) {
            if (!dataStream.DataAvailable && !waitResponse) {
                return null;
            }

            var s = input.ReadLine();
            try {
                var msg = new NetworkMessage(s, true);
                return msg;
            } catch (ArgumentException e) {
                Debug.WriteLine(e);
                return null;
            }
        }


        /// <summary> Sends a parameterised network message. </summary>
        /// <param name="message">The message.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="type">The type.</param>
        /// <param name="destination">The destination.</param>
        /// <remarks>Does not check for validity!</remarks>
        public void Send(string message, string prefix, string type, string destination) {
            var msg = new NetworkMessage(prefix, type, destination, message, false);
            Send(msg);
        }

        /// <summary> Sends the network message and flushes the socket. </summary>
        /// <param name="message">Pre-assembled network message</param>
        public void Send(NetworkMessage message) {
            output.Write(message.BuildMessage());
            output.Flush();
        }

        /// <summary>
        ///     Sets the keep alive tokens for the tcp connection
        /// </summary>
        /// <param name="client">A not-terminated TCP socket.</param>
        private static void SetKeepAlive(Socket client) {
            uint dummy = 0;
            // a byte map to write the values in.
            var inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
            // very raw variant to set the given parameters by moving them in the byte-map.
            BitConverter.GetBytes((uint) KeepAliveTime).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes((uint) KeepAliveTime).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
            BitConverter.GetBytes((uint) KeepAliveInterval).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);

            client.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
        }

        /// <summary>
        ///     Sends a raw plaintext message to the server, replaces all line-endings before to conform with IRCs exchange rules.
        /// </summary>
        /// <param name="message">A <c>string</c> with a single line of plaintext</param>
        public void RawSend(string message) {
            message = message.Replace("\r", "").Replace("\n", "");
            // in case you have gotten any linesbreaks from somewhere.
            output.Write(message + "\r\n");
            output.Flush();
            Debug.WriteLine("CLIENT> RAW: [ " + message + " ];");
        }

        /// <summary>
        ///     For debugging purposes, receives a single line from the network stack, no parsing.
        /// </summary>
        /// <returns>A single line of plaintext or <c>null</c> if there's no data</returns>
        public string RawReceive() {
            if (dataStream.DataAvailable) {
                return input.ReadLine();
            }
            return null;
        }

        /// <summary>
        ///     Disposes all incoming messages until the most recent message is reached.
        ///     This is useful if there are multiple messages sent by the server in between of the client receiving frames.
        /// </summary>
        /// <param name="waitResponse">if set to <c>true</c> method waits until there is a message to receive.</param>
        /// <returns>
        ///     Most recent NetworkMessage if there is any data, or <c>null</c> if <paramref name="waitResponse" /> is
        ///     <c>false</c>.
        /// </returns>
        public NetworkMessage ReceiveLast(bool waitResponse = false) {
            NetworkMessage lastMessage = Receive(waitResponse);
            while (true) {
                NetworkMessage message = Receive();
                if (message == null) {
                    break;
                }
                lastMessage = message;
            }
            return lastMessage;
        }
    }
}
