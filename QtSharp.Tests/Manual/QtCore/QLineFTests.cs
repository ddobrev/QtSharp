using NUnit.Framework;

namespace QtSharp.Tests.Manual.QtCore
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
            throw new AssertionException("Not implemented!");
            //var s = new QLineF();
        }

        [Test]
        public void TestPointsConstructor()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLineF(new QPointF(5.0, 5.0), new QPointF(10.0, 10.0));

            //Assert.AreEqual(5.0, s.X1);
            //Assert.AreEqual(5.0, s.Y1);

            //Assert.AreEqual(10.0, s.X2);
            //Assert.AreEqual(10.0, s.Y2);
        }

        [Test]
        public void TestIntConstructor()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLineF(5.0, 5.0, 10.0, 10.0);

            //Assert.AreEqual(5.0, s.X1);
            //Assert.AreEqual(5.0, s.Y1);

            //Assert.AreEqual(10.0, s.X2);
            //Assert.AreEqual(10.0, s.Y2);
        }

        [Test]
        public void TestPoints()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLineF(new QPointF(5.0, 5.0), new QPointF(10.0, 10.0));

            //Assert.AreEqual(5.0, s.P1.X);
            //Assert.AreEqual(5.0, s.P1.Y);

            //Assert.AreEqual(10.0, s.P2.X);
            //Assert.AreEqual(10.0, s.P2.Y);
        }

        [Test]
        public void Test_X1_Y1_X2_Y2()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLineF(5.0, 5.0, 10.0, 10.0);

            //Assert.AreEqual(5.0, s.X1);
            //Assert.AreEqual(5.0, s.Y1);

            //Assert.AreEqual(10.0, s.X2);
            //Assert.AreEqual(10.0, s.Y2);
        }

        [Test]
        public void Test_Dx_Dy()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLineF(5.0, 5.0, 10.0, 10.0);

            //Assert.AreEqual(5.0, s.Dx);
            //Assert.AreEqual(5.0, s.Dy);
        }

        [Test]
        public void TestIsNull()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLineF(5.0, 5.0, 10.0, 10.0);

            //Assert.IsFalse(s.IsNull);
        }

        [Test]
        public void TestSetLine()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLineF();
            //s.SetLine(5.0, 5.0, 10.0, 10.0);

            //Assert.AreEqual(5.0, s.X1);
            //Assert.AreEqual(5.0, s.Y1);

            //Assert.AreEqual(10.0, s.X2);
            //Assert.AreEqual(10.0, s.Y2);
        }

        [Test]
        public void TestSetPoints()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLineF();
            //var p1 = new QPointF(5.0, 5.0);
            //var p2 = new QPointF(10.0, 10.0);
            //s.SetPoints(p1, p2);

            //Assert.AreEqual(5.0, s.P1.X);
            //Assert.AreEqual(5.0, s.P1.Y);

            //Assert.AreEqual(10.0, s.P2.X);
            //Assert.AreEqual(10.0, s.P2.Y);
        }

        [Test]
        public void TestTranslateWithPoint()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLineF(5.0, 5.0, 10.0, 10.0);
            //s.Translate(new QPointF(5.0, 5.0));

            //Assert.AreEqual(10.0, s.X1);
            //Assert.AreEqual(10.0, s.Y1);

            //Assert.AreEqual(15.0, s.X2);
            //Assert.AreEqual(15.0, s.Y2);
        }

        [Test]
        public void TestTranslateWithInt()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLineF(5.0, 5.0, 10.0, 10.0);
            //s.Translate(5, 5);

            //Assert.AreEqual(10.0, s.X);
            //Assert.AreEqual(10.0, s.Y);

            //Assert.AreEqual(15.0, s.X);
            //Assert.AreEqual(15.0, s.Y);
        }

        [Test]
        public void TestTranslatedWithPoint()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLineF(5.0, 5.0, 10.0, 10.0);
            //var res = s.Translated(new QPointF(5.0, 5.0));

            //Assert.AreEqual(10.0, res.X1);
            //Assert.AreEqual(10.0, res.Y1);

            //Assert.AreEqual(15.0, res.X2);
            //Assert.AreEqual(15.0, res.Y2);
        }

        [Test]
        public void TestTranslatedWithInt()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLineF(5.0, 5.0, 10.0, 10.0);
            //var res = s.Translated(5.0, 5.0);

            //Assert.AreEqual(10.0, res.X);
            //Assert.AreEqual(10.0, res.Y);

            //Assert.AreEqual(15.0, res.X);
            //Assert.AreEqual(15.0, res.Y);
        }

        [Test]
        public void TestNotEqualOperator()
        {
            throw new AssertionException("Not implemented!");
            //var s1 = new QLineF(5.0, 5.0, 10.0, 10.0);
            //var s2 = new QLineF(5.0, 5.0, 15.0, 10.0);

            //Assert.AreNotEqual(s1, s2);           
        }

        [Test]
        public void TestEqualOperator()
        {
            throw new AssertionException("Not implemented!");
            //var s1 = new QLineF(5.0, 5.0, 10.0, 10.0);
            //var s2 = new QLineF(5.0, 5.0, 10.0, 10.0);

            //Assert.AreEqual(s1, s2);   
        }
    }
}