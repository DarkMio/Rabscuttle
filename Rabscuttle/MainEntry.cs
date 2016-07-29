﻿using Rabscuttle.core;
using Rabscuttle.core.commands;
using Rabscuttle.core.io;
using Logger = Rabscuttle.stuff.Logger;

namespace Rabscuttle {
    public class MainEntry {
        static void Main(string[] args) {
            // Logger.Setup();
            Logger.WriteInfo("Main", "Connecting...");
            ConnectionManager cmgr = new ConnectionManager("irc.gamesurge.net", 6667);
            Logger.WriteInfo("Main", "Connected!");
            cmgr.ReceiveLast(true);

            cmgr.Send(RawJoin.Generate("#w3x-to-vmf"));
            cmgr.Send(RawJoin.Generate("#miomio"));
            // cmgr.Send(RawJoin.Generate("#dota2mods"));
            // cmgr.Send(RawJoin.Generate("#dota2mods"));
            // cmgr.Send(RawJoin.Generate("#steamdb"));
            // cmgr.Send(RawJoin.Generate("#trashtest"));
            /*
            for (int i = 0; i < 500; i++) {
                cmgr.Send(RawPrivMsg.Instance.Generate(false, null, "#w3x-to-vmf", "Hello: " + i));
            }
            */
            while (true) {
                var msg = cmgr.ReceiveUntil(CommandCode.PING);
            }

        }

        public class AuthServ : RawCommand<AuthServ> {
            public override CommandCode type => CommandCode.DEFAULT;
            public override bool hasTypeParameter => true;
            public override bool hasMessage => false;

            public static NetworkMessage Generate(string typeParameter, bool fromServer = false, string prefix = null) {
                return Instance.InstanceRawGenerate(fromServer, "AUTHSERV", prefix, typeParameter);
            }
        }
    }
}