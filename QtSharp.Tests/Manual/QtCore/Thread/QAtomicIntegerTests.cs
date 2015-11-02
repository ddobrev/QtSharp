using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore.Thread
{
    [TestFixture]
    public class QAtomicIntegerTests
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
        public void TestEmptyConstructorNotThrowingAnException()
        {
            new QAtomicInteger();
        }
    }
}