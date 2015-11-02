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
        }

        [TearDown]
        public void Dispose()
        {
        }

        [Test]
        public void TestEmptyConstructor()
        {
            using (new QLineF())
            {
            }
        }

        [Test]
        public void TestPointsConstructor()
        {
            using (var s = new QLineF(new QPointF(5.0, 5.0), new QPointF(10.0, 10.0)))
            {
                Assert.AreEqual(5.0, s.X1);
                Assert.AreEqual(5.0, s.Y1);

                Assert.AreEqual(10.0, s.X2);
                Assert.AreEqual(10.0, s.Y2);
            }
        }

        [Test]
        public void TestIntConstructor()
        {
            using (var s = new QLineF(5.0, 5.0, 10.0, 10.0))
            {
                Assert.AreEqual(5.0, s.X1);
                Assert.AreEqual(5.0, s.Y1);

                Assert.AreEqual(10.0, s.X2);
                Assert.AreEqual(10.0, s.Y2);
            }
        }

        [Test]
        public void TestPoints()
        {
            using (var s = new QLineF(new QPointF(5.0, 5.0), new QPointF(10.0, 10.0)))
            {
                Assert.AreEqual(5.0, s.P1.X);
                Assert.AreEqual(5.0, s.P1.Y);

                Assert.AreEqual(10.0, s.P2.X);
                Assert.AreEqual(10.0, s.P2.Y);
            }
        }

        [Test]
        public void Test_X1_Y1_X2_Y2()
        {
            using (var s = new QLineF(5.0, 5.0, 10.0, 10.0))
            {
                Assert.AreEqual(5.0, s.X1);
                Assert.AreEqual(5.0, s.Y1);

                Assert.AreEqual(10.0, s.X2);
                Assert.AreEqual(10.0, s.Y2);
            }
        }

        [Test]
        public void Test_Dx_Dy()
        {
            using (var s = new QLineF(5.0, 5.0, 10.0, 10.0))
            {
                Assert.AreEqual(5.0, s.Dx);
                Assert.AreEqual(5.0, s.Dy);
            }
        }

        [Test]
        public void TestIsNull()
        {
            using (var s = new QLineF(5.0, 5.0, 10.0, 10.0))
            {
                Assert.IsFalse(s.IsNull);
            }
        }

        [Test]
        public void TestSetLine()
        {
            using (var s = new QLineF())
            {
                s.SetLine(5.0, 5.0, 10.0, 10.0);

                Assert.AreEqual(5.0, s.X1);
                Assert.AreEqual(5.0, s.Y1);

                Assert.AreEqual(10.0, s.X2);
                Assert.AreEqual(10.0, s.Y2);
            }
        }

        [Test]
        public void TestSetPoints()
        {
            using (var s = new QLineF())
            {
                var p1 = new QPointF(5.0, 5.0);
                var p2 = new QPointF(10.0, 10.0);
                s.SetPoints(p1, p2);

                Assert.AreEqual(5.0, s.P1.X);
                Assert.AreEqual(5.0, s.P1.Y);

                Assert.AreEqual(10.0, s.P2.X);
                Assert.AreEqual(10.0, s.P2.Y);
            }
        }

        [Test]
        public void TestTranslateWithPoint()
        {
            using (var s = new QLineF(5.0, 5.0, 10.0, 10.0))
            {
                s.Translate(new QPointF(5.0, 5.0));

                Assert.AreEqual(10.0, s.X1);
                Assert.AreEqual(10.0, s.Y1);

                Assert.AreEqual(15.0, s.X2);
                Assert.AreEqual(15.0, s.Y2);
            }
        }

        [Test]
        public void TestTranslateWithInt()
        {
            using (var s = new QLineF(5.0, 5.0, 10.0, 10.0))
            {
                s.Translate(5, 5);

                Assert.AreEqual(10.0, s.X1);
                Assert.AreEqual(10.0, s.Y1);

                Assert.AreEqual(15.0, s.X2);
                Assert.AreEqual(15.0, s.Y2);
            }
        }

        [Test]
        public void TestTranslatedWithPoint()
        {
            QLineF res;
            using (var s = new QLineF(5.0, 5.0, 10.0, 10.0))
            {
                res = s.Translated(new QPointF(5.0, 5.0));
            }

            Assert.AreEqual(10.0, res.X1);
            Assert.AreEqual(10.0, res.Y1);

            Assert.AreEqual(15.0, res.X2);
            Assert.AreEqual(15.0, res.Y2);
        }

        [Test]
        public void TestTranslatedWithInt()
        {
            QLineF res;
            using (var s = new QLineF(5.0, 5.0, 10.0, 10.0))
            {
                res = s.Translated(5.0, 5.0);
            }

            Assert.AreEqual(10.0, res.X1);
            Assert.AreEqual(10.0, res.Y1);

            Assert.AreEqual(15.0, res.X2);
            Assert.AreEqual(15.0, res.Y2);
        }

        [Test]
        public void TestNotEqualOperator()
        {
            using (var s1 = new QLineF(5.0, 5.0, 10.0, 10.0))
            {
                using (var s2 = new QLineF(5.0, 5.0, 15.0, 10.0))
                {
                    Assert.AreNotEqual(s1, s2);
                }
            }
        }

        [Test]
        public void TestEqualOperator()
        {
            using (var s1 = new QLineF(5.0, 5.0, 10.0, 10.0))
            {
                using (var s2 = new QLineF(5.0, 5.0, 10.0, 10.0))
                {
                    Assert.AreEqual(s1, s2);
                }
            }
        }
    }
}