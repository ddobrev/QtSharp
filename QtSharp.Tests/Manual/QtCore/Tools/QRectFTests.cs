using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore.Tools
{
    [TestFixture]
    public class QRectFTests
    {
        private QRectF _qRectF;

        [SetUp]
        public void Init()
        {
            // TODO: Add Init code.
            _qRectF = new QRectF();
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
            var s = new QRectF();
        }

        [Test]
        public void TestIntegerConstructor()
        {
            var s = new QRectF(50, 100, 200, 150);

            Assert.AreEqual(50, s.Left);
            Assert.AreEqual(100, s.Top);
            Assert.AreEqual(200, s.Width);
            Assert.AreEqual(150, s.Height);
        }

        [Test]
        public void TestPointsConstructor()
        {
            var p1 = new QPointF(50, 100);
            var p2 = new QPointF(250, 250);

            var s = new QRectF(p1, p2);

            Assert.AreEqual(50, s.Left);
            Assert.AreEqual(100, s.Top);
            Assert.AreEqual(200, s.Width);
            Assert.AreEqual(150, s.Height);
        }

        [Test]
        public void TestPointSizeConstructor()
        {
            var p1 = new QPointF(50, 100);
            var p2 = new QSizeF(200, 150);

            var s = new QRectF(p1, p2);

            Assert.AreEqual(50, s.Left);
            Assert.AreEqual(100, s.Top);
            Assert.AreEqual(200, s.Width);
            Assert.AreEqual(150, s.Height);
        }
        #endregion

        [Test]
        public void TestAdjust()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            s.Adjust(50, 100, 150, 200);

            Assert.AreEqual(100, s.X);
            Assert.AreEqual(250, s.Y);
            Assert.AreEqual(650, s.Width);
            Assert.AreEqual(800, s.Height);
        }

        [Test]
        public void TestAdjusted()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            var n = s.Adjusted(50, 100, 150, 200);

            Assert.AreEqual(100, n.X);
            Assert.AreEqual(250, n.Y);
            Assert.AreEqual(650, n.Width);
            Assert.AreEqual(800, n.Height);
        }

        [Test]
        public void TestBottom()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            var n = s.Bottom;

            Assert.AreEqual(s.Top + s.Height, n);
        }

        [Test]
        public void TestBottomLeft()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            var n = s.BottomLeft;

            Assert.AreEqual(new QPointF(s.Left, s.Height + s.Top - 1), n);
        }

        [Test]
        public void TestBottomRight()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            var n = s.BottomRight;

            Assert.AreEqual(new QPointF(s.Left + s.Width - 1, s.Top + s.Height - 1), n);
        }

        [Test]
        public void TestCenter()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            var n = s.Center;

            Assert.AreEqual(275, n.X);
            Assert.AreEqual(375, n.Y);
        }

        [Test]
        public void TestContainsWithQPointF()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            var p = new QPointF(275, 375);
            var n = s.Contains(p);

            Assert.IsTrue(n);
        }

        [Test]
        public void TestContainsWithIntegerValues()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            var n = s.Contains(275, 375);

            Assert.IsTrue(n);
        }

        [Test]
        public void TestContainsWithQRectF()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            var p = new QRectF(275, 375, 50, 50);
            var n = s.Contains(p);

            Assert.IsTrue(n);
        }

        [Test]
        public unsafe void TestGetCoords()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            double x1;
            double y1;
            double x2;
            double y2;

            s.GetCoords(&x1, &y1, &x2, &y2);

            Assert.AreEqual(50, x1);
            Assert.AreEqual(150, y1);
            Assert.AreEqual(550, x2);
            Assert.AreEqual(750, y2);
        }

        [Test]
        public void TestSetCoords()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            s.SetCoords(100, 100, 500, 500);

            Assert.AreEqual(100, s.X);
            Assert.AreEqual(100, s.Y);
            Assert.AreEqual(400, s.Width);
            Assert.AreEqual(400, s.Height);
        }

        [Test]
        public unsafe void TestGetRect()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            double x1;
            double y1;
            double width;
            double height;

            s.GetRect(&x1, &y1, &width, &height);

            Assert.AreEqual(50, x1);
            Assert.AreEqual(150, y1);
            Assert.AreEqual(500, width);
            Assert.AreEqual(600, height);
        }

        [Test]
        public void TestSetRect()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            s.SetRect(100, 100, 500, 500);

            Assert.AreEqual(100, s.X);
            Assert.AreEqual(100, s.Y);
            Assert.AreEqual(500, s.Width);
            Assert.AreEqual(500, s.Height);
        }

        [Test]
        public void TestHeight()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            Assert.AreEqual(600, s.Height);
        }

        [Test]
        public void TestIntersected()
        {
            var s1 = new QRectF();
            s1.X = 0;
            s1.Y = 0;
            s1.Width = 500;
            s1.Height = 600;

            var s2 = new QRectF();
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
            var s1 = new QRectF();
            s1.X = 0;
            s1.Y = 0;
            s1.Width = 500;
            s1.Height = 600;

            var s2 = new QRectF();
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
            var s1 = new QRectF();
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
            var s1 = new QRectF();
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
            var s1 = new QRectF();
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
            var s = new QRectF();
            s.X = 50;
            s.Y = 150;
            s.Width = 500;
            s.Height = 600;

            Assert.AreEqual(50, s.Left);
        }

        [Test]
        public void TestMarginsAdded()
        {
            var s = new QRectF();
            s.Left = 50;
            s.Top = 150;
            s.Right = 600;
            s.Bottom = 500;

            var mar = new QMarginsF(50, 100, 150, 200);

            var newR = s.MarginsAdded(mar);

            Assert.AreEqual(100, newR.Left);
            Assert.AreEqual(250, newR.Top);
            Assert.AreEqual(750, newR.Right);
            Assert.AreEqual(700, newR.Bottom);
        }

        [Test]
        public void TestMarginsRemoved()
        {
            var s = new QRectF();
            s.Left = 50;
            s.Top = 150;
            s.Right = 600;
            s.Bottom = 500;

            var mar = new QMarginsF(50, 100, 150, 200);

            var newR = s.MarginsRemoved(mar);

            Assert.AreEqual(0, newR.Left);
            Assert.AreEqual(50, newR.Top);
            Assert.AreEqual(450, newR.Right);
            Assert.AreEqual(300, newR.Bottom);
        }

        [Test]
        public void TestMoveBottom()
        {
            var s = new QRectF();
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
            var s = new QRectF();
            s.X = 50;
            s.Y = 100;
            s.Width = 500;
            s.Height = 600;

            s.MoveBottomLeft(new QPointF(100, 600));

            Assert.AreEqual(500, s.Width);
            Assert.AreEqual(600, s.Height);
            Assert.AreEqual(100, s.X);
            Assert.AreEqual(0, s.Y);
        }

        [Test]
        public void TestMoveBottomRight()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 100;
            s.Width = 500;
            s.Height = 600;

            s.MoveBottomRight(new QPointF(600, 600));

            Assert.AreEqual(500, s.Width);
            Assert.AreEqual(600, s.Height);
            Assert.AreEqual(100, s.X);
            Assert.AreEqual(100, s.Y);
        }

        [Test]
        public void TestMoveCenter()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 100;
            s.Width = 500;
            s.Height = 600;

            s.MoveCenter(new QPointF(600, 600));

            Assert.AreEqual(500, s.Width);
            Assert.AreEqual(600, s.Height);
            Assert.AreEqual(600 - 250, s.X);
            Assert.AreEqual(600 - 300, s.Y);
        }

        [Test]
        public void TestMoveLeft()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 100;
            s.Width = 500;
            s.Height = 600;

            s.MoveLeft(50);

            Assert.AreEqual(500, s.Width);
            Assert.AreEqual(600, s.Height);
            Assert.AreEqual(0, s.X);
            Assert.AreEqual(100, s.Y);
        }

        [Test]
        public void TestMoveRight()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 100;
            s.Width = 500;
            s.Height = 600;

            s.MoveRight(50);

            Assert.AreEqual(500, s.Width);
            Assert.AreEqual(600, s.Height);
            Assert.AreEqual(100, s.X);
            Assert.AreEqual(100, s.Y);
        }

        [Test]
        public void TestMoveToInteger()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 80;
            s.Width = 500;
            s.Height = 600;

            s.MoveTo(100, 100);

            Assert.AreEqual(500, s.Width);
            Assert.AreEqual(600, s.Height);
            Assert.AreEqual(100, s.X);
            Assert.AreEqual(100, s.Y);
        }

        [Test]
        public void TestMoveToQPointF()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 80;
            s.Width = 500;
            s.Height = 600;

            s.MoveTo(new QPointF(100, 100));

            Assert.AreEqual(500, s.Width);
            Assert.AreEqual(600, s.Height);
            Assert.AreEqual(100, s.X);
            Assert.AreEqual(100, s.Y);
        }

        [Test]
        public void TestMoveTop()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 80;
            s.Width = 500;
            s.Height = 600;

            s.MoveTop(100);

            Assert.AreEqual(500, s.Width);
            Assert.AreEqual(600, s.Height);
            Assert.AreEqual(50, s.X);
            Assert.AreEqual(100, s.Y);
        }

        [Test]
        public void TestMoveTopLeft()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 80;
            s.Width = 500;
            s.Height = 600;

            s.MoveTopLeft(new QPointF(100, 100));

            Assert.AreEqual(500, s.Width);
            Assert.AreEqual(600, s.Height);
            Assert.AreEqual(100, s.X);
            Assert.AreEqual(100, s.Y);
        }

        [Test]
        public void TestMoveTopRight()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 80;
            s.Width = 500;
            s.Height = 600;

            s.MoveTopRight(new QPointF(600, 100));

            Assert.AreEqual(500, s.Width);
            Assert.AreEqual(600, s.Height);
            Assert.AreEqual(100, s.X);
            Assert.AreEqual(100, s.Y);
        }

        [Test]
        public void TestNormalized()
        {
            var s = new QRectF();
            s.X = 0;
            s.Y = 0;
            s.Width = -1;
            s.Height = -1;

            var n = s.Normalized;

            Assert.IsTrue(n.Width > 0);
            Assert.IsTrue(n.Height > 0);
            Assert.AreEqual(0, n.X);
            Assert.AreEqual(0, n.Y);
        }

        [Test]
        public void TestRight()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 80;
            s.Width = 500;
            s.Height = 600;


            Assert.AreEqual(500, s.Width);
            Assert.AreEqual(600, s.Height);
            Assert.AreEqual(s.Left + s.Width, s.Right);
            Assert.AreEqual(80, s.Y);
        }

        [Test]
        public void TestSize()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 80;
            s.Width = 500;
            s.Height = 600;

            var size = s.Size;

            Assert.AreEqual(500, size.Width);
            Assert.AreEqual(600, size.Height);
        }

        [Test]
        public void TestTop()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 80;
            s.Width = 500;
            s.Height = 600;

            var size = s.Top;

            Assert.AreEqual(s.Y, size);
        }

        [Test]
        public void TestTopLeft()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 80;
            s.Width = 500;
            s.Height = 600;

            var size = s.TopLeft;

            Assert.AreEqual(s.Y, size.Y);
            Assert.AreEqual(s.X, size.X);
        }

        [Test]
        public void TestTopRight()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 80;
            s.Width = 500;
            s.Height = 600;

            var size = s.TopRight;

            Assert.AreEqual(s.Y, size.Y);
            Assert.AreEqual(s.X + s.Width, size.X);
        }

        [Test]
        public void TestTranslateInteger()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 80;
            s.Width = 500;
            s.Height = 600;

            s.Translate(50, 20);

            Assert.AreEqual(100, s.X);
            Assert.AreEqual(100, s.Y);
        }

        [Test]
        public void TestTranslateQPointF()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 80;
            s.Width = 500;
            s.Height = 600;

            s.Translate(new QPointF(50, 20));

            Assert.AreEqual(100, s.X);
            Assert.AreEqual(100, s.Y);
        }

        [Test]
        public void TestTranslatedInteger()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 80;
            s.Width = 500;
            s.Height = 600;

            var n = s.Translated(50, 20);

            Assert.AreEqual(100, n.X);
            Assert.AreEqual(100, n.Y);
        }

        [Test]
        public void TestTranslatedQPointF()
        {
            var s = new QRectF();
            s.X = 50;
            s.Y = 80;
            s.Width = 500;
            s.Height = 600;

            var n = s.Translated(new QPointF(50, 20));

            Assert.AreEqual(100, n.X);
            Assert.AreEqual(100, n.Y);
        }

        [Test]
        public void TestUnited()
        {
            var s1 = new QRectF();
            s1.X = 0;
            s1.Y = 0;
            s1.Width = 500;
            s1.Height = 600;

            var s2 = new QRectF();
            s2.X = 100;
            s2.Y = 100;
            s2.Width = 500;
            s2.Height = 600;

            var n = s1.United(s2);

            Assert.AreEqual(0, n.X);
            Assert.AreEqual(0, n.Y);
            Assert.AreEqual(600, n.Width);
            Assert.AreEqual(700, n.Height);
        }

        [Test]
        public void TestWidth()
        {
            var s = new QRectF();
            s.X = 0;
            s.Y = 0;
            s.Width = 500;
            s.Height = 600;

            Assert.AreEqual(500, s.Width);
        }

        [Test]
        public void TestX()
        {
            var s = new QRectF();
            s.X = 200;
            s.Y = 300;
            s.Width = 500;
            s.Height = 600;

            Assert.AreEqual(200, s.X);
        }

        [Test]
        public void TestY()
        {
            var s = new QRectF();
            s.X = 200;
            s.Y = 300;
            s.Width = 500;
            s.Height = 600;

            Assert.AreEqual(300, s.Y);
        }

        [Test]
        public void TestAndOperator()
        {
            var s1 = new QRectF();
            s1.X = 0;
            s1.Y = 0;
            s1.Width = 500;
            s1.Height = 600;

            var s2 = new QRectF();
            s2.X = 400;
            s2.Y = 500;
            s2.Width = 500;
            s2.Height = 600;

            var inter = s1 & s2;

            Assert.AreEqual(400, inter.X);
            Assert.AreEqual(500, inter.Y);
            Assert.AreEqual(100, inter.Width);
            Assert.AreEqual(100, inter.Height);
        }

        [Test]
        public void TestAndEqualOperator()
        {
            var s1 = new QRectF();
            s1.X = 0;
            s1.Y = 0;
            s1.Width = 500;
            s1.Height = 600;

            var s2 = new QRectF();
            s2.X = 400;
            s2.Y = 500;
            s2.Width = 500;
            s2.Height = 600;

            s1 &= s2;

            Assert.AreEqual(400, s1.X);
            Assert.AreEqual(500, s1.Y);
            Assert.AreEqual(100, s1.Width);
            Assert.AreEqual(100, s1.Height);
        }

        [Test]
        public void TestAddMarginOperator()
        {
            var s1 = new QRectF();
            s1.X = 0;
            s1.Y = 0;
            s1.Width = 500;
            s1.Height = 600;

            var s2 = new QMarginsF(100, 100, 100, 100);

            s1 += s2;

            Assert.AreEqual(100, s1.X);
            Assert.AreEqual(100, s1.Y);
            Assert.AreEqual(500, s1.Width);
            Assert.AreEqual(600, s1.Height);
        }

        [Test]
        public void TestSubMarginOperator()
        {
            var s1 = new QRectF();
            s1.X = 100;
            s1.Y = 100;
            s1.Width = 500;
            s1.Height = 600;

            var s2 = new QMarginsF(100, 100, 100, 100);

            s1 += s2;

            Assert.AreEqual(0, s1.X);
            Assert.AreEqual(0, s1.Y);
            Assert.AreEqual(500, s1.Width);
            Assert.AreEqual(600, s1.Height);
        }

        [Test]
        public void TestPipeOperator()
        {
            var s1 = new QRectF();
            s1.X = 0;
            s1.Y = 0;
            s1.Width = 500;
            s1.Height = 600;

            var s2 = new QRectF();
            s2.X = 100;
            s2.Y = 100;
            s2.Width = 500;
            s2.Height = 600;

            var n = s1 | s2;

            Assert.AreEqual(0, n.X);
            Assert.AreEqual(0, n.Y);
            Assert.AreEqual(600, n.Width);
            Assert.AreEqual(700, n.Height);
        }

        [Test]
        public void TestPipeEqualOperator()
        {
            var s1 = new QRectF();
            s1.X = 0;
            s1.Y = 0;
            s1.Width = 500;
            s1.Height = 600;

            var s2 = new QRectF();
            s2.X = 100;
            s2.Y = 100;
            s2.Width = 500;
            s2.Height = 600;

            s1 |= s2;

            Assert.AreEqual(0, s1.X);
            Assert.AreEqual(0, s1.Y);
            Assert.AreEqual(600, s1.Width);
            Assert.AreEqual(700, s1.Height);
        }

        [Test]
        public void TestNotEqualOperator()
        {
            var s1 = new QRectF();
            s1.X = 0;
            s1.Y = 0;
            s1.Width = 500;
            s1.Height = 600;

            var s2 = new QRectF();
            s2.X = 100;
            s2.Y = 100;
            s2.Width = 500;
            s2.Height = 600;

            Assert.AreNotEqual(s1, s2);
        }

        [Test]
        public void TestAddMarginToNewRectOperator()
        {
            var s1 = new QRectF();
            s1.X = 0;
            s1.Y = 0;
            s1.Width = 500;
            s1.Height = 600;

            var s2 = new QMarginsF(100, 100, 100, 100);

            var n = s1 + s2;

            Assert.AreEqual(100, n.X);
            Assert.AreEqual(100, n.Y);
            Assert.AreEqual(500, n.Width);
            Assert.AreEqual(600, n.Height);
        }

        [Test]
        public void TestAddMarginToNewRectOperator2()
        {
            var s1 = new QRectF();
            s1.X = 0;
            s1.Y = 0;
            s1.Width = 500;
            s1.Height = 600;

            var s2 = new QMarginsF(100, 100, 100, 100);

            var n = s2 + s1;

            Assert.AreEqual(100, n.X);
            Assert.AreEqual(100, n.Y);
            Assert.AreEqual(500, n.Width);
            Assert.AreEqual(600, n.Height);
        }

        [Test]
        public void TestSubMarginToNewRectOperator()
        {
            var s1 = new QRectF();
            s1.X = 100;
            s1.Y = 100;
            s1.Width = 500;
            s1.Height = 600;

            var s2 = new QMarginsF(100, 100, 100, 100);

            var n = s1 - s2;

            Assert.AreEqual(0, n.X);
            Assert.AreEqual(0, n.Y);
            Assert.AreEqual(500, n.Width);
            Assert.AreEqual(600, n.Height);
        }

        [Test]
        public void TestEqualOperator()
        {
            var s1 = new QRectF();
            s1.X = 0;
            s1.Y = 0;
            s1.Width = 500;
            s1.Height = 600;

            var s2 = new QRectF();
            s2.X = 0;
            s2.Y = 0;
            s2.Width = 500;
            s2.Height = 600;

            Assert.AreEqual(s1, s2);
        }

        // TODO Add Stream Operators
    }
}