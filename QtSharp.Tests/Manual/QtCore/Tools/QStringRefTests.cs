using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using QtCore;
using QtCore.Qt;

namespace QtSharp.Tests.Manual.QtCore.Tools
{
    [TestFixture]
    public class QStringRefTests
    {
        private readonly string testString = System.Reflection.Assembly.GetCallingAssembly().Location;

        private QStringRef qString;

        [SetUp]
        public void Init()
        {
            this.qString = new QStringRef(this.testString);
        }

        [TearDown]
        public void Dispose()
        {
            this.qString.Dispose();
        }

        #region Ctor
        [Test]
        public void TestQStringRefConstructor()
        {
            using (var s = new QStringRef(this.qString))
            {
                Assert.AreEqual(this.testString, s.ToString());
            }
        }

        [Test]
        public void TestQStringRefPointerConstructor()
        {
            using (var s = new QStringRef(this.qString))
            {
                Assert.AreEqual(this.testString, s.ToString());
            }
        }

        [Test]
        public void TestStringConstructor()
        {
            using (var s = new QStringRef(this.testString))
            {
                Assert.AreEqual(this.testString, s.ToString());
            }
        }

        [Test]
        public void TestIntPtrConstructor()
        {
            var s = QStringRef.__CreateInstance(this.qString.__Instance);

            Assert.AreEqual(this.testString, s.ToString());
        }

        [Test]
        public void TestRangeConstructor()
        {
            var n = this.testString.Substring(5, 10);

            using (var s = new QStringRef(this.testString, 5, 10))
            {
                Assert.AreEqual(n, s.ToString());
            }
        }
        #endregion

        #region Append
        [Test]
        public void TestAppendToStringToQString()
        {
            string app;
            QStringRef appended;
            using (var old = new QStringRef(this.testString))
            {
                app = "added";

                appended = old.AppendTo(app);
            }

            var q = appended.String;

            var exp = app + this.testString;

            Assert.AreEqual(exp, q);
        }
        #endregion

        #region At
        [Test]
        public void TestAtToGetQChar()
        {
            for (var j = 0; j < this.testString.Length; j++)
            {
                char net = this.testString[j];
                QChar q = this.qString.At(j);

                Assert.AreEqual(net, q.ToLatin1());
            }
        }
        #endregion

        #region Clear
        [Test]
        public void TestClear()
        {
            using (var s = new QStringRef(this.testString))
            {
                s.Clear();

                Assert.True(s.IsEmpty);
            }
        }
        #endregion

        #region Compare
        [Test]
        public void TestCompareQStringRefAndQStringLatinCaseInsensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 50);

            var netString1 = Helper.RandomString(i1);
            int netRes;
            int qRes;
            using (var qString1 = new QStringRef(netString1))
            {
                var r2 = new Random();
                var i2 = r2.Next(0, 50);

                var netString2 = Helper.RandomString(i2);
                using (var qLString2 = new QLatin1String(netString2))
                {
                    netRes = string.Compare(netString1, netString2, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase);

                    qRes = QStringRef.Compare(qString1, qLString2, CaseSensitivity.CaseInsensitive);
                }
            }

