using NUnit.Framework;

namespace QtSharp.Tests.Manual.QtCore
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
            throw new AssertionException("Not implemented!");
            //var s = new QLine();
        }

        [Test]
        public void TestPointsConstructor()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLine(new QPoint(5, 5), new QPoint(10, 10));

            //Assert.AreEqual(5, s.X1);
            //Assert.AreEqual(5, s.Y1);

            //Assert.AreEqual(10, s.X2);
            //Assert.AreEqual(10, s.Y2);
        }

        [Test]
        public void TestIntConstructor()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLine(5, 5, 10, 10);

            //Assert.AreEqual(5, s.X1);
            //Assert.AreEqual(5, s.Y1);

            //Assert.AreEqual(10, s.X2);
            //Assert.AreEqual(10, s.Y2);
        }

        [Test]
        public void TestPoints()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLine(new QPoint(5, 5), new QPoint(10, 10));

            //Assert.AreEqual(5, s.P1.X);
            //Assert.AreEqual(5, s.P1.Y);

            //Assert.AreEqual(10, s.P2.X);
            //Assert.AreEqual(10, s.P2.Y);
        }

        [Test]
        public void Test_X1_Y1_X2_Y2()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLine(5, 5, 10, 10);

            //Assert.AreEqual(5, s.X1);
            //Assert.AreEqual(5, s.Y1);

            //Assert.AreEqual(10, s.X2);
            //Assert.AreEqual(10, s.Y2);
        }

        [Test]
        public void Test_Dx_Dy()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLine(5, 5, 10, 10);

            //Assert.AreEqual(5, s.Dx);
            //Assert.AreEqual(5, s.Dy);
        }

        [Test]
        public void TestIsNull()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLine(5, 5, 10, 10);

            //Assert.IsFalse(s.IsNull);
        }

        [Test]
        public void TestSetLine()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLine();
            //s.SetLine(5, 5, 10, 10);

            //Assert.AreEqual(5, s.X1);
            //Assert.AreEqual(5, s.Y1);

            //Assert.AreEqual(10, s.X2);
            //Assert.AreEqual(10, s.Y2);
        }

        [Test]
        public void TestSetPoints()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLine();
            //var p1 = new QPoint(5, 5);
            //var p2 = new QPoint(10, 10);
            //s.SetPoints(p1, p2);

            //Assert.AreEqual(5, s.P1.X);
            //Assert.AreEqual(5, s.P1.Y);

            //Assert.AreEqual(10, s.P2.X);
            //Assert.AreEqual(10, s.P2.Y);
        }

        [Test]
        public void TestTranslateWithPoint()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLine(5, 5, 10, 10);
            //s.Translate(new QPoint(5, 5));

            //Assert.AreEqual(10, s.X1);
            //Assert.AreEqual(10, s.Y1);

            //Assert.AreEqual(15, s.X2);
            //Assert.AreEqual(15, s.Y2);
        }

        [Test]
        public void TestTranslateWithInt()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLine(5, 5, 10, 10);
            //s.Translate(5, 5);

            //Assert.AreEqual(10, s.X);
            //Assert.AreEqual(10, s.Y);

            //Assert.AreEqual(15, s.X);
            //Assert.AreEqual(15, s.Y);
        }

        [Test]
        public void TestTranslatedWithPoint()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLine(5, 5, 10, 10);
            //var res = s.Translated(new QPoint(5, 5));

            //Assert.AreEqual(10, res.X1);
            //Assert.AreEqual(10, res.Y1);

            //Assert.AreEqual(15, res.X2);
            //Assert.AreEqual(15, res.Y2);
        }

        [Test]
        public void TestTranslatedWithInt()
        {
            throw new AssertionException("Not implemented!");
            //var s = new QLine(5, 5, 10, 10);
            //var res = s.Translated(5, 5);

            //Assert.AreEqual(10, res.X);
            //Assert.AreEqual(10, res.Y);

            //Assert.AreEqual(15, res.X);
            //Assert.AreEqual(15, res.Y);
        }

        [Test]
        public void TestNotEqualOperator()
        {
            throw new AssertionException("Not implemented!");
            //var s1 = new QLine(5, 5, 10, 10);
            //var s2 = new QLine(5, 5, 15, 10);

            //Assert.AreNotEqual(s1, s2);  
        }

        [Test]
        public void TestEqualOperator()
        {
            throw new AssertionException("Not implemented!");
            //var s1 = new QLine(5, 5, 10, 10);
            //var s2 = new QLine(5, 5, 10, 10);

            //Assert.AreEqual(s1, s2);   
        }
    }
}