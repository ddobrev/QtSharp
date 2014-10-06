using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore.Tools
{
    [TestFixture]
    public class QPointTests
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
            var s = new QPoint();

            Assert.AreEqual(0, s.X);
            Assert.AreEqual(0, s.Y);
        }

        [Test]
        public void TestIntegerConstructor()
        {
            var s = new QPoint(5, 10);

            Assert.AreEqual(5, s.X);
            Assert.AreEqual(10, s.Y);
        }

        [Test]
        public void TestDotProduct()
        {
            var s1 = new QPoint(3, 7);
            var s2 = new QPoint(-1, 4);
            var dot = QPoint.DotProduct(s1, s2);

            Assert.AreEqual(25, dot);
        }

        [Test]
        public void TestIsNull()
        {
            var s = new QPoint();

            Assert.IsTrue(s.IsNull);
        }

        [Test]
        public void TestManhattanLength()
        {
            var s1 = new QPoint(3, 7);
            var res = s1.ManhattanLength;

            Assert.AreEqual(10, res);
        }

        [Test]
        public unsafe void TestRx()
        {
            var s1 = new QPoint(3, 7);

            int* res = s1.Rx;

            Assert.AreEqual(3, *res);
        }

        [Test]
        public unsafe void TestRy()
        {
            var s1 = new QPoint(3, 7);

            int* res = s1.Ry;

            Assert.AreEqual(7, *res);
        }

        [Test]
        public void TestX()
        {
            var s1 = new QPoint(3, 7);

            var res = s1.X;

            Assert.AreEqual(3, res);
        }

        [Test]
        public void TestY()
        {
            var s1 = new QPoint(3, 7);

            var res = s1.Y;

            Assert.AreEqual(7, res);
        }

        [Test]
        public void TestMultiEqualIntOperator()
        {
            var s1 = new QPoint(3, 7);

            throw new AssertionException("Not implemented!");
            //s1 *= 5;

            Assert.AreEqual(15, s1.X);
            Assert.AreEqual(35, s1.Y);
        }

        [Test]
        public void TestMultiEqualFloatOperator()
        {
            var s1 = new QPoint(3, 7);

            throw new AssertionException("Not implemented!");
            //s1 *= 5.0f;

            Assert.AreEqual(15, s1.X);
            Assert.AreEqual(35, s1.Y);
        }

        [Test]
        public void TestMultiEqualDoubleOperator()
        {
            var s1 = new QPoint(3, 7);

            throw new AssertionException("Not implemented!");
            //s1 *= 5.0;

            Assert.AreEqual(15, s1.X);
            Assert.AreEqual(35, s1.Y);
        }

        [Test]
        public void TestAddEqualOperator()
        {
            var s1 = new QPoint(3, 7);
            var s2 = new QPoint(-1, 4);

            throw new AssertionException("Not implemented!");
            //s1 += s2;

            Assert.AreEqual(2, s1.X);
            Assert.AreEqual(11, s1.Y);
        }

        [Test]
        public void TestSubEqualOperator()
        {
            var s1 = new QPoint(3, 7);
            var s2 = new QPoint(-1, 4);

            throw new AssertionException("Not implemented!");
            //s1 -= s2;

            Assert.AreEqual(4, s1.X);
            Assert.AreEqual(3, s1.Y);
        }

        [Test]
        public void TestDivEqualOperator()
        {
            var s1 = new QPoint(-3, 10);

            throw new AssertionException("Not implemented!");
            //s1 /= 2.5;

            Assert.AreEqual(-1, s1.X);
            Assert.AreEqual(4, s1.Y);
        }

        [Test]
        public void TestNotEqualOperator()
        {
            var s1 = new QPoint(3, 7);
            var s2 = new QPoint(-1, 4);

            throw new AssertionException("Not implemented!");

            Assert.AreNotEqual(s1, s2);
        }

        [Test]
        public void TestMultFloatOperator()
        {
            var s1 = new QPoint(3, 7);

            throw new AssertionException("Not implemented!");
            //var res = s1 * 5.0f;

            //Assert.AreEqual(15, res.X);
            //Assert.AreEqual(35, res.Y);
        }

        [Test]
        public void TestMultDoubleOperator()
        {
            var s1 = new QPoint(3, 7);

            throw new AssertionException("Not implemented!");
            //var res = s1 * 5.0;

            //Assert.AreEqual(15, res.X);
            //Assert.AreEqual(35, res.Y);
        }

        [Test]
        public void TestMultIntOperator()
        {
            var s1 = new QPoint(3, 7);

            throw new AssertionException("Not implemented!");
            //var res = s1 * 5;

            //Assert.AreEqual(15, res.X);
            //Assert.AreEqual(35, res.Y);
        }

        [Test]
        public void TestMultFloatRevOperator()
        {
            var s1 = new QPoint(3, 7);

            throw new AssertionException("Not implemented!");
            //var res = 5.0f * s1;

            //Assert.AreEqual(15, res.X);
            //Assert.AreEqual(35, res.Y);
        }

        [Test]
        public void TestMultDoubleRevOperator()
        {
            var s1 = new QPoint(3, 7);

            throw new AssertionException("Not implemented!");
            //var res =  5.0 * s1;

            //Assert.AreEqual(15, res.X);
            //Assert.AreEqual(35, res.Y);
        }

        [Test]
        public void TestMultIntRevOperator()
        {
            var s1 = new QPoint(3, 7);

            throw new AssertionException("Not implemented!");
            //var res = 5 * s1;

            //Assert.AreEqual(15, res.X);
            //Assert.AreEqual(35, res.Y);
        }

        [Test]
        public void TestAddQPointsOperator()
        {
            var s1 = new QPoint(3, 7);
            var s2 = new QPoint(-1, 4);

            throw new AssertionException("Not implemented!");
            //var res = s1 + s2;

            //Assert.AreEqual(2, res.X);
            //Assert.AreEqual(11, res.Y);
        }

        [Test]
        public void TestAddQPointOperator()
        {
            var s1 = new QPoint(3, 7);

            throw new AssertionException("Not implemented!");
            //var res = +s1;

            //Assert.AreEqual(3, res.X);
            //Assert.AreEqual(7, res.Y);
        }

        [Test]
        public void TestSubQPointsOperator()
        {
            var s1 = new QPoint(3, 7);
            var s2 = new QPoint(-1, 4);

            throw new AssertionException("Not implemented!");
            //var res = s1 - s2;

            //Assert.AreEqual(4, res.X);
            //Assert.AreEqual(3, res.Y);
        }

        [Test]
        public void TestSubQPointOperator()
        {
            var s1 = new QPoint(3, 7);

            throw new AssertionException("Not implemented!");
            //var res = -s1;

            //Assert.AreEqual(-3, res.X);
            //Assert.AreEqual(-7, res.Y);
        }

        [Test]
        public void TestDivOperator()
        {
            var s1 = new QPoint(3, 7);

            throw new AssertionException("Not implemented!");
            //var res = s1 / 1.0;

            //Assert.AreEqual(3, res.X);
            //Assert.AreEqual(7, res.Y);
        }

        [Test]
        public void TestEqualOperator()
        {
            var s1 = new QPoint(3, 7);
            var s2 = new QPoint(3, 7);

            throw new AssertionException("Not implemented!");

            Assert.AreEqual(s1, s2);
        }
    }
}