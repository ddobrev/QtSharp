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
            // TODO: Add Init code.
        }

        [TearDown]
        public void Dispose()
        {
            // TODO: Add tear down code.
        }

        [Test]
        public void TestEmptyConstructorNotThrowingAnException()
        {
            var i = new QAtomicInteger();
        }
    }
}