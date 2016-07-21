using System;
using System.Diagnostics;
using System.IO;
using System.Net.Configuration;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;

namespace Rabscuttle.networking {
    public class BotClient : ISender, IReceiver {
        private string host;
        private int port;
        private TcpClient client;
        private NetworkStream dataStream;
        private StreamReader input;
        private StreamWriter output;

        public const int Timeout = 300000; // 300s until timeout; normal IRC timeout
        public const int KeepAliveInterval = 5000; // 5s to send keep alive
        public const int KeepAliveTime = 300000; // 300s until timeout

        public BotClient(string host, int port) {
            this.host = host;
            this.port = port;

            client = new TcpClient(host, port);
            client.SendTimeout = Timeout;
            dataStream = client.GetStream();
            input = new StreamReader(dataStream);
            output = new StreamWriter(dataStream);
            SetKeepAlive(client.Client);
        }

        private static void SetKeepAlive(Socket client) {
		    uint dummy = 0;
		    byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
		    BitConverter.GetBytes((uint)(KeepAliveTime)).CopyTo(inOptionValues, 0);
		    BitConverter.GetBytes((uint)KeepAliveTime).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
		    BitConverter.GetBytes((uint)KeepAliveInterval).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);


            int result = client.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);

        }

        public void Send(string message, string prefix, string type, string destination) {
            NetworkMessage msg = new NetworkMessage(prefix, type, destination, message, false);
            Send(msg);
        }

        public void Send(NetworkMessage message) {
            output.Write(message.BuildMessage());
            output.Flush();
            Debug.WriteLine(message);
        }

        public void RawSend(string message) {
            output.Write(message);
            output.Flush();
            Debug.WriteLine("CLIENT> RAW: " + message);
        }

        /**
         * Receives a message
         * @param waitResponse: waits for a message until there is one, if true yields null if there is no mesasge
         */
        public NetworkMessage Receive(bool waitResponse=false) {
            if (!dataStream.DataAvailable && !waitResponse) {
                return null;
            }

            string s = input.ReadLine();
            try {
                var msg = new NetworkMessage(s, true);
                return msg;
            } catch (ArgumentException e) {
                Debug.WriteLine(e);
                return null;
            }
        }

        /**
         * For debugging purposes, receives a single line of the network stack, no parsing.
         */
        public string RawReceive() {
            if (dataStream.DataAvailable) {
                return input.ReadLine();
            }
            return null;
        }

        /**
         * Disposes all incoming messages until the most recent is reached,
         * for example when the server sends multiple messages between receiving frames.
         * @param waitResponse: waits for the FIRST message to appear and loads then everything.
         * @return: null when nothing is there in the first place - like Receive does.
         */
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
