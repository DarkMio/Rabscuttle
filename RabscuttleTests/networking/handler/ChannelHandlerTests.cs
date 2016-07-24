using NUnit.Framework;
using Rabscuttle.networking.handler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Rabscuttle.core.channel;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;

namespace Rabscuttle.networking.handler.Tests {
    [TestFixture()]
    public class ChannelHandlerTests {

        private class MockSenderReceiver : ISender, IReceiver {
            private readonly Queue<string> messages = new Queue<string>(new [] {
                ":Rabscootle!~Gabe@Somehost.com JOIN #w3x-to-vmf",
                ":TAL.DE.EU.GameSurge.net RPL_NAMREPLY Rabscootle @ #w3x-to-vmf :Rabscootle +Perry Yoshi2 +DarkMio @SinZ Not-efb8 @penguinwizzard Rabscuttle Renol @ChanServ",
                ":Rabscootle!~Gabe@Somehost.com JOIN #miomio",
                ":TAL.DE.EU.GameSurge.net RPL_NAMREPLY Rabscootle = #miomio @Rabscootle",
                ":TAL.DE.EU.GameSurge.net RPL_WHOREPLY Rabscootle #w3x-to-vmf ~Gabe Somehost.com *.GameSurge.net Rabscootle Hx :0 Rabscuttle",
                ":TAL.DE.EU.GameSurge.net RPL_WHOREPLY Rabscootle #w3x-to-vmf ~Perry Perry.user.gamesurge *.GameSurge.net Perry H+x :3 realname",
                ":TAL.DE.EU.GameSurge.net RPL_WHOREPLY Rabscootle #w3x-to-vmf ~Yoshi2 gamesurge.com *.GameSurge.net Yoshi2 Hx :3 realname",
                ":TAL.DE.EU.GameSurge.net RPL_WHOREPLY Rabscootle #w3x-to-vmf ~Mio DarkMio.user.gamesurge *.GameSurge.net DarkMio H+x :3 realname",
                ":TAL.DE.EU.GameSurge.net RPL_WHOREPLY Rabscootle #w3x-to-vmf ~SinZ SinZ.user.gamesurge *.GameSurge.net SinZ H@x :3 SinZ",
                ":TAL.DE.EU.GameSurge.net RPL_WHOREPLY Rabscootle #w3x-to-vmf ~Gabe Somehost.com *.GameSurge.net Rabscootle H@x :0 Rabscuttle",
                ":DarkMio!~Mio@DarkMio.user.gamesurge JOIN #miomio",
                ":DarkMio!~Mio@DarkMio.user.gamesurge PART #miomio :Leaving"
            });
            public void Send(string message, string prefix, string type, string typeParams) {

            }

            public void Send(NetworkMessage message) {

            }

            public NetworkMessage Receive(bool waitResponse = false) {
                if (messages.Count > 0) {
                    return new NetworkMessage(messages.Dequeue(), true);
                } else {
                    return null;
                }
            }
        }

        [Test()]
        public void HandlerTest() {
            var mock = new MockSenderReceiver();
            ChannelHandler ch = new ChannelHandler(mock);
            var nm = mock.Receive();
            while (nm != null) {
                if (nm.typeEnum is CommandCode) {
                    ch.HandleCommand(nm);
                } else {
                    ch.HandleReply(nm);
                }
                nm = mock.Receive();
            }

            Console.WriteLine(ch.users);
        }
    }
}
