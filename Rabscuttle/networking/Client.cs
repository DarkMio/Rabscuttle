using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace Rabscuttle.networking {
    public class Client : ISender, IReceiver {
        private string host;
        private int port;
        private TcpClient client;
        private NetworkStream dataStream;
        private StreamReader input;
        private StreamWriter output;

        struct tcp_keepalive {
	            ulong  onoff;
	            ulong  keepalivetime;
	            ulong  keepaliveinterval;
		        };

        public const int Timeout = 300000; // 300s until timeout; normal IRC timeout
        public const int KeepAliveInterval = 5000; // 5s to send keep alive
        public const int KeepAliveTime = 300000; // 300s until timeout

        public Client(string host, int port) {
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
            NetworkMessage msg = new NetworkMessage(message, prefix, type, destination, false);
            output.Write(msg.BuildMessage());
            output.Flush();
            Debug.WriteLine(msg);
            // throw new System.NotImplementedException();
        }

        public void Send(NetworkMessage message) {
            throw new System.NotImplementedException();
        }

        public NetworkMessage Receive() {
            if (dataStream.DataAvailable) {
                string s = input.ReadLine();
                return new NetworkMessage(s, true);
            }
            return null;
        }

        public string RawReceive() {
            if (dataStream.DataAvailable) {
                return input.ReadLine();
            }
            return null;
        }
    }
}
