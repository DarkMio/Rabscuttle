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
            Debug.WriteLine("Connecting...");
            ConnectionManager cmgr = new ConnectionManager("localhost", 6667);
            Debug.WriteLine("Connected!");
            Thread.Sleep(4000);
            cmgr.client.Send("#dota2mods", "source", "JOIN", null);
            Thread.Sleep(500);
            cmgr.client.Send("Hello world.", "source", "PRIVMSG #dota2mods", null);
            Thread.Sleep(300000);
        }
    }
}
