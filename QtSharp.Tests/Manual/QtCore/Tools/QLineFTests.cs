using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore.Tools
{
    [TestFixture]
    public class QLineFTests
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
            var s = new QLineF();
        }

        [Test]
        public void TestPointsConstructor()
        {
            var s = new QLineF(new QPointF(5.0, 5.0), new QPointF(10.0, 10.0));

            Assert.AreEqual(5.0, s.X1);
            Assert.AreEqual(5.0, s.Y1);

            Assert.AreEqual(10.0, s.X2);
            Assert.AreEqual(10.0, s.Y2);
        }

        [Test]
        public void TestIntConstructor()
        {
            var s = new QLineF(5.0, 5.0, 10.0, 10.0);

            Assert.AreEqual(5.0, s.X1);
            Assert.AreEqual(5.0, s.Y1);

            Assert.AreEqual(10.0, s.X2);
            Assert.AreEqual(10.0, s.Y2);
        }

        [Test]
        public void TestPoints()
        {
            var s = new QLineF(new QPointF(5.0, 5.0), new QPointF(10.0, 10.0));

            Assert.AreEqual(5.0, s.P1.X);
            Assert.AreEqual(5.0, s.P1.Y);

            Assert.AreEqual(10.0, s.P2.X);
            Assert.AreEqual(10.0, s.P2.Y);
        }

        [Test]
        public void Test_X1_Y1_X2_Y2()
        {
            var s = new QLineF(5.0, 5.0, 10.0, 10.0);

            Assert.AreEqual(5.0, s.X1);
            Assert.AreEqual(5.0, s.Y1);

            Assert.AreEqual(10.0, s.X2);
            Assert.AreEqual(10.0, s.Y2);
        }

        [Test]
        public void Test_Dx_Dy()
        {
            var s = new QLineF(5.0, 5.0, 10.0, 10.0);

            Assert.AreEqual(5.0, s.Dx);
            Assert.AreEqual(5.0, s.Dy);
        }

        [Test]
        public void TestIsNull()
        {
            var s = new QLineF(5.0, 5.0, 10.0, 10.0);

            Assert.IsFalse(s.IsNull);
        }

        [Test]
        public void TestSetLine()
        {
            var s = new QLineF();
            s.SetLine(5.0, 5.0, 10.0, 10.0);

            Assert.AreEqual(5.0, s.X1);
            Assert.AreEqual(5.0, s.Y1);

            Assert.AreEqual(10.0, s.X2);
            Assert.AreEqual(10.0, s.Y2);
        }

        [Test]
        public void TestSetPoints()
        {
            var s = new QLineF();
            var p1 = new QPointF(5.0, 5.0);
            var p2 = new QPointF(10.0, 10.0);
            s.SetPoints(p1, p2);

            Assert.AreEqual(5.0, s.P1.X);
            Assert.AreEqual(5.0, s.P1.Y);

            Assert.AreEqual(10.0, s.P2.X);
            Assert.AreEqual(10.0, s.P2.Y);
        }

        [Test]
        public void TestTranslateWithPoint()
        {
            var s = new QLineF(5.0, 5.0, 10.0, 10.0);
            s.Translate(new QPointF(5.0, 5.0));

            Assert.AreEqual(10.0, s.X1);
            Assert.AreEqual(10.0, s.Y1);

            Assert.AreEqual(15.0, s.X2);
            Assert.AreEqual(15.0, s.Y2);
        }

        [Test]
        public void TestTranslateWithInt()
        {
            var s = new QLineF(5.0, 5.0, 10.0, 10.0);
            s.Translate(5, 5);

            Assert.AreEqual(10.0, s.X1);
            Assert.AreEqual(10.0, s.Y1);

            Assert.AreEqual(15.0, s.X2);
            Assert.AreEqual(15.0, s.Y2);
        }

        [Test]
        public void TestTranslatedWithPoint()
        {
            var s = new QLineF(5.0, 5.0, 10.0, 10.0);
            var res = s.Translated(new QPointF(5.0, 5.0));

            Assert.AreEqual(10.0, res.X1);
            Assert.AreEqual(10.0, res.Y1);

            Assert.AreEqual(15.0, res.X2);
            Assert.AreEqual(15.0, res.Y2);
        }

        [Test]
        public void TestTranslatedWithInt()
        {
            var s = new QLineF(5.0, 5.0, 10.0, 10.0);
            var res = s.Translated(5.0, 5.0);

            Assert.AreEqual(10.0, res.X1);
            Assert.AreEqual(10.0, res.Y1);

            Assert.AreEqual(15.0, res.X2);
            Assert.AreEqual(15.0, res.Y2);
        }

        [Test]
        public void TestNotEqualOperator()
        {
            var s1 = new QLineF(5.0, 5.0, 10.0, 10.0);
            var s2 = new QLineF(5.0, 5.0, 15.0, 10.0);

            Assert.AreNotEqual(s1, s2);
        }

        [Test]
        public void TestEqualOperator()
        {
            var s1 = new QLineF(5.0, 5.0, 10.0, 10.0);
            var s2 = new QLineF(5.0, 5.0, 10.0, 10.0);

            Assert.AreEqual(s1, s2);
        }
    }
}