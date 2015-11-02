using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore
{
    [TestFixture]
    public class QBasicTimerTests
    {
        [SetUp]
        public void Init()
        {
        }

        [TearDown]
        public void Dispose()
        {
        }

        [Test]
        public void TestEmptyConstructor()
        {
            using (var t = new QBasicTimer())
            {
                Assert.NotNull(t);
            }
        }

        [Test]
        public void TestIsActive()
        {
            using (var t = new QBasicTimer())
            {
                Assert.IsFalse(t.IsActive);
                using (var qObject = new QObject())
                {
                    t.Start(1, qObject);
                    Assert.IsTrue(t.IsActive);
                    t.Stop();
                    Assert.IsFalse(t.IsActive);
                }
            }
        }
    }
}