            if (netRes == qRes)
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void TestCompareQStringRefAndQStringLatinCaseSensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 50);

            var netString1 = Helper.RandomString(i1);
            int netRes;
            int qRes;
            using (var qString1 = new QStringRef(netString1))
            {
                var r2 = new Random();
                var i2 = r2.Next(0, 50);

                var netString2 = Helper.RandomString(i2);
                using (var qLString2 = new QLatin1String(netString2))
                {
                    netRes = string.CompareOrdinal(netString1, netString2);

                    qRes = QStringRef.Compare(qString1, qLString2);
                }
            }

            if (netRes == qRes)
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void TestCompareQStringRefAndQStringRefCaseInsensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 60);
            var netString1 = Helper.RandomString(i1);
            int netRes;
            int qRes;
            using (var qString1 = new QStringRef(netString1))
            {
                System.Threading.Thread.Sleep(50);

                var r2 = new Random();
                var i2 = r2.Next(0, 60);
                var netString2 = Helper.RandomString(i2);
                using (var qString2 = new QStringRef(netString2))
                {
                    netRes = string.Compare(netString1, netString2, StringComparison.OrdinalIgnoreCase);

                    qRes = QStringRef.Compare(qString1, qString2, CaseSensitivity.CaseInsensitive);
                }
            }

            if (netRes == qRes)
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void TestCompareQStringRefAndQStringRefCaseSensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 60);
            var netString1 = Helper.RandomString(i1);
            int netRes;
            int qRes;
            using (var qString1 = new QStringRef(netString1))
            {
                System.Threading.Thread.Sleep(50);

                var r2 = new Random();
                var i2 = r2.Next(0, 60);
                var netString2 = Helper.RandomString(i2);
                using (var qString2 = new QStringRef(netString2))
                {
                    netRes = string.CompareOrdinal(netString1, netString2);

                    qRes = QStringRef.Compare(qString1, qString2);
                }
            }

            if (netRes == qRes)
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void TestCompareQStringRefAndStringCaseInsensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 60);
            var netString1 = Helper.RandomString(i1);
            int netRes;
            int qRes;
            using (var qString1 = new QStringRef(netString1))
            {
                System.Threading.Thread.Sleep(50);

                var r2 = new Random();
                var i2 = r2.Next(0, 60);
                var netString2 = Helper.RandomString(i2);

                netRes = string.Compare(netString1, netString2, StringComparison.OrdinalIgnoreCase);

                qRes = QStringRef.Compare(qString1, netString2, CaseSensitivity.CaseInsensitive);
            }

            if (netRes == qRes)
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void TestCompareQStringRefAndStringCaseSensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 60);
            var netString1 = Helper.RandomString(i1);
            int netRes;
            int qRes;
            using (var qString1 = new QStringRef(netString1))
            {
                System.Threading.Thread.Sleep(50);

                var r2 = new Random();
                var i2 = r2.Next(0, 60);
                var netString2 = Helper.RandomString(i2);

                netRes = string.CompareOrdinal(netString1, netString2);

                qRes = QStringRef.Compare(qString1, netString2);
            }

            if (netRes == qRes)
            {
                Assert.IsTrue(true);
            }
        }
        #endregion

        #region Contains
        [Test]
        public void TestContainsQStringRef_A_QStringRefCaseSensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(100, 150);
            var netString1 = Helper.RandomString(i1);
            bool net;
            bool q;
            using (var qString1 = new QStringRef(netString1))
            {
                var r = new Random();
                var i = r.Next(0, 30);

                var look = netString1.Substring(i, 4).ToUpper();

                net = netString1.Contains(look);
                using (var qs = new QStringRef(look))
                {
                    q = qString1.Contains(qs);
                }
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestContainsQStringRef_A_QStringRefCaseInsensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(100, 150);
            var netString1 = Helper.RandomString(i1);
            bool net;
            bool q;
            using (var qString1 = new QStringRef(netString1))
            {
                var r = new Random();
                var i = r.Next(0, 30);

                var look = netString1.Substring(i, 4).ToUpper();

                net = netString1.Contains(look, StringComparison.OrdinalIgnoreCase);
                using (var qs = new QStringRef(look))
                {
                    q = qString1.Contains(qs, CaseSensitivity.CaseInsensitive);
                }
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestContainsQStringRef_A_QCharCaseSensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(100, 150);
            var netString1 = Helper.RandomString(i1);
            bool net;
            bool q;
            using (var qString1 = new QStringRef(netString1))
            {
                var r = new Random();
                var i = r.Next(0, 30);

                var look = netString1.ElementAt(i);

                net = netString1.Contains(look);
                var qs = new QChar(look);
                q = qString1.Contains(qs);
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestContainsQStringRef_A_QCharCaseInsensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(100, 150);
            var netString1 = Helper.RandomString(i1);
            bool net;
            bool q;
            using (var qString1 = new QStringRef(netString1))
            {
                var r = new Random();
                var i = r.Next(0, 30);

                var look = netString1.Substring(i, 1).ToUpper();

                net = netString1.Contains(look, StringComparison.OrdinalIgnoreCase);
                var qs = new QChar(look[0]);
                q = qString1.Contains(qs, CaseSensitivity.CaseInsensitive);
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestContainsQStringRef_A_QLatin1StringCaseSensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(100, 150);
            var netString1 = Helper.RandomString(i1);
            bool net;
            bool q;
            using (var qString1 = new QStringRef(netString1))
            {
                var r = new Random();
                var i = r.Next(0, 30);

                var look = netString1.Substring(i, 4);

                net = netString1.Contains(look);
                using (var ql = new QLatin1String(look))
                {
                    q = qString1.Contains(ql);
                }
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestContainsQStringRef_A_QLatin1StringCaseInsensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(100, 150);
            var netString1 = Helper.RandomString(i1);
            bool net;
            bool q;
            using (var qString1 = new QStringRef(netString1))
            {
                var r = new Random();
                var i = r.Next(0, 30);

                var look = netString1.Substring(i, 4).ToUpper();

                net = netString1.Contains(look, StringComparison.OrdinalIgnoreCase);
                using (var ql = new QLatin1String(look))
                {
                    q = qString1.Contains(ql, CaseSensitivity.CaseInsensitive);
                }
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestContainsQStringRef_A_StringCaseSensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(100, 150);
            var netString1 = Helper.RandomString(i1);
            bool net;
            bool q;
            using (var qString1 = new QStringRef(netString1))
            {
                var r = new Random();
                var i = r.Next(0, 30);

                var look = netString1.Substring(i, 4).ToUpper();

                net = netString1.Contains(look);
                q = qString1.Contains(look);
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestContainsQStringRef_A_StringCaseInsensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(100, 150);
            var netString1 = Helper.RandomString(i1);
            bool net;
            bool q;
            using (var qString1 = new QStringRef(netString1))
            {
                var r = new Random();
                var i = r.Next(0, 30);

                var look = netString1.Substring(i, 4).ToUpper();

                net = netString1.Contains(look, StringComparison.OrdinalIgnoreCase);
                q = qString1.Contains(look, CaseSensitivity.CaseInsensitive);
            }

            Assert.AreEqual(net, q);
        }
        #endregion

        #region Count
        [Test]
        public void TestCountWithoutArgs()
        {
            var net = this.testString.Length;
            var q = this.qString.Count();

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestCountWithQCharArgsCaseInsensitive()
        {
            var r = new Random();
            var rx = r.Next(0, this.testString.Length);
            var charac = char.ToLowerInvariant(this.testString.ElementAt(rx));

            var net = this.testString.Count(c => char.ToLowerInvariant(c) == charac);

            int q;
            using (var qchar = new QChar(charac))
            {
                q = this.qString.Count(qchar, CaseSensitivity.CaseInsensitive);
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestCountWithQCharArgsCaseSensitive()
        {
            var r = new Random();
            var rx = r.Next(0, this.testString.Length);
            var charac = this.testString.ElementAt(rx);

            var net = this.testString.Count(x => x == charac);

            int q;
            using (var qchar = new QChar(charac))
            {
                q = this.qString.Count(qchar);
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestCountWithStringArgsCaseInsensitive()
        {
            var r = new Random();
            var rx = r.Next(0, this.testString.Length);
            var charac = this.testString.ElementAt(rx);

            var net = Regex.Matches(this.testString, @charac.ToString(), RegexOptions.IgnoreCase).Count;

            var q = this.qString.Count(charac.ToString(), CaseSensitivity.CaseInsensitive);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestCountWithStringArgsCaseSensitive()
        {
            var r = new Random();
            var rx = r.Next(0, this.testString.Length);
            var charac = this.testString.ElementAt(rx);

            var net = this.testString.Count(x => x == charac);

            var q = this.qString.Count(charac.ToString());

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestCountWithQStringRefArgsCaseInsensitive()
        {
            var r = new Random();
            var rx = r.Next(0, this.testString.Length);
            var charac = char.ToLowerInvariant(this.testString.ElementAt(rx));

            var net = this.testString.Count(c => char.ToLowerInvariant(c) == charac);

            int q;
            using (var qs = new QStringRef(charac.ToString()))
            {
                q = this.qString.Count(qs, CaseSensitivity.CaseInsensitive);
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestCountWithQStringRefArgsCaseSensitive()
        {
            var r = new Random();
            var rx = r.Next(0, this.testString.Length);
            var charac = this.testString.ElementAt(rx);

            var net = this.testString.Count(x => x == charac);

            int q;
            using (var qs = new QStringRef(charac.ToString()))
            {
                q = this.qString.Count(qs);
            }

            Assert.AreEqual(net, q);
        }
        #endregion

        #region Ends with
        [Test]
        public void TestEndsWith_WithQStringRefArgsCaseInsensitive()
        {
            var i = this.testString.ToUpper().Last();

            var net = this.testString.EndsWith(i.ToString(), StringComparison.OrdinalIgnoreCase);

            bool q;
            using (var j = new QStringRef(i.ToString()))
            {
                q = this.qString.EndsWith(j, CaseSensitivity.CaseInsensitive);
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestEndsWith_WithQStringRefArgsCaseSensitive()
        {
            var i = this.testString.ToUpper().Last();

            var net = this.testString.EndsWith(i.ToString());

            bool q;
            using (var j = new QStringRef(i.ToString()))
            {
                q = this.qString.EndsWith(j);
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestEndsWith_WithQCharArgsCaseSensitive()
        {
            var i = this.testString.Last();

            var net = this.testString.EndsWith(i.ToString());

            bool q;
            using (var j = new QChar(i))
            {
                q = this.qString.EndsWith(j);
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestEndsWith_WithQCharArgsCaseInsensitive()
        {
            var i = this.testString.ToUpper().Last();

            var net = this.testString.EndsWith(i.ToString(), StringComparison.OrdinalIgnoreCase);

            bool q;
            using (var j = new QChar(i))
            {
                q = this.qString.EndsWith(j, CaseSensitivity.CaseInsensitive);
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestEndsWith_WithQLatinStringArgsCaseSensitive()
        {
            var i = this.testString.Last();

            var net = this.testString.EndsWith(i.ToString());

            bool q;
            using (var j = new QLatin1String(i.ToString()))
            {
                q = this.qString.EndsWith(j);
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestEndsWith_WithQLatinStringArgsCaseInsensitive()
        {
            var i = this.testString.Last();

            var net = this.testString.EndsWith(i.ToString(), StringComparison.OrdinalIgnoreCase);

            bool q;
            using (var j = new QLatin1String(i.ToString()))
            {
                q = this.qString.EndsWith(j, CaseSensitivity.CaseInsensitive);
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestEndsWith_WithStringArgsCaseInsensitive()
        {
            var i = this.testString.ToUpper().Last();

            var net = this.testString.EndsWith(i.ToString(), StringComparison.OrdinalIgnoreCase);

            var q = this.qString.EndsWith(i.ToString(), CaseSensitivity.CaseInsensitive);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestEndsWith_WithStringArgsCaseSensitive()
        {
            var i = this.testString.Last();

            var net = this.testString.EndsWith(i.ToString());

            var q = this.qString.EndsWith(i.ToString());

            Assert.AreEqual(net, q);
        }
        #endregion

        #region Indexof
        [Test]
        public void TestIndexOfQCharInQStringRefCaseInsensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 60);
            var netString1 = Helper.RandomString(i1);
            int net;
            int q;
            using (var qString1 = new QStringRef(netString1))
            {
                var r = new Random();
                var rx = r.Next(0, netString1.Length);
                var charac = netString1.ElementAt(rx);
                net = netString1.IndexOf(new string(charac, 1), StringComparison.OrdinalIgnoreCase);

                using (var qChar = new QChar(charac))
                {
                    q = qString1.IndexOf(qChar, 0, CaseSensitivity.CaseInsensitive);
                }
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestIndexOfQCharInQStringRefCaseInsensitiveWithStartIndex()
        {
            var r1 = new Random();
            var i1 = r1.Next(10, 60);
            var netString1 = Helper.RandomString(i1);
            int net;
            int q;
            using (var qString1 = new QStringRef(netString1))
            {
                var r = new Random();
                var rx = r.Next(0, netString1.Length);
                var charac = netString1.ElementAt(rx);
                net = netString1.IndexOf(new string(charac, 1), 3, StringComparison.OrdinalIgnoreCase);

                using (var qChar = new QChar(charac))
                {
                    q = qString1.IndexOf(qChar, 3, CaseSensitivity.CaseInsensitive);
                }
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestIndexOfQCharInQStringRefCaseSensitiveWithStartIndex()
        {
            var r1 = new Random();
            var i1 = r1.Next(10, 60);
            var netString1 = Helper.RandomString(i1);
            int net;
            int q;
            using (var qString1 = new QStringRef(netString1))
            {
                var r = new Random();
                var rx = r.Next(0, netString1.Length);
                var charac = netString1.ElementAt(rx);
                net = netString1.IndexOf(charac, 5);

                using (var qChar = new QChar(charac))
                {
                    q = qString1.IndexOf(qChar, 5);
                }
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestIndexOfQStringRefInQStringRefCaseInsensitiveWithStartIndex()
        {
            var r1 = new Random();
            var i1 = r1.Next(10, 60);
            var netString1 = Helper.RandomString(i1);
            int net;
            int q;
            using (var qString1 = new QStringRef(netString1))
            {
                var r = new Random();
                var rx = r.Next(0, netString1.Length);
                var charac = netString1.ElementAt(rx);
                net = netString1.IndexOf(new string(charac, 1), 5, StringComparison.Ordinal);

                using (var qChar = new QStringRef(new string(charac, 1)))
                {
                    q = qString1.IndexOf(qChar, 5, CaseSensitivity.CaseInsensitive);
                }
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestIndexOfQStringRefInQStringRefCaseSensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 60);
            var netString1 = Helper.RandomString(i1);
            int net;
            int q;
            using (var qString1 = new QStringRef(netString1))
            {
                var r = new Random();
                var rx = r.Next(0, netString1.Length);
                var charac = netString1.ElementAt(rx);
                net = netString1.IndexOf(charac);

                using (var qChar = new QStringRef(new string(charac, 1)))
                {
                    q = qString1.IndexOf(qChar);
                }
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestIndexOfStringRefInQStringRef()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 60);
            var netString1 = Helper.RandomString(i1);
            int net;
            int q;
            using (var qString1 = new QStringRef(netString1))
            {
                var r = new Random();
                var rx = r.Next(0, netString1.Length);
                var charac = netString1.ElementAt(rx);
                net = netString1.IndexOf(charac);

                q = qString1.IndexOf(new string(charac, 1));
            }

            Assert.AreEqual(net, q);
        }
        #endregion

        #region LastIndexOf
        [Test]
        public void TestLastIndexOfQCharInQStringRef()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 60);
            var netString1 = Helper.RandomString(i1);
            int net;
            int q;
            using (var qString1 = new QStringRef(netString1))
            {
                var r = new Random();
                var rx = r.Next(0, netString1.Length);
                var charac = netString1.ElementAt(rx);
                net = netString1.LastIndexOf(charac);

                using (var qChar = new QChar(charac))
                {
                    q = qString1.LastIndexOf(qChar);
                }
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestLastIndexOfQStringRefInQStringRef()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 60);
            var netString1 = Helper.RandomString(i1);
            int net;
            int q;
            using (var qString1 = new QStringRef(netString1))
            {
                var r = new Random();
                var rx = r.Next(0, netString1.Length);
                var charac = netString1.ElementAt(rx);
                net = netString1.LastIndexOf(charac);

                using (var qChar = new QStringRef(new string(charac, 1)))
                {
                    q = qString1.LastIndexOf(qChar);
                }
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestLastIndexOfStringInQStringRef()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 60);
            var netString1 = Helper.RandomString(i1);
            int net;
            int q;
            using (var qString1 = new QStringRef(netString1))
            {
                var r = new Random();
                var rx = r.Next(0, netString1.Length);
                var charac = netString1.ElementAt(rx);
                net = netString1.LastIndexOf(charac);

                q = qString1.LastIndexOf(new string(charac, 1));
            }

            Assert.AreEqual(net, q);
        }
        #endregion

        #region Left
        [Test]
        public void TestLeft()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 50);
            var netString1 = Helper.RandomString(i1);
            string subNet;
            QStringRef subQ;
            using (var qString1 = new QStringRef(netString1))
            {
                subNet = netString1.Substring(0, i1 / 5);
                subQ = qString1.Left(i1 / 5);
            }

            Assert.AreEqual(subNet, subQ.ToString());
        }
        #endregion

        #region LocaleAwareCompare
        [Test]
        public void TestLocaleAwareCompare()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 60);
            var netString1 = Helper.RandomString(i1);
            int netRes;
            int qRes;
            using (var qString1 = new QStringRef(netString1))
            {
                System.Threading.Thread.Sleep(50);

                var r2 = new Random();
                var i2 = r2.Next(0, 60);
                var netString2 = Helper.RandomString(i2);

                netRes = string.Compare(netString1, netString2, StringComparison.CurrentCulture);

                qRes = QStringRef.LocaleAwareCompare(qString1, netString2);
            }

            Assert.AreEqual(netRes, qRes);
        }
        #endregion

        #region Mid
        [Test]
        public void TestMid()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 50);
            var netString1 = Helper.RandomString(i1);
            string subNet;
            QStringRef subQ;
            using (var qString1 = new QStringRef(netString1))
            {
                subNet = netString1.Substring(i1 / 5);
                subQ = qString1.Mid(i1 / 5);
            }

            Assert.AreEqual(subNet, subQ.ToString());
        }
        #endregion

        #region Right
        [Test]
        public void TestRight()
        {
            var r1 = new Random();
            var i1 = r1.Next(10, 50);
            var netString1 = Helper.RandomString(i1);
            string subNet;
            QStringRef subQ;
            using (var qString1 = new QStringRef(netString1))
            {
                subNet = netString1.Substring(netString1.Length - 5);
                subQ = qString1.Right(5);
            }

            Assert.AreEqual(subNet, subQ.ToString());
        }
        #endregion

        #region StartsWith
        [Test]
        public void TestStartsWith_WithStringArgs()
        {
            var i = this.testString.Last();

            var net = this.testString.StartsWith(i.ToString());

            var q = this.qString.StartsWith(i.ToString());

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestStartsWith_WithQStringRefArgs()
        {
            var i = this.testString.Last();

            var net = this.testString.StartsWith(i.ToString());

            bool q;
            using (var qs = new QStringRef(new string(i, 1)))
            {
                q = this.qString.StartsWith(qs);
            }

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestStartsWith_WithQCharArgs()
        {
            var i = this.testString.Last();

            var net = this.testString.StartsWith(i.ToString());

            bool q;
            using (var qs = new QChar(i))
            {
                q = this.qString.StartsWith(qs);
            }

            Assert.AreEqual(net, q);
        }
        #endregion

        #region ToDouble
        [Test]
        public void TestToDouble()
        {
            var d = 566.15;
            double w;
            using (var q = new QStringRef("566.15"))
            {
                w = q.ToDouble();
            }

            Assert.AreEqual(d, w);
        }
        #endregion

        #region ToFloat
        [Test]
        public void TestToFloat()
        {
            var d = 566.15f;
            float w;
            using (var q = new QStringRef("566.15"))
            {
                w = q.ToFloat();
            }

            Assert.AreEqual(d, w);
        }
        #endregion

        #region ToInt
        [Test]
        public void TestToInt()
        {
            var d = -566;
            int w;
            using (var q = new QStringRef("-566"))
            {
                w = q.ToInt();
            }

            Assert.AreEqual(d, w);
        }
        #endregion

        #region ToLong
        [Test]
        public void TestToLong()
        {
            var d = 566;
            int w;
            using (var q = new QStringRef("566"))
            {
                w = q.ToLong();
            }

            Assert.AreEqual(d, w);
        }
        #endregion

        #region ToLongLong
        [Test]
        public void TestToLongLong()
        {
            var d = 566;
            long w;
            using (var q = new QStringRef("566"))
            {
                w = q.ToLongLong();
            }

            Assert.AreEqual(d, w);
        }
        #endregion

        #region ToLatin1
        [Test]
        public void TestToLatin1()
        {
            using (var q = new QStringRef("something"))
            {
                var w = q.ToLatin1();

                Assert.AreEqual(q.ToString(), w.ConstData);
            }
        }
        #endregion

        #region ToLocal8Bit
        [Test]
        public void TestToLocal8Bit()
        {
            using (var q = new QStringRef("so"))
            {
                var w = q.ToLocal8Bit();

                Assert.AreEqual(q.ToString(), w.ConstData);
            }
        }
        #endregion

        #region ToShort
        [Test]
        public void TestToShort()
        {
            short w;
            using (var q = new QStringRef("-50"))
            {
                w = q.ToShort();
            }

            Assert.AreEqual(-50, w);
        }
        #endregion

        #region ToString
        [Test]
        public void TestQStringToStringAndDotNetStringIfEqual()
        {
            var q = this.qString.ToString();
            var exp = this.testString;

            Assert.AreEqual(exp, q);
        }
        #endregion

        #region ToUInt
        [Test]
        public void TestToUInt()
        {
            uint w;
            using (var q = new QStringRef("50"))
            {
                w = q.ToUInt();
            }

            Assert.AreEqual(50, w);
        }
        #endregion

        #region ToULong
        [Test]
        public void TestToULong()
        {
            uint w;
            using (var q = new QStringRef("50"))
            {
                w = q.ToULong();
            }

            Assert.AreEqual(50, w);
        }
        #endregion

        #region ToULongLong
        [Test]
        public void TestToULongLong()
        {
            ulong w;
            using (var q = new QStringRef("50"))
            {
                w = q.ToULongLong();
            }

            Assert.AreEqual(50, w);
        }
        #endregion

        #region ToUShort
        [Test]
        public void TestToUShort()
        {
            ushort w;
            using (var q = new QStringRef("50"))
            {
                w = q.ToUShort();
            }

            Assert.AreEqual(50, w);
        }
        #endregion

        #region ToUtf8
        [Test]
        public void TestToUtf8()
        {
            QByteArray w;
            using (var q = new QStringRef("kklkl"))
            {
                w = q.ToUtf8();
            }

            Assert.AreEqual("kklkl", w.ConstData);
        }
        #endregion

        #region Ops
        [Test]
        public void TestEqual_QStringRef_QLatin1StringOperator()
        {
            var s = this.qString;
            using (var s2 = new QLatin1String(this.testString))
            {
                Assert.AreEqual(s, s2);
            }
        }

        [Test]
        public void TestEqual_QStringRef_QStringRefOperator()
        {
            var s = this.qString;
            using (var s2 = new QStringRef(this.testString))
            {
                Assert.AreEqual(s, s2);
            }
        }

        [Test]
        public void TestEqual_QStringRef_StringOperator()
        {
            var s = this.qString;

            Assert.IsTrue(s == this.testString);
        }

        [Test]
        public void TestNotEqual_QString_QLatin1StringOperator()
        {
            var s = this.qString;
            using (var s2 = new QLatin1String(this.testString))
            {
                Assert.IsTrue(s != s2);
            }
        }

        [Test]
        public void TestNotEqual_QStringRef_QStringRefOperator()
        {
            var s = this.qString;
            using (var s2 = new QStringRef(this.testString + "a"))
            {
                Assert.AreNotEqual(s, s2);
            }
        }

        [Test]
        public void TestNotEqual_QStringRef_StringOperator()
        {
            var s = this.qString;

            Assert.AreNotEqual(s, this.testString);
        }

        [Test]
        public void TestGreaterOperator()
        {
            using (var s = new QStringRef(this.testString))
            {
                using (var s2 = new QStringRef(this.testString.ToUpper()))
                {
                    Assert.IsTrue(s > s2);
                }
            }
        }

        [Test]
        public void TestGreaterEqualOperator()
        {
            using (var s = new QStringRef(this.testString))
            {
                using (var s2 = new QStringRef(this.testString.ToUpper()))
                {
                    Assert.IsTrue(s >= s2);
                }
            }
        }

        [Test]
        public void TestLessOperator()
        {
            using (var s = new QStringRef(this.testString))
            {
                using (var s2 = new QStringRef(this.testString.ToUpper()))
                {
                    Assert.IsTrue(s2 < s);
                }
            }
        }

        [Test]
        public void TestLessEqualOperator()
        {
            using (var s = new QStringRef(this.testString))
            {
                using (var s2 = new QStringRef(this.testString.ToUpper()))
                {
                    Assert.IsTrue(s2 <= s);
                }
            }
        }

        #endregion

        #region TestUnicode_Data_ConstData
        [Test]
        public void TestUnicode_Data_ConstData()
        {
            var u = this.qString.Unicode;
            var d = this.qString.Data;
            var cd = this.qString.ConstData;

            Assert.AreNotEqual(u.__Instance, IntPtr.Zero);
            Assert.AreNotEqual(d.__Instance, IntPtr.Zero);
            Assert.AreNotEqual(cd.__Instance, IntPtr.Zero);

            Assert.AreEqual(u.Cell, d.Cell);
            Assert.AreEqual(u.Cell, cd.Cell);
        }
        #endregion
    }
}