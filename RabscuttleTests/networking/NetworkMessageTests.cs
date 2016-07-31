using NUnit.Framework;
using Rabscuttle.networking.commands;
using Rabscuttle.networking.io;

namespace RabscuttleTests.networking {
    [TestFixture()]
    public class NetworkMessageTests {

        private static readonly NetworkMessage SERVER_TEST_NAMREPLY = new NetworkMessage(
            "Rabscootle!~rabs@rabs.com",
            "Rabscootle #dota2mods",
            "+A @B C",
            true,
            ReplyCode.RPL_NAMREPLY
        );
        private static readonly NetworkMessage SERVER_TEST_MODE = new NetworkMessage(
            "services.esper.net",
            "#foo-bar +o foobar  ",
            null,
            true,
            CommandCode.MODE
        );
        private static readonly NetworkMessage CLIENT_TEST_SUBJECT = new NetworkMessage(
            null,
            "#dota2mods",
            "Hello world!",
            false,
            CommandCode.PRIVMSG
        );

        [Test()]
        public void NetworkMessageRawGeneration() {
            // Testing Client Subject
            Assert.AreEqual(
                CLIENT_TEST_SUBJECT.ToString(),
                "CLIENT> P: [  ] | T: [ PRIVMSG ] | TP: [ #dota2mods ] | M: [ Hello world! ];"
            );
            Assert.AreEqual(
                CLIENT_TEST_SUBJECT,
                new NetworkMessage("PRIVMSG #dota2mods :Hello world!", false)
            );

            // Testing Server NAMREPLY
            Assert.AreEqual(
                "SERVER> P: [ Rabscootle!~rabs@rabs.com ] | T: [ RPL_NAMREPLY ] | TP: [ Rabscootle #dota2mods ] | M: [ +A @B C ];",
                SERVER_TEST_NAMREPLY.ToString()

            );
            Assert.AreEqual(
                SERVER_TEST_NAMREPLY,
                new NetworkMessage(":Rabscootle!~rabs@rabs.com 353 Rabscootle #dota2mods :+A @B C", true)
            );

            // Testing Server MODE
            Assert.AreEqual(
                "SERVER> P: [ services.esper.net ] | T: [ MODE ] | TP: [ #foo-bar +o foobar   ] | M: [  ];",
                SERVER_TEST_MODE.ToString()
            );
            Assert.AreEqual(
                SERVER_TEST_MODE,
                new NetworkMessage(":services.esper.net MODE #foo-bar +o foobar  ", true)
            );
        }

        [Test()]
        public void NetworkMessageMessageBuild() {
            string message = ":m@m.net PRIVMSG #mychannel :Hiya, buddy.";
            NetworkMessage networkMessage = new NetworkMessage(message, true);
            Assert.AreEqual(message + "\r\n", networkMessage.BuildMessage());
        }
    }
}
