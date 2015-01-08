using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore.Tools
{
    [TestFixture]
    public class QMarginsFTests
    {
        private QMarginsF _margins;
        private const double Left = 5.0;
        private const double Top = 5.0;
        private const double Right = 10.0;
        private const double Bottom = 10.0;

        [SetUp]
        public void Init()
        {
            _margins = new QMarginsF(Left, Top, Right, Bottom);
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
            var s = new QMarginsF();
        }

        [Test]
        public void TestIntegerConstructor()
        {
            var s = new QMarginsF(5.0, 5.0, 10.0, 10.0);
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
            _margins += new QMarginsF(1.0, 2.0, 3.0, 4.0);

            Assert.AreEqual(Left + 1.0, _margins.Left);
            Assert.AreEqual(Top + 2.0, _margins.Top);
            Assert.AreEqual(Right + 3.0, _margins.Right);
            Assert.AreEqual(Bottom + 4.0, _margins.Bottom);
        }

        [Test]
        public void TestAddEqualWithIntOperator()
        {
            _margins += 2.0;

            Assert.AreEqual(Left + 2.0, _margins.Left);
            Assert.AreEqual(Top + 2.0, _margins.Top);
            Assert.AreEqual(Right + 2.0, _margins.Right);
            Assert.AreEqual(Bottom + 2.0, _margins.Bottom);
        }

        [Test]
        public void TestSubEqualWithMarginsOperator()
        {
            _margins -= new QMarginsF(1.0, 2.0, 3.0, 4.0);

            Assert.AreEqual(Left - 1.0, _margins.Left);
            Assert.AreEqual(Top - 2.0, _margins.Top);
            Assert.AreEqual(Right - 3.0, _margins.Right);
            Assert.AreEqual(Bottom - 4.0, _margins.Bottom);
        }

        [Test]
        public void TestSubEqualWithIntOperator()
        {
            _margins -= 2.0;

            Assert.AreEqual(Left - 2.0, _margins.Left);
            Assert.AreEqual(Top - 2.0, _margins.Top);
            Assert.AreEqual(Right - 2.0, _margins.Right);
            Assert.AreEqual(Bottom - 2.0, _margins.Bottom);
        }

        [Test]
        public void TestDivEqualOperator()
        {
            const double factor = 5.0;

            _margins /= factor;

            Assert.AreEqual(Left / factor, _margins.Left);
            Assert.AreEqual(Top / factor, _margins.Top);
            Assert.AreEqual(Right / factor, _margins.Right);
            Assert.AreEqual(Bottom / factor, _margins.Bottom);
        }

        [Test]
        public void TestNotEqualOperator()
        {
            var res = new QMarginsF(Left, Top, Right, Bottom);

            Assert.AreNotEqual(res, _margins);
        }

        [Test]
        public void TestMult1Operator()
        {
            const double factor = 5.0;

            var res = _margins * factor;

            Assert.AreEqual(Left * factor, res.Left);
            Assert.AreEqual(Top * factor, res.Top);
            Assert.AreEqual(Right * factor, res.Right);
            Assert.AreEqual(Bottom * factor, res.Bottom);
        }

        [Test]
        public void TestMult2Operator()
        {
            const double factor = 5.0;

            var res = factor * _margins;

            Assert.AreEqual(Left * factor, res.Left);
            Assert.AreEqual(Top * factor, res.Top);
            Assert.AreEqual(Right * factor, res.Right);
            Assert.AreEqual(Bottom * factor, res.Bottom);
        }

        [Test]
        public void TestAddWithMarginsOperator()
        {
            var res = _margins + new QMarginsF(1.0, 2.0, 3.0, 4.0);

            Assert.AreEqual(Left + 1.0, res.Left);
            Assert.AreEqual(Top + 2.0, res.Top);
            Assert.AreEqual(Right + 3.0, res.Right);
            Assert.AreEqual(Bottom + 4.0, res.Bottom);
        }

        [Test]
        public void TestAdd1WithIntOperator()
        {
            var res = _margins + 2.0;

            Assert.AreEqual(Left + 2.0, res.Left);
            Assert.AreEqual(Top + 2.0, res.Top);
            Assert.AreEqual(Right + 2.0, res.Right);
            Assert.AreEqual(Bottom + 2.0, res.Bottom);
        }

        [Test]
        public void TestAdd2WithIntOperator()
        {
            var res = 2.0 + _margins;

            Assert.AreEqual(Left + 2.0, res.Left);
            Assert.AreEqual(Top + 2.0, res.Top);
            Assert.AreEqual(Right + 2.0, res.Right);
            Assert.AreEqual(Bottom + 2.0, res.Bottom);
        }

        [Test]
        public void TestAddMarginsToThisOperator()
        {
            _margins = _margins + new QMarginsF(1.0, 2.0, 3.0, 4.0);

            Assert.AreEqual(Left + 1.0, _margins.Left);
            Assert.AreEqual(Top + 2.0, _margins.Top);
            Assert.AreEqual(Right + 3.0, _margins.Right);
            Assert.AreEqual(Bottom + 4.0, _margins.Bottom);
        }

        [Test]
        public void TestSubWithMarginsOperator()
        {
            var res = _margins - new QMarginsF(1.0, 2.0, 3.0, 4.0);

            Assert.AreEqual(Left - 1.0, res.Left);
            Assert.AreEqual(Top - 2.0, res.Top);
            Assert.AreEqual(Right - 3.0, res.Right);
            Assert.AreEqual(Bottom - 4.0, res.Bottom);
        }

        [Test]
        public void TestSubWithIntOperator()
        {
            var res = _margins - 2.0;

            Assert.AreEqual(Left - 2.0, res.Left);
            Assert.AreEqual(Top - 2.0, res.Top);
            Assert.AreEqual(Right - 2.0, res.Right);
            Assert.AreEqual(Bottom - 2.0, res.Bottom);
        }

        [Test]
        public void TestSubMarginsToThisOperator()
        {
            _margins = _margins - new QMarginsF(1.0, 2.0, 3.0, 4.0);

            Assert.AreEqual(Left - 1.0, _margins.Left);
            Assert.AreEqual(Top - 2.0, _margins.Top);
            Assert.AreEqual(Right - 3.0, _margins.Right);
            Assert.AreEqual(Bottom - 4.0, _margins.Bottom);
        }

        [Test]
        public void TestDivOperator()
        {
            const double factor = 5.0;

            var res = _margins / factor;

            Assert.AreEqual(Left / factor, res.Left);
            Assert.AreEqual(Top / factor, res.Top);
            Assert.AreEqual(Right / factor, res.Right);
            Assert.AreEqual(Bottom / factor, res.Bottom);
        }

        [Test]
        public void TestEqualOperator()
        {
            var res = new QMarginsF(Left, Top, Right, Bottom);

            Assert.AreEqual(res, _margins);
        }
    }
}