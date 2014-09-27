using System;
using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.QtCore
{
    [TestFixture]
    public class QUrlTests
    {
        private const string Url = "ftp://tray:5uQQo_f@ftp.example.com:2021?58#questionX";
        private const string Url2 = "http://www.example.com/List of holidays.xml";
        private QUrl _qUrl;

        [SetUp]
        public void Init()
        {
            // TODO: Add Init code.
            _qUrl = new QUrl(Url);
        }

        [TearDown]
        public void Dispose()
        {
            // TODO: Add tear down code.
        }

        [Test]
        public void TestEmpyConstructor()
        {
            var s = new QUrl();
            Assert.NotNull(s);
        }

        [Test]
        public void TestStringTolerantConstructor()
        {
            var s = new QUrl("http://www.example.com/List of holidays.xml");
            Assert.NotNull(s);
        }

        [Test]
        public void TestStringStrictConstructor()
        {
            var s = new QUrl("http://www.example.com/List of holidays.xml", QUrl.ParsingMode.StrictMode);
            Assert.NotNull(s);
        }

        [Test]
        public void TestStringDecodedConstructor()
        {
            var s = new QUrl("http://www.example.com/List of holidays.xml", QUrl.ParsingMode.DecodedMode);
            Assert.NotNull(s);
        }

        [Ignore("Bug!")]
        [Test]
        public void TestCopyConstructor()
        {
            var s = new QUrl("http://www.example.com/List of holidays.xml");

            var n = new QUrl(s);

            Assert.NotNull(n);
        }

        [Test]
        public void TestAdjusted()
        {
            throw new AssertionException("Not implemented!");
        }

        [Test]
        public void TestAuthority()
        {
            foreach (QUrl.ComponentFormattingOption formatting in Enum.GetValues(typeof(QUrl.ComponentFormattingOption)))
            {
                var s = _qUrl.Authority(formatting);
                Assert.IsNotNullOrEmpty(s, formatting.ToString());
            }
        }

        [Test]
        public void TestClear()
        {
            _qUrl.Clear();
            Assert.IsNullOrEmpty(_qUrl.Path());
        }

        [Test]
        public void TestFragment()
        {
            var s = _qUrl.Fragment();

            Assert.IsNotNullOrEmpty(s);
        }

        [Test]
        public void TestFromAce()
        {
            // TODO
        }

        [Test]
        public void TestFromCFURL()
        {
            // TODO
        }

        [Test]
        public void TestFromEncoded()
        {
            // TODO
        }

        [Test]
        public void TestFromLocalFile()
        {
            var u = QUrl.FromLocalFile("//servername/path/to/file.txt");

            Assert.AreEqual("file.txt", u.FileName());
        }

        [Test]
        public void TestFromNSURL()
        {
            // TODO
        }

        [Test]
        public void TestFromPercentEncoding()
        {
            // TODO
        }

        [Test]
        public void TestFromStringList()
        {
            var url = QUrl.FromUserInput("qt-project.org");

            // TODO
        }

        [Test]
        public void TestHasFragment()
        {
            Assert.IsTrue(_qUrl.HasFragment);
        }

        [Test]
        public void TestHasQuery()
        {
            Assert.IsTrue(_qUrl.HasQuery);
        }

        [Test]
        public void TestHost()
        {
            Assert.AreEqual("ftp.example.com", _qUrl.Host());
        }

        [Test]
        public void TestIdnWhitelist()
        {
            var w = QUrl.IdnWhitelist;
            // TODO
        }

        [Test]
        public void TestIsEmpty()
        {
            Assert.IsFalse(_qUrl.IsEmpty);
        }
        
        [Test]
        public void TestIsLocalFile()
        {
            var u = QUrl.FromLocalFile("//servername/path/to/file.txt");
            Assert.IsTrue(u.IsLocalFile);
        }

        [Test]
        public void TestIsParentOf()
        {
            var u = _qUrl.IsParentOf(new QUrl());
            // TODO
        }

        [Test]
        public void TestIsRelative()
        {
            Assert.IsFalse(_qUrl.IsRelative);
        }

        [Test]
        public void TestIsValid()
        {
            Assert.IsTrue(_qUrl.IsValid);
        }

        [Test]
        public void TestMatches()
        {
            // TODO
        }

        [Test]
        public void TestPassword()
        {
            var p = _qUrl.Password();

            Assert.AreEqual("5uQQo_f", p);
        }

        [Test]
        public void TestPath()
        {
            var p = _qUrl.Path();
            // TODO
            //Assert.AreEqual("5uQQo_f", p);
        }

        [Test]
        public void TestPort()
        {
            var p = _qUrl.Port();
            
            Assert.AreEqual(2021, p);
        }

        [Test]
        public void TestQuery()
        {
            var p = _qUrl.Query();

            Assert.AreEqual(58, p);
        }

        [Test]
        public void TestResolved()
        {
            var baseUrl = new QUrl("http://qt.digia.com/Support/");
            var relativeUrl = new QUrl("../Product/Library/");

            var res = baseUrl.Resolved(relativeUrl);

            Assert.AreEqual("http://qt.digia.com/Product/Library/", res.ToString());
        }

        // TODO Add Schemes










    }
}