using System;
using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore.IO
{
    [TestFixture]
    public class QUrlTests
    {
        private const string Url = "ftp://tray:5uQQo_f@ftp.example.com:2021?58#question13";
        private const string Url2 = "http://www.example.com/List of holidays.xml";
        private QUrl qUrl;

        [SetUp]
        public void Init()
        {
            this.qUrl = new QUrl(Url);
        }

        [TearDown]
        public void Dispose()
        {
            this.qUrl.Dispose();
        }

        [Test]
        public void TestEmpyConstructor()
        {
            using (new QUrl())
            {
            }
        }

        [Test]
        public void TestStringTolerantConstructor()
        {
            using (new QUrl(Url2))
            {
            }
        }

        [Test]
        public void TestStringStrictConstructor()
        {
            using (new QUrl(Url2, QUrl.ParsingMode.StrictMode))
            {
            }
        }

        [Test]
        public void TestStringDecodedConstructor()
        {
            using (new QUrl(Url2, QUrl.ParsingMode.DecodedMode))
            {
            }
        }

        [Test]
        public void TestCopyConstructor()
        {
            using (var s = new QUrl(Url2))
            {
                using (new QUrl(s))
                {
                }
            }
        }

        [Test]
        public void TestAdjusted()
        {
            throw new AssertionException("Not implemented!");
        }

        [Test]
        public void TestAuthority()
        {
            foreach (
                QUrl.ComponentFormattingOption formatting in Enum.GetValues(typeof(QUrl.ComponentFormattingOption)))
            {
                var s = this.qUrl.Authority(formatting);
                Assert.IsNotNullOrEmpty(s, "Problem in enum: " + formatting.ToString());
            }
        }

        [Test]
        public void TestClear()
        {
            this.qUrl.Clear();
            Assert.IsTrue(this.qUrl.IsEmpty);
        }

        [Test]
        public void TestFragment()
        {
            var s = this.qUrl.Fragment();

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
            using (var url = QUrl.FromUserInput("qt-project.org"))
            {
            }

            // TODO
        }

        [Test]
        public void TestHasFragment()
        {
            Assert.IsTrue(this.qUrl.HasFragment);
        }

        [Test]
        public void TestHasQuery()
        {
            Assert.IsTrue(this.qUrl.HasQuery);
        }

        [Test]
        public void TestHost()
        {
            Assert.AreEqual("ftp.example.com", this.qUrl.Host());
        }

        [Test]
        public void TestIdnWhitelist()
        {
            using (var w = QUrl.IdnWhitelist)
            {
            }
            // TODO
        }

        [Test]
        public void TestIsEmpty()
        {
            Assert.IsFalse(this.qUrl.IsEmpty);
        }

        [Test]
        public void TestIsLocalFile()
        {
            var u = QUrl.FromLocalFile("//servername/path/to/file.txt");
            Assert.IsTrue(u.IsLocalFile);
        }

        [Ignore("Bug!")]
        [Test]
        public void TestIsParentOf()
        {
            var u = this.qUrl.IsParentOf(new QUrl());
            // TODO
        }

        [Test]
        public void TestIsRelative()
        {
            Assert.IsFalse(this.qUrl.IsRelative);
        }

        [Test]
        public void TestIsValid()
        {
            Assert.IsTrue(this.qUrl.IsValid);
        }

        [Test]
        public void TestMatches()
        {
            using (var n = new QUrl("ftp://tray:5uQQo_f@ftp.example.com:2021?58#question13"))
            {
            }

            throw new AssertionException("Not implemented!");
        }

        [Test]
        public void TestPassword()
        {
            var p = this.qUrl.Password();

            Assert.AreEqual("5uQQo_f", p);
        }

        [Test]
        public void TestPath()
        {
            var p = this.qUrl.Path();
            // TODO
            //Assert.AreEqual("5uQQo_f", p);
        }

        [Test]
        public void TestPort()
        {
            var p = this.qUrl.Port();

            Assert.AreEqual(2021, p);
        }

        [Test]
        public void TestQuery()
        {
            var p = this.qUrl.Query();

            Assert.AreEqual(58.ToString(), p);
        }

        [Test]
        public void TestResolved()
        {
            var baseUrl = new QUrl("http://qt.digia.com/Support/");
            var relativeUrl = new QUrl("../Product/Library/");

            var res = baseUrl.Resolved(relativeUrl);

            Assert.AreEqual("http://qt.digia.com/Product/Library/", res.ToString());
        }

        [Test]
        public void TestScheme()
        {
            var baseUrl = new QUrl();
            baseUrl.Scheme = "ftp";
            baseUrl.SetAuthority("tray:5uQQo_f@ftp.example.com:2021");
            baseUrl.SetFragment("#question13");

            Assert.AreEqual("ftp", baseUrl.Scheme);
        }

        [Test]
        public void TestSetAuthority()
        {
            var baseUrl = new QUrl();
            baseUrl.Scheme = "ftp";
            baseUrl.SetAuthority("tray:5uQQo_f@ftp.example.com:2021");
            baseUrl.SetFragment("#question13");

            Assert.AreEqual("tray:5uQQo_f@ftp.example.com:2021", baseUrl.Authority());
        }

        [Test]
        public void TestSetFragment()
        {
            var baseUrl = new QUrl();
            baseUrl.Scheme = "ftp";
            baseUrl.SetAuthority("tray:5uQQo_f@ftp.example.com:2021");
            baseUrl.SetFragment("#question13");

            Assert.AreEqual("#question13", baseUrl.Fragment());
        }

        [Test]
        public void TestSetHost()
        {
            var baseUrl = new QUrl();
            baseUrl.Scheme = "ftp";
            baseUrl.SetAuthority("tray:5uQQo_f@ftp.example.com:2021");
            baseUrl.SetFragment("#question13");
            baseUrl.SetHost("ftp.example2.com");

            Assert.AreEqual("ftp.example2.com", baseUrl.Host());
        }

        [Test]
        public void TestSetIdnWhitelist()
        {
            // TODO
        }

        [Test]
        public void TestSetPassword()
        {
            var baseUrl = new QUrl();
            baseUrl.Scheme = "ftp";
            baseUrl.SetAuthority("tray:5uQQo_f@ftp.example.com:2021");
            baseUrl.SetFragment("#question13");
            baseUrl.SetHost("ftp.example.com");
            baseUrl.SetPassword("5uQQo_f2");

            Assert.AreEqual("5uQQo_f2", baseUrl.Password());
        }

        [Test]
        public void TestSetPath()
        {
            var baseUrl = new QUrl();
            baseUrl.Scheme = "ftp";
            baseUrl.SetAuthority("tray:5uQQo_f@ftp.example.com:2021");
            baseUrl.SetFragment("#question13");
            baseUrl.SetHost("ftp.example.com");
            baseUrl.SetPassword("5uQQo_f2");
            baseUrl.SetPath("/pub/something/");

            Assert.AreEqual("/pub/something/", baseUrl.Path());
        }

        [Test]
        public void TestSetPort()
        {
            var baseUrl = new QUrl();
            baseUrl.Scheme = "ftp";
            baseUrl.SetAuthority("tray:5uQQo_f@ftp.example.com:2021");
            baseUrl.SetFragment("#question13");
            baseUrl.SetHost("ftp.example.com");
            baseUrl.SetPassword("5uQQo_f2");
            baseUrl.SetPath("/pub/something/");
            baseUrl.SetPort(2022);

            Assert.AreEqual(2022, baseUrl.Port());
        }

        [Test]
        public void TestSetQuery()
        {
            var baseUrl = new QUrl();
            baseUrl.Scheme = "ftp";
            baseUrl.SetAuthority("tray:5uQQo_f@ftp.example.com:2021");
            baseUrl.SetFragment("#question13");
            baseUrl.SetHost("ftp.example.com");
            baseUrl.SetPassword("5uQQo_f2");
            baseUrl.SetPath("/pub/something/");
            baseUrl.SetPort(2022);
            //baseUrl.SetQuery(new QUrlQuery("new"));
            baseUrl.SetQuery("new");

            Assert.AreEqual("new", baseUrl.Query());
        }

        [Test]
        public void TestSetQuery2()
        {
            var baseUrl = new QUrl();
            baseUrl.Scheme = "ftp";
            baseUrl.SetAuthority("tray:5uQQo_f@ftp.example.com:2021");
            baseUrl.SetFragment("#question13");
            baseUrl.SetHost("ftp.example.com");
            baseUrl.SetPassword("5uQQo_f2");
            baseUrl.SetPath("/pub/something/");
            baseUrl.SetPort(2022);
            baseUrl.SetQuery(new QUrlQuery("new"));

            Assert.AreEqual("new", baseUrl.Query());
        }

        [Test]
        public void TestSetUrl()
        {
            var baseUrl = new QUrl();
            baseUrl.Scheme = "ftp";
            baseUrl.SetAuthority("tray:5uQQo_f@ftp.example.com:2021");
            baseUrl.SetFragment("#question13");
            baseUrl.SetHost("ftp.example.com");
            baseUrl.SetPassword("5uQQo_f2");
            baseUrl.SetPath("/pub/something/");
            baseUrl.SetPort(2022);
            baseUrl.SetUrl("");

            throw new AssertionException("Url() not implemented!");

            //Assert.AreEqual(2022, baseUrl.Url());
        }

        [Test]
        public void TestSetUserInfo()
        {
            var baseUrl = new QUrl();
            baseUrl.Scheme = "ftp";
            baseUrl.SetAuthority("tray:5uQQo_f@ftp.example.com:2021");
            baseUrl.SetFragment("#question13");
            baseUrl.SetHost("ftp.example.com");
            baseUrl.SetUserInfo("tray2:5uQQo_f2");
            baseUrl.SetPath("/pub/something/");
            baseUrl.SetPort(2022);
            baseUrl.SetUrl("");

            Assert.AreEqual("tray2", baseUrl.UserName());
            Assert.AreEqual("5uQQo_f2", baseUrl.Password());
            Assert.AreEqual("tray2:5uQQo_f2", baseUrl.UserInfo());
        }

        [Test]
        public void TestSetUserName()
        {
            var baseUrl = new QUrl();
            baseUrl.Scheme = "ftp";
            baseUrl.SetAuthority("tray:5uQQo_f@ftp.example.com:2021");
            baseUrl.SetFragment("#question13");
            baseUrl.SetHost("ftp.example.com");
            baseUrl.SetUserName("tray2");
            baseUrl.SetPath("/pub/something/");
            baseUrl.SetPort(2022);
            baseUrl.SetUrl("");

            Assert.AreEqual("tray2", baseUrl.UserName());
        }

        //[Ignore("Bug!")]
        [Test]
        public void TestSwap()
        {
            var baseUrl = new QUrl();
            baseUrl.Scheme = "ftp";
            baseUrl.SetAuthority("tray:5uQQo_f@ftp.example.com:2021");
            baseUrl.SetFragment("#question13");
            baseUrl.SetHost("ftp.example.com");
            baseUrl.SetUserName("tray2");
            baseUrl.SetPath("/pub/something/");
            baseUrl.SetPort(2022);
            baseUrl.Swap(new QUrl("llll"));

            throw new AssertionException("Url() not implemented!");

            //Assert.AreEqual(2022, baseUrl.Url());
        }

        [Test]
        public void TestToAce()
        {
            // TODO
        }

        [Test]
        public void TestToCFURL()
        {
            // TODO
        }

        [Test]
        public void TestToDisplayString()
        {
            // TODO
        }

        [Test]
        public void TestToEncoded()
        {
            // TODO
        }

        [Test]
        public void TestToLocalFile()
        {
            // TODO
        }

        [Test]
        public void TestToNSURL()
        {
            // TODO
        }

        [Test]
        public void TestToPercentEncoding()
        {
            // TODO
        }

        [Test]
        public void TestToString()
        {
            // TODO
        }

        [Test]
        public void TestToStringList()
        {
            // TODO
        }

        [Test]
        public void TestTopLevelDomain()
        {
            var baseUrl = new QUrl();
            baseUrl.Scheme = "ftp";
            baseUrl.SetAuthority("tray:5uQQo_f@ftp.example.com:2021");
            baseUrl.SetFragment("#question13");
            baseUrl.SetHost("ftp.example.com");
            baseUrl.SetUserName("tray2");
            baseUrl.SetPath("/pub/something/");
            baseUrl.SetPort(2022);
            baseUrl.SetUrl("");

            Assert.AreEqual(".com", baseUrl.TopLevelDomain());
        }

        [Test]
        public void TestUrl()
        {
            var baseUrl = new QUrl();
            baseUrl.Scheme = "ftp";
            baseUrl.SetAuthority("tray:5uQQo_f@ftp.example.com:2021");
            baseUrl.SetFragment("#question13");
            baseUrl.SetHost("ftp.example.com");
            baseUrl.SetUserName("tray2");
            baseUrl.SetPath("/pub/something/");
            baseUrl.SetPort(2022);
            baseUrl.SetUrl("");

            throw new AssertionException("Url() not implemented!");
            //Assert.AreEqual(".com", baseUrl.Url());
        }

        [Test]
        public void TestNotEqualOperator()
        {
            var n = new QUrl(Url);

            Assert.IsFalse(n != this.qUrl);
        }

        [Test]
        public void TestEqualWithQUrlOperator()
        {
            QUrl n = this.qUrl;

            //Assert.IsFalse(n != _qUrl);
        }

        [Test]
        public void TestEqualWithStringOperator()
        {
            //QUrl n = _qUrl.Url();
            throw new AssertionException("Url() not implemented!");

            //Assert.AreEqual(n, _qUrl);           
        }

        [Test]
        public void TestEqualOperator()
        {
            var n = new QUrl(Url);

            Assert.AreEqual(n, this.qUrl);
        }
    }
}