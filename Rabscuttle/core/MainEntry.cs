using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Rabscuttle.networking;

namespace Rabscuttle {
    public class MainEntry {
        static void Main(string[] args) {
          
            Client ss = new Client("localhost", 6667);
            ReceiveEverything(ss);
            ss.Send("Real Name", null, "USER sessionId irc.foobar.net irc.foobar.net", null);
            Thread.Sleep(500);
            ReceiveEverything(ss);
            ss.Send(null, null, "NICK UncreativePersonName", null);
            Thread.Sleep(500);
            NetworkMessage msg = ss.Receive();
            Debug.WriteLine(msg);
            ss.Send(msg.message, null, "PONG", null);
            Thread.Sleep(500);
            ReceiveEverything(ss);
            ss.Send("A bunch of text.", null, "PING", null);
            Thread.Sleep(500);
            ReceiveEverything(ss);
            Debug.WriteLine("Done sending");
        }

        static void ReceiveEverything(Client client) {
            while (true) {
                try {
                    var response = client.Receive();
                    if (response == null) {
                        break;
                    }
                    Debug.WriteLine(response);
                } catch (ArgumentException e) {
                    Debug.WriteLine(e.Message);
                }
            }
        }
    }
}
