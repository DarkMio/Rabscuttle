using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Rabscuttle.networking;
using Rabscuttle.networking.commands;

namespace Rabscuttle {
    public class MainEntry {
        static void Main(string[] args) {
            Debug.WriteLine("Connecting...");
            ConnectionManager cmgr = new ConnectionManager("localhost", 6667);
            Debug.WriteLine("Connected!");
            cmgr.client.ReceiveLast(true);
            Thread.Sleep(500);
            cmgr.client.Send(Join.Instance.Generate(false, null, null, "#w3x-to-vmf"));
            Thread.Sleep(500);
            cmgr.client.Send(PrivMsg.Instance.Generate(false, null, "#w3x-to-vmf", "Hello world!"));
            for (int i = 0; i < 500; i++) {
                cmgr.Send(PrivMsg.Instance.Generate(false, null, "#w3x-to-vmf", "Hello: " + i));
            }
            while (true) {
                var msg = cmgr.client.ReceiveUntil(Ping.Instance);
                cmgr.client.Send(Pong.Instance.Generate(false, null, msg.message));
            }
        }
    }
}
