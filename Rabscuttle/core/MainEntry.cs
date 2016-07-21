﻿using System;
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
            Console.WriteLine("!> Connecting...");
            ConnectionManager cmgr = new ConnectionManager("irc.gamesurge.net", 6667);
            Console.WriteLine("!> Connected!");
            cmgr.ReceiveLast(true);

            cmgr.Send(Join.Instance.Generate(false, null, null, "#w3x-to-vmf"));

            /*
            for (int i = 0; i < 500; i++) {
                cmgr.Send(PrivMsg.Instance.Generate(false, null, "#w3x-to-vmf", "Hello: " + i));
            }
            */
            while (true) {
                var msg = cmgr.ReceiveUntil(Ping.Instance);
            }
        }

        public class AuthServ : RawCommand<AuthServ> {
            public override string type => "AUTHSERV";
            public override bool hasTypeParameter => true;
            public override bool hasMessage => false;
        }
    }
}
