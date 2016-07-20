using NUnit.Framework;
using Rabscuttle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabscuttle.Tests {
    [TestFixture()]
    public class RabscuttleTests {
        [Test()]
        public void addTest() {
            Assert.IsTrue(2 == Rabscuttle.Bot.Add(1, 1));
        }
    }
}