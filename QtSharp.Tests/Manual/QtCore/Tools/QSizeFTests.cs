using NUnit.Framework;
using QtCore;
using QtCore.Qt;

namespace QtSharp.Tests.Manual.QtCore.Tools
{
    [TestFixture]
    public class QSizeFTests
    {
        private QSizeF _qSize;

        [SetUp]
        public void Init()
        {
            // TODO: Add Init code.
            _qSize = new QSizeF(500, 100);
        }

        [TearDown]
        public void Dispose()
        {
            // TODO: Add tear down code.
        }

        #region Ctor
        [Test]
        public void TestEmpyConstructor()
        {
            var s = new QSizeF();
        }

        [Test]
        public void TestIntConstructor()
        {
            var s = new QSizeF(100, 200);

            Assert.AreEqual(200, s.Height);
            Assert.AreEqual(100, s.Width);
        }
        #endregion

        [Test]
        public void TestBoundedTo()
        {
            this._qSize.Width = 500;
            this._qSize.Height = 100;

            var other = new QSizeF();
            other.Width = 400;
            other.Height = 500;

            var res = _qSize.BoundedTo(other);

            Assert.AreEqual(400, res.Width);
            Assert.AreEqual(100, res.Height);
        }

        [Test]
        public void TestExpandedTo()
        {
            this._qSize.Width = 500;
            this._qSize.Height = 100;

            var other = new QSizeF();
            other.Width = 400;
            other.Height = 500;

            var res = _qSize.ExpandedTo(other);

            Assert.AreEqual(500, res.Width);
            Assert.AreEqual(500, res.Height);
        }

        [Test]
        public void TestHeight()
        {
            this._qSize.Height = 100;
            Assert.AreEqual(100, _qSize.Height);
        }

        [Test]
        public void TestWidth()
        {
            this._qSize.Width = 500;
            Assert.AreEqual(500, _qSize.Width);
        }

        [Test]
        public void TestIsEmpty()
        {
            this._qSize.Width = 10;
            this._qSize.Height = 0;

            Assert.IsTrue(_qSize.IsEmpty);
        }

        [Test]
        public void TestIsNull()
        {
            this._qSize.Width = 10;
            this._qSize.Height = 10;

            Assert.IsFalse(_qSize.IsNull);
        }

        [Test]
        public void TestIsValid()
        {
            this._qSize.Width = -10;
            this._qSize.Height = 10;

            Assert.IsFalse(_qSize.IsValid);
        }

        [Test]
        public unsafe void TestRefWidthHeight()
        {
            int w = 50;
            int h = 100;

            var rW = _qSize.Rwidth;
            var rH = _qSize.Rheight;

            var resW = *rW + w;
            var resH = *rH + h;

            rW = &resW;
            rH = &resH;

            Assert.AreEqual(_qSize.Width + w, *rW);
            Assert.AreEqual(_qSize.Height + h, *rH);
        }

        [Test]
        public void TestScaleWithInteger()
        {
            var s1 = new QSizeF(10, 12);
            s1.Scale(60, 60, AspectRatioMode.IgnoreAspectRatio);
            Assert.AreEqual(60, s1.Width);
            Assert.AreEqual(60, s1.Height);

            var s2 = new QSizeF(10, 12);
            s2.Scale(60, 60, AspectRatioMode.KeepAspectRatio);
            Assert.AreEqual(50, s2.Width);
            Assert.AreEqual(60, s2.Height);

            var s3 = new QSizeF(10, 12);
            s3.Scale(60, 60, AspectRatioMode.KeepAspectRatioByExpanding);
            Assert.AreEqual(60, s3.Width);
            Assert.AreEqual(72, s3.Height);
        }

        [Test]
        public void TestScaleWithSize()
        {
            var s1 = new QSizeF(10, 12);
            var sS1 = new QSizeF(60, 60);
            s1.Scale(sS1, AspectRatioMode.IgnoreAspectRatio);
            Assert.AreEqual(60, s1.Width);
            Assert.AreEqual(60, s1.Height);

            var s2 = new QSizeF(10, 12);
            var sS2 = new QSizeF(60, 60);
            s2.Scale(sS2, AspectRatioMode.KeepAspectRatio);
            Assert.AreEqual(50, s2.Width);
            Assert.AreEqual(60, s2.Height);

            var s3 = new QSizeF(10, 12);
            var sS3 = new QSizeF(60, 60);
            s3.Scale(sS3, AspectRatioMode.KeepAspectRatioByExpanding);
            Assert.AreEqual(60, s3.Width);
            Assert.AreEqual(72, s3.Height);
        }

        [Test]
        public void TestScaledWithInteger()
        {
            var s1 = new QSizeF(10, 12);
            var res1 = s1.Scaled(60, 60, AspectRatioMode.IgnoreAspectRatio);
            Assert.AreEqual(60, res1.Width);
            Assert.AreEqual(60, res1.Height);

            var s2 = new QSizeF(10, 12);
            var res2 = s2.Scaled(60, 60, AspectRatioMode.KeepAspectRatio);
            Assert.AreEqual(50, res2.Width);
            Assert.AreEqual(60, res2.Height);

            var s3 = new QSize(10, 12);
            var res3 = s3.Scaled(60, 60, AspectRatioMode.KeepAspectRatioByExpanding);
            Assert.AreEqual(60, res3.Width);
            Assert.AreEqual(72, res3.Height);
        }

