using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.QtCore
{
    [TestFixture]
    public class QFlagTests
    {
        [Test]
        public void TestInt()
        {
            int d = -5;
            var s = new QFlag(d);

            int i = s;

            Assert.AreEqual(d, i);
        }

        [Test]
        public void TestUInt()
        {
            uint d = 5;
            var s = new QFlag(d);

            int i = s;
            uint ui = s;

            Assert.AreEqual(d, i);
            Assert.AreEqual(d, ui);
        }

        [Test]
        public void TestShort()
        {
            short d = -5;
            var s = new QFlag(d);

            int i = s;

            Assert.AreEqual(d, i);
        }

        [Test]
        public void TestUShort()
        {
            ushort d = 5;
            var s = new QFlag(d);

            int i = s;
            uint ui = s;

            Assert.AreEqual(d, i);
            Assert.AreEqual(d, ui);
        }
    }
}