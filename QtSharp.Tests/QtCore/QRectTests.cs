using System.Data;
using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.QtCore
{
    [TestFixture]
    public class QRectTests
    {
        private QRect _qRect;

        [SetUp]
        public void Init()
        {
            // TODO: Add Init code.
            _qRect = new QRect();
        }

        [TearDown]
        public void Dispose()
        {
            // TODO: Add tear down code.
        }

        #region Ctor
        [Test]
        public void TestEmptyConstructor()
        {
            var s = new QRect();
            Assert.NotNull(s.IsValid);
        }

        [Test]
        public void TestIntegerConstructor()
        {
            var s = new QRect(50, 100, 200, 150);

            Assert.AreEqual(50, s.Left);
            Assert.AreEqual(100, s.Top);
            Assert.AreEqual(200, s.Width);
            Assert.AreEqual(150, s.Height);
        }

        [Test]
        public void TestPointsConstructor()
        {
            var p1 = new QPoint(50, 100);
            var p2 = new QPoint(250, 250);

            var s = new QRect(p1, p2);

            Assert.AreEqual(50, s.Left);
            Assert.AreEqual(100, s.Top);
            Assert.AreEqual(200, s.Width);
            Assert.AreEqual(150, s.Height);
        }

        [Test]
        public void TestPointSizeConstructor()
        {
            var p1 = new QPoint(50, 100);
            var p2 = new QSize(200, 150);

            var s = new QRect(p1, p2);

            Assert.AreEqual(50, s.Left);
            Assert.AreEqual(100, s.Top);
            Assert.AreEqual(200, s.Width);
            Assert.AreEqual(150, s.Height);
        }
        #endregion

        // TODO Add members
    }
}