        [Test]
        public void TestScaledWithSize()
        {
            var s1 = new QSizeF(10, 12);
            var sS1 = new QSizeF(60, 60);
            var res1 = s1.Scaled(sS1, AspectRatioMode.IgnoreAspectRatio);
            Assert.AreEqual(60, res1.Width);
            Assert.AreEqual(60, res1.Height);

            var s2 = new QSizeF(10, 12);
            var sS2 = new QSizeF(60, 60);
            var res2 = s2.Scaled(sS2, AspectRatioMode.KeepAspectRatio);
            Assert.AreEqual(50, res2.Width);
            Assert.AreEqual(60, res2.Height);

            var s3 = new QSizeF(10, 12);
            var sS3 = new QSizeF(60, 60);
            var res3 = s3.Scaled(sS3, AspectRatioMode.KeepAspectRatioByExpanding);
            Assert.AreEqual(60, res3.Width);
            Assert.AreEqual(72, res3.Height);
        }

        [Test]
        public void TestTranspose()
        {
            var size = new QSizeF(500, 100);
            size.Transpose();

            Assert.AreEqual(100, _qSize.Width);
            Assert.AreEqual(500, _qSize.Height);
        }

        [Test]
        public void TestTransposed()
        {
            var size = new QSizeF(500, 100);
            var tr = size.Transposed;

            Assert.AreEqual(100, tr.Width);
            Assert.AreEqual(500, tr.Height);
        }

        [Test]
        public void TestMultWidthIntegerOperator()
        {
            //var size = new QSizeF(500, 100);
            //size *= 5;

            //Assert.AreEqual(2500, size.Width);
            //Assert.AreEqual(500, size.Height);

            throw new AssertionException("Not implemented");
        }

        [Test]
        public void TestAddWidthIntegerOperator()
        {
            //var size = new QSizeF(500, 100);
            //size += 5;

            //Assert.AreEqual(505, size.Width);
            //Assert.AreEqual(105, size.Height);

            throw new AssertionException("Not implemented");
        }

        [Test]
        public void TestSubWidthIntegerOperator()
        {
            //var size = new QSizeF(500, 100);
            //size -= 5;

            //Assert.AreEqual(495, size.Width);
            //Assert.AreEqual(95, size.Height);

            throw new AssertionException("Not implemented");
        }

        [Test]
        public void TestDivWidthIntegerOperator()
        {
            //var size = new QSizeF(500, 100);
            //size /= 5;

            //Assert.AreEqual(100, size.Width);
            //Assert.AreEqual(20, size.Height);

            throw new AssertionException("Not implemented");
        }

        [Test]
        public void TestNotEqualOperator()
        {
            var size = new QSizeF(500, 100);
            var size2 = new QSizeF(100, 50);

            Assert.AreNotEqual(size, size2);
            throw new AssertionException("Not implemented");
        }

        [Test]
        public void TestMultOperator()
        {
            var size = new QSizeF(500, 100);

            //var res = size*50;

            //Assert.AreEqual(500 * 50, res.Width);
            //Assert.AreEqual(50 * 100, res.Height);

            throw new AssertionException("Not implemented");
        }

        [Test]
        public void TestMult2Operator()
        {
            var size = new QSizeF(500, 100);

            //var res = 50*size;

            //Assert.AreEqual(500 * 50, res.Width);
            //Assert.AreEqual(50 * 100, res.Height);

            throw new AssertionException("Not implemented");
        }

        [Test]
        public void TestAddOperator()
        {
            var size = new QSizeF(500, 100);
            var size2 = new QSizeF(500, 100);

            //var res = size + size2;

            //Assert.AreEqual(1000, res.Width);
            //Assert.AreEqual(200, res.Height);

            throw new AssertionException("Not implemented");
        }

        [Test]
        public void TestSubOperator()
        {
            var size = new QSizeF(500, 100);
            var size2 = new QSizeF(500, 100);

            //var res = size - size2;

            //Assert.AreEqual(0, res.Width);
            //Assert.AreEqual(0, res.Height);

            throw new AssertionException("Not implemented");
        }

        [Test]
        public void TestDivOperator()
        {
            var size = new QSizeF(500, 100);

            //var res = size / 5;

            //Assert.AreEqual(100, res.Width);
            //Assert.AreEqual(20, res.Height);

            throw new AssertionException("Not implemented");
        }

        [Test]
        public void TestEqualOperator()
        {
            var size = new QSizeF(500, 100);
            var size2 = new QSizeF(500, 100);

            Assert.AreEqual(size, size2);

            throw new AssertionException("Not implemented");
        }
    }
}