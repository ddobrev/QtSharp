using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore.Tools
{
    [TestFixture]
    public class QMarginsTests
    {
        private QMargins _margins;
        private const int Left = 5;
        private const int Top = 5;
        private const int Right = 10;
        private const int Bottom = 10;

        [SetUp]
        public void Init()
        {
            _margins = new QMargins(Left, Top, Right, Bottom);
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
            var s = new QMargins();
        }

        [Test]
        public void TestIntegerConstructor()
        {
            var s = new QMargins(5, 5, 10, 10);
        }

        [Test]
        public void TestBottom()
        {
            Assert.AreEqual(Bottom, _margins.Bottom);
        }

        [Test]
        public void TestIsNull()
        {
            Assert.IsFalse(_margins.IsNull);
        }

        [Test]
        public void TestLeft()
        {
            Assert.AreEqual(Left, _margins.Left);
        }

        [Test]
        public void TestRight()
        {
            Assert.AreEqual(Right, _margins.Right);
        }

        [Test]
        public void TestMultEqualOperator()
        {
            const int factor = 5;

            _margins *= factor;

            Assert.AreEqual(Left * factor, _margins.Left);
            Assert.AreEqual(Top * factor, _margins.Top);
            Assert.AreEqual(Right * factor, _margins.Right);
            Assert.AreEqual(Bottom * factor, _margins.Bottom);
        }

        [Test]
        public void TestAddEqualWithMarginsOperator()
        {
            _margins += new QMargins(1, 2, 3, 4);

            Assert.AreEqual(Left + 1, _margins.Left);
            Assert.AreEqual(Top + 2, _margins.Top);
            Assert.AreEqual(Right + 3, _margins.Right);
            Assert.AreEqual(Bottom + 4, _margins.Bottom);
        }

        [Test]
        public void TestAddEqualWithIntOperator()
        {
            _margins += 2;

            Assert.AreEqual(Left + 2, _margins.Left);
            Assert.AreEqual(Top + 2, _margins.Top);
            Assert.AreEqual(Right + 2, _margins.Right);
            Assert.AreEqual(Bottom + 2, _margins.Bottom);
        }

        [Test]
        public void TestSubEqualWithMarginsOperator()
        {
            _margins -= new QMargins(1, 2, 3, 4);

            Assert.AreEqual(Left - 1, _margins.Left);
            Assert.AreEqual(Top - 2, _margins.Top);
            Assert.AreEqual(Right - 3, _margins.Right);
            Assert.AreEqual(Bottom - 4, _margins.Bottom);
        }

        [Test]
        public void TestSubEqualWithIntOperator()
        {
            _margins -= 2;

            Assert.AreEqual(Left - 2, _margins.Left);
            Assert.AreEqual(Top - 2, _margins.Top);
            Assert.AreEqual(Right - 2, _margins.Right);
            Assert.AreEqual(Bottom - 2, _margins.Bottom);
        }

        [Test]
        public void TestDivEqualOperator()
        {
            const int factor = 5;

            _margins /= factor;

            Assert.AreEqual(Left / factor, _margins.Left);
            Assert.AreEqual(Top / factor, _margins.Top);
            Assert.AreEqual(Right / factor, _margins.Right);
            Assert.AreEqual(Bottom / factor, _margins.Bottom);
        }

        [Test]
        public void TestNotEqualOperator()
        {
            var res = new QMargins(Left, Top, Right, Bottom);

            Assert.AreNotEqual(res, _margins);
        }

        [Test]
        public void TestMult1Operator()
        {
            const int factor = 5;

            var res = _margins * factor;

            Assert.AreEqual(Left * factor, res.Left);
            Assert.AreEqual(Top * factor, res.Top);
            Assert.AreEqual(Right * factor, res.Right);
            Assert.AreEqual(Bottom * factor, res.Bottom);
        }

        [Test]
        public void TestMult2Operator()
        {
            const int factor = 5;

            var res = factor * _margins;

            Assert.AreEqual(Left * factor, res.Left);
            Assert.AreEqual(Top * factor, res.Top);
            Assert.AreEqual(Right * factor, res.Right);
            Assert.AreEqual(Bottom * factor, res.Bottom);
        }

        [Test]
        public void TestAddWithMarginsOperator()
        {
            var res = _margins + new QMargins(1, 2, 3, 4);

            Assert.AreEqual(Left + 1, res.Left);
            Assert.AreEqual(Top + 2, res.Top);
            Assert.AreEqual(Right + 3, res.Right);
            Assert.AreEqual(Bottom + 4, res.Bottom);
        }

        [Test]
        public void TestAdd1WithIntOperator()
        {
            var res = _margins + 2;

            Assert.AreEqual(Left + 2, res.Left);
            Assert.AreEqual(Top + 2, res.Top);
            Assert.AreEqual(Right + 2, res.Right);
            Assert.AreEqual(Bottom + 2, res.Bottom);
        }

        [Test]
        public void TestAdd2WithIntOperator()
        {
            var res = 2 + _margins;

            Assert.AreEqual(Left + 2, res.Left);
            Assert.AreEqual(Top + 2, res.Top);
            Assert.AreEqual(Right + 2, res.Right);
            Assert.AreEqual(Bottom + 2, res.Bottom);
        }

        [Test]
        public void TestAddMarginsToThisOperator()
        {
            _margins = _margins + new QMargins(1, 2, 3, 4);

            Assert.AreEqual(Left + 1, _margins.Left);
            Assert.AreEqual(Top + 2, _margins.Top);
            Assert.AreEqual(Right + 3, _margins.Right);
            Assert.AreEqual(Bottom + 4, _margins.Bottom);
        }

        [Test]
        public void TestSubWithMarginsOperator()
        {
            var res = _margins - new QMargins(1, 2, 3, 4);

            Assert.AreEqual(Left - 1, res.Left);
            Assert.AreEqual(Top - 2, res.Top);
            Assert.AreEqual(Right - 3, res.Right);
            Assert.AreEqual(Bottom - 4, res.Bottom);
        }

        [Test]
        public void TestSubWithIntOperator()
        {
            var res = _margins - 2;

            Assert.AreEqual(Left - 2, res.Left);
            Assert.AreEqual(Top - 2, res.Top);
            Assert.AreEqual(Right - 2, res.Right);
            Assert.AreEqual(Bottom - 2, res.Bottom);
        }

        [Test]
        public void TestSubMarginsToThisOperator()
        {
            _margins = _margins - new QMargins(1, 2, 3, 4);

            Assert.AreEqual(Left - 1, _margins.Left);
            Assert.AreEqual(Top - 2, _margins.Top);
            Assert.AreEqual(Right - 3, _margins.Right);
            Assert.AreEqual(Bottom - 4, _margins.Bottom);
        }

        [Test]
        public void TestDivOperator()
        {
            const int factor = 5;

            var res = _margins / factor;

            Assert.AreEqual(Left / factor, res.Left);
            Assert.AreEqual(Top / factor, res.Top);
            Assert.AreEqual(Right / factor, res.Right);
            Assert.AreEqual(Bottom / factor, res.Bottom);
        }

        [Test]
        public void TestEqualOperator()
        {
            var res = new QMargins(Left, Top, Right, Bottom);

            Assert.AreEqual(res, _margins);
        }
    }
}