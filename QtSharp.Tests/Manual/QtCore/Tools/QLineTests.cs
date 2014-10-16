using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore.Tools
{
    [TestFixture]
    public class QLineTests
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
            var s = new QLine();
        }

        [Test]
        public void TestPointsConstructor()
        {
            var s = new QLine(new QPoint(5, 5), new QPoint(10, 10));

            Assert.AreEqual(5, s.X1);
            Assert.AreEqual(5, s.Y1);

            Assert.AreEqual(10, s.X2);
            Assert.AreEqual(10, s.Y2);
        }

        [Test]
        public void TestIntConstructor()
        {
            var s = new QLine(5, 5, 10, 10);

            Assert.AreEqual(5, s.X1);
            Assert.AreEqual(5, s.Y1);

            Assert.AreEqual(10, s.X2);
            Assert.AreEqual(10, s.Y2);
        }

        [Test]
        public void TestPoints()
        {
            var s = new QLine(new QPoint(5, 5), new QPoint(10, 10));

            Assert.AreEqual(5, s.P1.X);
            Assert.AreEqual(5, s.P1.Y);

            Assert.AreEqual(10, s.P2.X);
            Assert.AreEqual(10, s.P2.Y);
        }

        [Test]
        public void Test_X1_Y1_X2_Y2()
        {
            var s = new QLine(5, 5, 10, 10);

            Assert.AreEqual(5, s.X1);
            Assert.AreEqual(5, s.Y1);

            Assert.AreEqual(10, s.X2);
            Assert.AreEqual(10, s.Y2);
        }

        [Test]
        public void Test_Dx_Dy()
        {
            var s = new QLine(5, 5, 10, 10);

            Assert.AreEqual(5, s.Dx);
            Assert.AreEqual(5, s.Dy);
        }

        [Test]
        public void TestIsNull()
        {
            var s = new QLine(5, 5, 10, 10);

            Assert.IsFalse(s.IsNull);
        }

        [Test]
        public void TestSetLine()
        {
            var s = new QLine();
            s.SetLine(5, 5, 10, 10);

            Assert.AreEqual(5, s.X1);
            Assert.AreEqual(5, s.Y1);

            Assert.AreEqual(10, s.X2);
            Assert.AreEqual(10, s.Y2);
        }

        [Test]
        public void TestSetPoints()
        {
            var s = new QLine();
            var p1 = new QPoint(5, 5);
            var p2 = new QPoint(10, 10);
            s.SetPoints(p1, p2);

            Assert.AreEqual(5, s.P1.X);
            Assert.AreEqual(5, s.P1.Y);

            Assert.AreEqual(10, s.P2.X);
            Assert.AreEqual(10, s.P2.Y);
        }

        [Test]
        public void TestTranslateWithPoint()
        {
            var s = new QLine(5, 5, 10, 10);
            s.Translate(new QPoint(5, 5));

            Assert.AreEqual(10, s.X1);
            Assert.AreEqual(10, s.Y1);

            Assert.AreEqual(15, s.X2);
            Assert.AreEqual(15, s.Y2);
        }

        [Test]
        public void TestTranslateWithInt()
        {
            var s = new QLine(5, 5, 10, 10);
            s.Translate(5, 5);

            Assert.AreEqual(10, s.X1);
            Assert.AreEqual(10, s.Y1);

            Assert.AreEqual(15, s.X2);
            Assert.AreEqual(15, s.Y2);
        }

        [Test]
        public void TestTranslatedWithPoint()
        {
            var s = new QLine(5, 5, 10, 10);
            var res = s.Translated(new QPoint(5, 5));

            Assert.AreEqual(10, res.X1);
            Assert.AreEqual(10, res.Y1);

            Assert.AreEqual(15, res.X2);
            Assert.AreEqual(15, res.Y2);
        }

        [Test]
        public void TestTranslatedWithInt()
        {
            var s = new QLine(5, 5, 10, 10);
            var res = s.Translated(5, 5);

            Assert.AreEqual(10, res.X1);
            Assert.AreEqual(10, res.Y1);

            Assert.AreEqual(15, res.X2);
            Assert.AreEqual(15, res.Y2);
        }

        [Test]
        public void TestNotEqualOperator()
        {
            var s1 = new QLine(5, 5, 10, 10);
            var s2 = new QLine(5, 5, 15, 10);

            Assert.AreNotEqual(s1, s2);
        }

        [Test]
        public void TestEqualOperator()
        {
            var s1 = new QLine(5, 5, 10, 10);
            var s2 = new QLine(5, 5, 10, 10);

            Assert.AreEqual(s1, s2);
        }
    }
}