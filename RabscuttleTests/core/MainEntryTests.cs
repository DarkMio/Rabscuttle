using NUnit.Framework;
using Rabscuttle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabscuttle.Tests {
    [TestFixture()]
    public class MainEntryTests {
        [Test()]
        public void TrueTest() {
            Assert.IsTrue(MainEntry.True());
        }
    }
}
