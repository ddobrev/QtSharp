using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.QtCore
{
    [TestFixture]
    public class QBasicTimerTests
    {
        [SetUp]
        public void Init()
        {
            // TODO: Add Init code.
        }

        [TearDown]
        public void Dispose()
        {
            // TODO: Add tear down code.
        }

        [Test]
        public void TestEmptyConstructor()
        {
            var t = new QBasicTimer();

            Assert.NotNull(t);
        }

        [Test]
        public void TestIsActive()
        {
            var t = new QBasicTimer();
            var isAc = t.IsActive;
        }
    }
}