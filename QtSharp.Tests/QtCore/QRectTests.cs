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

        [Test]
        public void TestAdjust()
        {
            var s = new QRect();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            s.Adjust(50, 100, 150, 200);

            Assert.AreEqual(s.X, 100);
            Assert.AreEqual(s.Y, 250);
            Assert.AreEqual(s.Width, 650);
            Assert.AreEqual(s.Height, 800);
        }

        [Test]
        public void TestAdjusted()
        {
            var s = new QRect();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            var n = s.Adjusted(50, 100, 150, 200);

            Assert.AreEqual(n.X, 100);
            Assert.AreEqual(n.Y, 250);
            Assert.AreEqual(n.Width, 650);
            Assert.AreEqual(n.Height, 800);
        }

        [Test]
        public void TestBottom()
        {
            var s = new QRect();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            var n = s.Bottom;

            Assert.AreEqual(s.Top+s.Height-1, n);
        }

        [Test]
        public void TestBottomLeft()
        {
            var s = new QRect();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            var n = s.BottomLeft;

            Assert.AreEqual(s.Left + s.Top - 1, n);
        }

        [Test]
        public void TestBottomRight()
        {
            var s = new QRect();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            var n = s.BottomRight;

            Assert.AreEqual(s.Width + s.Left - 1, n);
        }

        [Test]
        public void TestCenter()
        {
            var s = new QRect();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            var n = s.Center;

            Assert.AreEqual(275, n.X);
            Assert.AreEqual(375, n.Y);
        }

        [Test]
        public void TestContainsWithQPoint()
        {
            var s = new QRect();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            var p = new QPoint(275, 375);
            var n = s.Contains(p);

            Assert.IsTrue(n);
        }

        [Test]
        public void TestContainsWithIntegerValues()
        {
            var s = new QRect();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            var n = s.Contains(275, 375);

            Assert.IsTrue(n);
        }

        [Test]
        public void TestContainsWithQRect()
        {
            var s = new QRect();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            var p = new QRect(275, 375, 50, 50);
            var n = s.Contains(p, true);

            Assert.IsTrue(n);
        }

        [Test]
        public unsafe void TestGetCoords()
        {
            var s = new QRect();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            int x1;
            int y1;
            int x2;
            int y2;

            s.GetCoords(&x1, &y1, &x2, &y2);

            Assert.AreEqual(50, x1);
            Assert.AreEqual(150, y1);
            Assert.AreEqual(550, x2);
            Assert.AreEqual(750, y2);
        }

        [Test]
        public unsafe void TestGetRect()
        {
            var s = new QRect();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            int x1;
            int y1;
            int width;
            int height;

            s.GetRect(&x1, &y1, &width, &height);

            Assert.AreEqual(50, x1);
            Assert.AreEqual(150, y1);
            Assert.AreEqual(500, width);
            Assert.AreEqual(600, height);
        }

        [Test]
        public void TestHeight()
        {
            var s = new QRect();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            Assert.AreEqual(600, s.Height);
        }

        [Test]
        public void TestIntersected()
        {
            var s1 = new QRect();
            s1.X = 0;
            s1.Y = 0;
            s1.Width = 500;
            s1.Height = 600;

            var s2 = new QRect();
            s2.X = 400;
            s2.Y = 500;
            s2.Width = 500;
            s2.Height = 600;

            var inter = s1.Intersected(s2);

            Assert.AreEqual(400, inter.X);
            Assert.AreEqual(500, inter.Y);
            Assert.AreEqual(100, inter.Width);
            Assert.AreEqual(100, inter.Height);
        }

        [Test]
        public void TestIntersects()
        {
            var s1 = new QRect();
            s1.X = 0;
            s1.Y = 0;
            s1.Width = 500;
            s1.Height = 600;

            var s2 = new QRect();
            s2.X = 400;
            s2.Y = 500;
            s2.Width = 500;
            s2.Height = 600;

            var inter = s1.Intersects(s2);

            Assert.IsTrue(inter);
        }

        [Test]
        public void TestIsEmpty()
        {
            var s1 = new QRect();
            s1.Left = 500;
            s1.Right = 400;
            s1.Top = 700;
            s1.Bottom = 600;

            var inter = s1.IsEmpty;

            Assert.IsTrue(inter);
        }

        [Test]
        public void TestIsNull()
        {
            var s1 = new QRect();
            s1.X = 0;
            s1.Y = 0;
            s1.Width = 0;
            s1.Height = 0;

            var inter = s1.IsNull;

            Assert.IsTrue(inter);
        }

        [Test]
        public void TestIsValid()
        {
            var s1 = new QRect();
            s1.Left = 500;
            s1.Right = 400;
            s1.Top = 700;
            s1.Bottom = 600;

            var inter = s1.IsValid;

            Assert.IsFalse(inter);
        }

        [Test]
        public void TestLeft()
        {
            var s = new QRect();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            Assert.AreEqual(50, s.Left);
        }

        [Test]
        public void TestMarginsAdded()
        {
            var s = new QRect();
            s.Left = 50;
            s.Top = 150;
            s.Right = 600;
            s.Bottom = 500;

            var mar = new QMargins(50, 100, 150, 200);

            var newR = s.MarginsAdded(mar);

            Assert.AreEqual(100, newR.Left);
            Assert.AreEqual(250, newR.Top);
            Assert.AreEqual(750, newR.Right);
            Assert.AreEqual(700, newR.Bottom);
        }

        [Test]
        public void TestMarginsRemoved()
        {
            var s = new QRect();
            s.Left = 50;
            s.Top = 150;
            s.Right = 600;
            s.Bottom = 500;

            var mar = new QMargins(50, 100, 150, 200);

            var newR = s.MarginsRemoved(mar);

            Assert.AreEqual(0, newR.Left);
            Assert.AreEqual(50, newR.Top);
            Assert.AreEqual(450, newR.Right);
            Assert.AreEqual(300, newR.Bottom);
        }

        [Test]
        public void TestMoveBottom()
        {
            var s = new QRect();
            s.Left = 50;
            s.Top = 150;
            s.Right = 600;
            s.Bottom = 500;

            s.MoveBottom(50);
     
            Assert.AreEqual(550, s.Bottom);
        }

        [Test]
        public void TestMoveBottomLeft()
        {
            var s = new QRect();
            s.X = 50;
            s.Y = 100;
            s.Width = 500;
            s.Height = 600;

            s.MoveBottomLeft(new QPoint(100, 600));

            Assert.AreEqual(500, s.Width);
            Assert.AreEqual(600, s.Height);
            Assert.AreEqual(100, s.X);
            Assert.AreEqual(0, s.Y);
        }

        [Test]
        public void TestMoveBottomRight()
        {
            var s = new QRect();
            s.X = 50;
            s.Y = 100;
            s.Width = 500;
            s.Height = 600;

            s.MoveBottomRight(new QPoint(600, 600));

            Assert.AreEqual(500, s.Width);
            Assert.AreEqual(600, s.Height);
            Assert.AreEqual(100, s.X);
            Assert.AreEqual(100, s.Y);
        }

        [Test]
        public void TestMoveCenter()
        {
            var s = new QRect();
            s.X = 50;
            s.Y = 100;
            s.Width = 500;
            s.Height = 600;

            s.MoveCenter(new QPoint(600, 600));

            Assert.AreEqual(500, s.Width);
            Assert.AreEqual(600, s.Height);
            Assert.AreEqual(600-250, s.X);
            Assert.AreEqual(600-300, s.Y);
        }

        // TODO Add moveLeft

        // TODO Add members
    }
}