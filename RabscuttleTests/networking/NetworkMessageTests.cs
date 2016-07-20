﻿using NUnit.Framework;
using Rabscuttle.networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabscuttle.networking.Tests {
    [TestFixture()]
    public class NetworkMessageTests {
        [Test()]
        public void NetworkMessageRawGeneration() {
            Assert.AreEqual(
                new NetworkMessage(":m@m.net PRIVMSG #mychannel :Hiya, buddy.", false).ToString(),
                "CLIENT> P: [ m@m.net ] | T:[ PRIVMSG ] | D: [ #mychannel ] | M: [ Hiya, buddy. ];"
            );
            Assert.AreEqual(
                new NetworkMessage("PING :server.net", false).ToString(),
                "CLIENT> P: [  ] | T:[ PING ] | D: [  ] | M: [ server.net ];"
            );
        }

        [Test()]
        public void NetworkMessageMessageBuild() {
            string message = ":m@m.net PRIVMSG #mychannel :Hiya, buddy.\r\n";
            NetworkMessage networkMessage = new NetworkMessage(message, true);
            Assert.AreEqual(message, networkMessage.BuildMessage());
        }
    }
}