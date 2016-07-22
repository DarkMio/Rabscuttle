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

namespace Rabscuttle.networking.handler.Tests {
    [TestFixture()]
    public class ChannelHandlerTests {
        [Test()]
        public void ChannelHandlerSearchTest() {
            /*
            HashSet<ChannelUser> users = new HashSet<ChannelUser>();
            var created = new ChannelUser("Rabscootle", true);
            users.Add(created);
            Assert.AreSame(created, users.SingleOrDefault(s => s.userName == "Rabscootle"));
            var newCreated = new ChannelUser("Rabscootle", true);
            Assert.AreEqual(newCreated, users.SingleOrDefault(s => s.userName == "Rabscootle"));
            users.Add(newCreated);
            Assert.AreNotSame(newCreated, users.SingleOrDefault(s => s.userName == "Rabscootle"));
            Assert.AreSame(created, users.SingleOrDefault(s => s.userName == "Rabscootle"));
            */
            ChannelHandler co = new ChannelHandler(new ConnectionManager("localhost", 6667));
            var x = GetMethod("FindOrCreateUser", co);
            var created = x.Invoke(co, new object[] {"Rabscuttle", false});
            // var retrieved = x.Invoke(co, new object[] {"Rabscuttle", false});
            // Assert.AreEqual(created, retrieved);
            // Assert.AreSame(created, retrieved);
            Console.WriteLine(created);
            ChannelUser cu = new ChannelUser("Rabscootle", true);
            var get = co.users.SingleOrDefault(s => s == cu);
            var arr = co.users.ToArray();
            string y = "";
            foreach (ChannelUser channelUser in arr) {
                y += " " + channelUser.userName;
            }

            Assert.AreSame(created, get, y);

        }

        private MethodInfo GetMethod(string methodName, Object o) {
            if (string.IsNullOrWhiteSpace(methodName))
                Assert.Fail("methodName cannot be null or whitespace");

            var method = o.GetType()
                .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
                Assert.Fail($"{methodName} method not found");

            return method;
         }
    }
}
