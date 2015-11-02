using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using QtCore;
using QtCore.Qt;

namespace QtSharp.Tests.Manual.QtCore.Tools
{
    [TestFixture]
    public class QStringRefTests
    {
        private readonly string _testString = System.Reflection.Assembly.GetCallingAssembly().Location;

        private QStringRef _qString;

        [SetUp]
        public void Init()
        {
            // TODO: Add Init code.
            _qString = new QStringRef(_testString);
        }

        [TearDown]
        public void Dispose()
        {
            // TODO: Add tear down code.
        }

        #region Ctor
        [Test]
        public void TestQStringRefConstructor()
        {
            var s = new QStringRef(_qString);

            Assert.AreEqual(_testString, s.ToString());
        }

        [Test]
        public void TestQStringRefPointerConstructor()
        {
            var s = new QStringRef(_qString);

            Assert.AreEqual(_testString, s.ToString());
        }

        [Test]
        public void TestStringConstructor()
        {
            var s = new QStringRef(_testString);

            Assert.AreEqual(_testString, s.ToString());
        }

        [Test]
        public void TestIntPtrConstructor()
        {
            var s = QStringRef.__CreateInstance(_qString.__Instance);

            Assert.AreEqual(_testString, s.ToString());
        }

        [Test]
        public void TestRangeConstructor()
        {
            var n = _testString.Substring(5, 10);

            var s = new QStringRef(_testString, 5, 10);

            Assert.AreEqual(n, s.ToString());
        }
        #endregion

        #region Append
        [Test]
        public void TestAppendToStringToQString()
        {
            var old = new QStringRef(_testString);
            var app = "added";

            var appended = old.AppendTo(app);

            var q = appended.String;

            var exp = app + this._testString;

            Assert.AreEqual(exp, q);
        }
        #endregion

        #region At
        [Test]
        public void TestAtToGetQChar()
        {
            for (var j = 0; j < this._testString.Length; j++)
            {
                char net = _testString.ElementAt(0);
                QChar q = _qString.At(0);

                Assert.AreEqual(net, q.ToLatin1());
            }
        }
        #endregion

        #region Clear
        [Test]
        public void TestClear()
        {
            var s = new QStringRef(_testString);
            s.Clear();

            Assert.True(s.IsEmpty);
        }
        #endregion

        #region Compare
        [Test]
        public void TestCompareQStringRefAndQStringLatinCaseInsensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 50);

            var netString1 = Helper.RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var r2 = new Random();
            var i2 = r2.Next(0, 50);

            var netString2 = Helper.RandomString(i2);
            var qLString2 = new QLatin1String(netString2);

            var netRes = string.Compare(netString1, netString2, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase);

            var qRes = QStringRef.Compare(qString1, qLString2, CaseSensitivity.CaseInsensitive);

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
            var qString1 = new QStringRef(netString1);

            var r2 = new Random();
            var i2 = r2.Next(0, 50);

            var netString2 = Helper.RandomString(i2);
            var qLString2 = new QLatin1String(netString2);

            var netRes = string.Compare(netString1, netString2);

            var qRes = QStringRef.Compare(qString1, qLString2, CaseSensitivity.CaseSensitive);

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
            var qString1 = new QStringRef(netString1);

            System.Threading.Thread.Sleep(50);

            var r2 = new Random();
            var i2 = r2.Next(0, 60);
            var netString2 = Helper.RandomString(i2);
            var qString2 = new QStringRef(netString2);

            var netRes = string.Compare(netString1, netString2, true);

            var qRes = QStringRef.Compare(qString1, qString2, CaseSensitivity.CaseInsensitive);

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
            var qString1 = new QStringRef(netString1);

            System.Threading.Thread.Sleep(50);

            var r2 = new Random();
            var i2 = r2.Next(0, 60);
            var netString2 = Helper.RandomString(i2);
            var qString2 = new QStringRef(netString2);

            var netRes = string.Compare(netString1, netString2, false);

            var qRes = QStringRef.Compare(qString1, qString2, CaseSensitivity.CaseSensitive);

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
            var qString1 = new QStringRef(netString1);

            System.Threading.Thread.Sleep(50);

            var r2 = new Random();
            var i2 = r2.Next(0, 60);
            var netString2 = Helper.RandomString(i2);
            var qString2 = new QStringRef(netString2);

            var netRes = string.Compare(netString1, netString2, true);

            var qRes = QStringRef.Compare(qString1, netString2, CaseSensitivity.CaseInsensitive);

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
            var qString1 = new QStringRef(netString1);

            System.Threading.Thread.Sleep(50);

            var r2 = new Random();
            var i2 = r2.Next(0, 60);
            var netString2 = Helper.RandomString(i2);
            var qString2 = new QStringRef(netString2);

            var netRes = string.Compare(netString1, netString2, false);

            var qRes = QStringRef.Compare(qString1, netString2, CaseSensitivity.CaseSensitive);

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
            var qString1 = new QStringRef(netString1);

            var r = new Random();
            var i = r.Next(0, 30);

            var look = netString1.Substring(i, 4).ToUpper();

            var net = netString1.Contains(look);
            var qs = new QStringRef(look);
            var q = qString1.Contains(qs);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestContainsQStringRef_A_QStringRefCaseInsensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(100, 150);
            var netString1 = Helper.RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var r = new Random();
            var i = r.Next(0, 30);

            var look = netString1.Substring(i, 4).ToUpper();

            var net = netString1.Contains(look, StringComparison.OrdinalIgnoreCase);
            var qs = new QStringRef(look);
            var q = qString1.Contains(qs, CaseSensitivity.CaseInsensitive);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestContainsQStringRef_A_QCharCaseSensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(100, 150);
            var netString1 = Helper.RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var r = new Random();
            var i = r.Next(0, 30);

            var look = netString1.ElementAt(i);

            var net = netString1.Contains(look);
            var qs = new QChar(look);
            var q = qString1.Contains(qs);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestContainsQStringRef_A_QCharCaseInsensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(100, 150);
            var netString1 = Helper.RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var r = new Random();
            var i = r.Next(0, 30);

            var look = netString1.Substring(i, 1).ToUpper();

            var net = netString1.Contains(look, StringComparison.OrdinalIgnoreCase);
            var qs = new QChar(look[0]);
            var q = qString1.Contains(qs, CaseSensitivity.CaseInsensitive);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestContainsQStringRef_A_QLatin1StringCaseSensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(100, 150);
            var netString1 = Helper.RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var r = new Random();
            var i = r.Next(0, 30);

            var look = netString1.Substring(i, 4);

            var net = netString1.Contains(look);
            var ql = new QLatin1String(look);
            var q = qString1.Contains(ql);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestContainsQStringRef_A_QLatin1StringCaseInsensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(100, 150);
            var netString1 = Helper.RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var r = new Random();
            var i = r.Next(0, 30);

            var look = netString1.Substring(i, 4).ToUpper();

            var net = netString1.Contains(look, StringComparison.OrdinalIgnoreCase);
            var ql = new QLatin1String(look);
            var q = qString1.Contains(ql, CaseSensitivity.CaseInsensitive);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestContainsQStringRef_A_StringCaseSensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(100, 150);
            var netString1 = Helper.RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var r = new Random();
            var i = r.Next(0, 30);

            var look = netString1.Substring(i, 4).ToUpper();

            var net = netString1.Contains(look);
            var q = qString1.Contains(look);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestContainsQStringRef_A_StringCaseInsensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(100, 150);
            var netString1 = Helper.RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var r = new Random();
            var i = r.Next(0, 30);

            var look = netString1.Substring(i, 4).ToUpper();

            var net = netString1.Contains(look, StringComparison.OrdinalIgnoreCase);
            var q = qString1.Contains(look, CaseSensitivity.CaseInsensitive);

            Assert.AreEqual(net, q);
        }
        #endregion

        #region Count
        [Test]
        public void TestCountWithoutArgs()
        {
            var net = _testString.Count();
            var q = _qString.Count();

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestCountWithQCharArgsCaseInsensitive()
        {
            var r = new Random();
            var rx = r.Next(0, _testString.Count());
            var charac = char.ToLowerInvariant(_testString.ElementAt(rx));

            var net = _testString.Count(c => char.ToLowerInvariant(c) == charac);

            var qchar = new QChar(charac);
            var q = _qString.Count(qchar, CaseSensitivity.CaseInsensitive);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestCountWithQCharArgsCaseSensitive()
        {
            var r = new Random();
            var rx = r.Next(0, _testString.Count());
            var charac = _testString.ElementAt(rx);

            var net = _testString.Count(x => x == charac);

            var qchar = new QChar(charac);
            var q = _qString.Count(qchar);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestCountWithStringArgsCaseInsensitive()
        {
            var r = new Random();
            var rx = r.Next(0, _testString.Count());
            var charac = _testString.ElementAt(rx);

            var net = Regex.Matches(_testString, @charac.ToString(), RegexOptions.IgnoreCase).Count;

            var q = _qString.Count(charac.ToString(), CaseSensitivity.CaseInsensitive);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestCountWithStringArgsCaseSensitive()
        {
            var r = new Random();
            var rx = r.Next(0, _testString.Count());
            var charac = _testString.ElementAt(rx);

            var net = _testString.Count(x => x == charac);

            var q = _qString.Count(charac.ToString());

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestCountWithQStringRefArgsCaseInsensitive()
        {
            var r = new Random();
            var rx = r.Next(0, this._testString.Length);
            var charac = char.ToLowerInvariant(_testString.ElementAt(rx));

            var net = _testString.Count(c => char.ToLowerInvariant(c) == charac);

            var qs = new QStringRef(charac.ToString());

            var q = _qString.Count(qs, CaseSensitivity.CaseInsensitive);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestCountWithQStringRefArgsCaseSensitive()
        {
            var r = new Random();
            var rx = r.Next(0, _testString.Count());
            var charac = _testString.ElementAt(rx);

            var net = _testString.Count(x => x == charac);

            var qs = new QStringRef(charac.ToString());

            var q = _qString.Count(qs);

            Assert.AreEqual(net, q);
        }
        #endregion

        #region Ends with
        [Test]
        public void TestEndsWith_WithQStringRefArgsCaseInsensitive()
        {
            var i = _testString.ToUpper().Last();

            var net = _testString.EndsWith(i.ToString(), StringComparison.OrdinalIgnoreCase);

            var j = new QStringRef(i.ToString());

            var q = _qString.EndsWith(j, CaseSensitivity.CaseInsensitive);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestEndsWith_WithQStringRefArgsCaseSensitive()
        {
            var i = _testString.ToUpper().Last();

            var net = _testString.EndsWith(i.ToString());

            var j = new QStringRef(i.ToString());

            var q = _qString.EndsWith(j);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestEndsWith_WithQCharArgsCaseSensitive()
        {
            var i = _testString.Last();

            var net = _testString.EndsWith(i.ToString());

            var j = new QChar(i);

            var q = _qString.EndsWith(j);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestEndsWith_WithQCharArgsCaseInsensitive()
        {
            var i = _testString.ToUpper().Last();

            var net = _testString.EndsWith(i.ToString(), StringComparison.OrdinalIgnoreCase);

            var j = new QChar(i);

            var q = _qString.EndsWith(j, CaseSensitivity.CaseInsensitive);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestEndsWith_WithQLatinStringArgsCaseSensitive()
        {
            var i = _testString.Last();

            var net = _testString.EndsWith(i.ToString());

            var j = new QLatin1String(i.ToString());

            var q = _qString.EndsWith(j);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestEndsWith_WithQLatinStringArgsCaseInsensitive()
        {
            var i = _testString.Last();

            var net = _testString.EndsWith(i.ToString(), StringComparison.OrdinalIgnoreCase);

            var j = new QLatin1String(i.ToString());

            var q = _qString.EndsWith(j, CaseSensitivity.CaseInsensitive);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestEndsWith_WithStringArgsCaseInsensitive()
        {
            var i = _testString.ToUpper().Last();

            var net = _testString.EndsWith(i.ToString(), StringComparison.OrdinalIgnoreCase);

            var q = _qString.EndsWith(i.ToString(), CaseSensitivity.CaseInsensitive);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestEndsWith_WithStringArgsCaseSensitive()
        {
            var i = _testString.Last();

            var net = _testString.EndsWith(i.ToString());

            var q = _qString.EndsWith(i.ToString());

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
            var qString1 = new QStringRef(netString1);

            var r = new Random();
            var rx = r.Next(0, netString1.Count());
            var charac = netString1.ElementAt(rx);
            var net = netString1.IndexOf(new string(charac, 1), StringComparison.OrdinalIgnoreCase);

            var qChar = new QChar(charac);
            var q = qString1.IndexOf(qChar, 0, CaseSensitivity.CaseInsensitive);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestIndexOfQCharInQStringRefCaseInsensitiveWithStartIndex()
        {
            var r1 = new Random();
            var i1 = r1.Next(10, 60);
            var netString1 = Helper.RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var r = new Random();
            var rx = r.Next(0, netString1.Count());
            var charac = netString1.ElementAt(rx);
            var net = netString1.IndexOf(new string(charac, 1), 3, StringComparison.OrdinalIgnoreCase);

            var qChar = new QChar(charac);
            var q = qString1.IndexOf(qChar, 3, CaseSensitivity.CaseInsensitive);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestIndexOfQCharInQStringRefCaseSensitiveWithStartIndex()
        {
            var r1 = new Random();
            var i1 = r1.Next(10, 60);
            var netString1 = Helper.RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var r = new Random();
            var rx = r.Next(0, netString1.Count());
            var charac = netString1.ElementAt(rx);
            var net = netString1.IndexOf(charac, 5);

            var qChar = new QChar(charac);
            var q = qString1.IndexOf(qChar, 5);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestIndexOfQStringRefInQStringRefCaseInsensitiveWithStartIndex()
        {
            var r1 = new Random();
            var i1 = r1.Next(10, 60);
            var netString1 = Helper.RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var r = new Random();
            var rx = r.Next(0, netString1.Count());
            var charac = netString1.ElementAt(rx);
            var net = netString1.IndexOf(new string(charac, 1), 5, StringComparison.Ordinal);

            var qChar = new QStringRef(new string(charac, 1));
            var q = qString1.IndexOf(qChar, 5, CaseSensitivity.CaseInsensitive);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestIndexOfQStringRefInQStringRefCaseSensitive()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 60);
            var netString1 = Helper.RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var r = new Random();
            var rx = r.Next(0, netString1.Count());
            var charac = netString1.ElementAt(rx);
            var net = netString1.IndexOf(charac);

            var qChar = new QStringRef(new string(charac, 1));
            var q = qString1.IndexOf(qChar);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestIndexOfStringRefInQStringRef()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 60);
            var netString1 = Helper.RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var r = new Random();
            var rx = r.Next(0, netString1.Count());
            var charac = netString1.ElementAt(rx);
            var net = netString1.IndexOf(charac);

            var q = qString1.IndexOf(new string(charac, 1));

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
            var qString1 = new QStringRef(netString1);

            var r = new Random();
            var rx = r.Next(0, netString1.Count());
            var charac = netString1.ElementAt(rx);
            var net = netString1.LastIndexOf(charac);

            var qChar = new QChar(charac);
            var q = qString1.LastIndexOf(qChar);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestLastIndexOfQStringRefInQStringRef()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 60);
            var netString1 = Helper.RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var r = new Random();
            var rx = r.Next(0, netString1.Count());
            var charac = netString1.ElementAt(rx);
            var net = netString1.LastIndexOf(charac);

            var qChar = new QStringRef(new string(charac, 1));
            var q = qString1.LastIndexOf(qChar);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestLastIndexOfStringInQStringRef()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 60);
            var netString1 = Helper.RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var r = new Random();
            var rx = r.Next(0, netString1.Count());
            var charac = netString1.ElementAt(rx);
            var net = netString1.LastIndexOf(charac);

            var q = qString1.LastIndexOf(new string(charac, 1));

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
            var qString1 = new QStringRef(netString1);

            var subNet = netString1.Substring(0, i1 / 5);
            var subQ = qString1.Left(i1 / 5);

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
            var qString1 = new QStringRef(netString1);

            System.Threading.Thread.Sleep(50);

            var r2 = new Random();
            var i2 = r2.Next(0, 60);
            var netString2 = Helper.RandomString(i2);
            var qString2 = new QStringRef(netString2);

            var netRes = string.Compare(netString1, netString2);

            var qRes = QStringRef.LocaleAwareCompare(qString1, netString2);

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
            var qString1 = new QStringRef(netString1);

            var subNet = netString1.Substring(i1 / 5);
            var subQ = qString1.Mid(i1 / 5);

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
            var qString1 = new QStringRef(netString1);

            var subNet = netString1.Substring(netString1.Length - 5);
            var subQ = qString1.Right(5);

            Assert.AreEqual(subNet, subQ.ToString());
        }
        #endregion

        #region StartsWith
        [Test]
        public void TestStartsWith_WithStringArgs()
        {
            var i = _testString.Last();

            var net = _testString.StartsWith(i.ToString());

            var q = _qString.StartsWith(i.ToString());

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestStartsWith_WithQStringRefArgs()
        {
            var i = _testString.Last();

            var net = _testString.StartsWith(i.ToString());

            var qs = new QStringRef(new string(i, 1));

            var q = _qString.StartsWith(qs);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestStartsWith_WithQCharArgs()
        {
            var i = _testString.Last();

            var net = _testString.StartsWith(i.ToString());

            var qs = new QChar(i);

            var q = _qString.StartsWith(qs);

            Assert.AreEqual(net, q);
        }
        #endregion

        #region ToDouble
        [Test]
        public unsafe void TestToDouble()
        {
            var d = 566.15;
            var q = new QStringRef("566.15");
            var w = q.ToDouble();

            Assert.AreEqual(d, w);
        }
        #endregion

        #region ToFloat
        [Test]
        public unsafe void TestToFloat()
        {
            var d = 566.15f;
            var q = new QStringRef("566.15");
            var w = q.ToFloat();

            Assert.AreEqual(d, w);
        }
        #endregion

        #region ToInt
        [Test]
        public unsafe void TestToInt()
        {
            var d = -566;
            var q = new QStringRef("-566");
            var w = q.ToInt();

            Assert.AreEqual(d, w);
        }
        #endregion

        #region ToLong
        [Test]
        public unsafe void TestToLong()
        {
            var d = 566;
            var q = new QStringRef("566");
            var w = q.ToLong();

            Assert.AreEqual(d, w);
        }
        #endregion

        #region ToLongLong
        [Test]
        public unsafe void TestToLongLong()
        {
            var d = 566;
            var q = new QStringRef("566");
            var w = q.ToLongLong();

            Assert.AreEqual(d, w);
        }
        #endregion

        #region ToLatin1
        [Test]
        public unsafe void TestToLatin1()
        {
            var q = new QStringRef("something");
            var w = q.ToLatin1();

            Assert.AreEqual(q.ToString(), w.ConstData);
        }
        #endregion

        #region ToLocal8Bit
        [Test]
        public void TestToLocal8Bit()
        {
            var q = new QStringRef("so");
            var w = q.ToLocal8Bit();

            Assert.AreEqual(q.ToString(), w.ConstData);
        }
        #endregion

        #region ToShort
        [Test]
        public unsafe void TestToShort()
        {
            var q = new QStringRef("-50");
            var w = q.ToShort();

            Assert.AreEqual(-50, w);
        }
        #endregion

        #region ToString
        [Test]
        public void TestQStringToStringAndDotNetStringIfEqual()
        {
            var q = _qString.ToString();
            var exp = this._testString;

            Assert.AreEqual(exp, q);
        }
        #endregion

        #region ToUInt
        [Test]
        public unsafe void TestToUInt()
        {
            var q = new QStringRef("50");
            var w = q.ToUInt();

            Assert.AreEqual(50, w);
        }
        #endregion

        #region ToULong
        [Test]
        public unsafe void TestToULong()
        {
            var q = new QStringRef("50");
            var w = q.ToULong();

            Assert.AreEqual(50, w);
        }
        #endregion

        #region ToULongLong
        [Test]
        public unsafe void TestToULongLong()
        {
            var q = new QStringRef("50");
            var w = q.ToULongLong();

            Assert.AreEqual(50, w);
        }
        #endregion

        #region ToUShort
        [Test]
        public unsafe void TestToUShort()
        {
            var q = new QStringRef("50");
            var w = q.ToUShort();

            Assert.AreEqual(50, w);
        }
        #endregion

        #region ToUtf8
        [Test]
        public void TestToUtf8()
        {
            var q = new QStringRef("kklkl");
            var w = q.ToUtf8();

            Assert.AreEqual("kklkl", w.ConstData);
        }
        #endregion

        #region Ops
        [Test]
        public void TestEqual_QStringRef_QLatin1StringOperator()
        {
            var s = _qString;
            var s2 = new QLatin1String(_testString);

            Assert.AreEqual(s, s2);
        }

        [Test]
        public void TestEqual_QStringRef_QStringRefOperator()
        {
            var s = _qString;
            var s2 = new QStringRef(_testString);

            Assert.AreEqual(s, s2);
        }

        [Test]
        public void TestEqual_QStringRef_StringOperator()
        {
            var s = _qString;

            Assert.IsTrue(s == _testString);
        }

        [Test]
        public void TestNotEqual_QString_QLatin1StringOperator()
        {
            var s = _qString;
            var s2 = new QLatin1String(_testString);

            Assert.IsTrue(s != s2);
        }

        [Test]
        public void TestNotEqual_QStringRef_QStringRefOperator()
        {
            var s = _qString;
            var s2 = new QStringRef(_testString + "a");

            Assert.AreNotEqual(s, s2);
        }

        [Test]
        public void TestNotEqual_QStringRef_StringOperator()
        {
            var s = _qString;

            Assert.AreNotEqual(s, _testString);
        }

        [Test]
        public void TestGreaterOperator()
        {
            var s = new QStringRef(_testString);
            var s2 = new QStringRef(_testString.ToUpper());

            Assert.IsTrue(s > s2);
        }

        [Test]
        public void TestGreaterEqualOperator()
        {
            var s = new QStringRef(_testString);
            var s2 = new QStringRef(_testString.ToUpper());

            Assert.IsTrue(s >= s2);
        }

        [Test]
        public void TestLessOperator()
        {
            var s = new QStringRef(_testString);
            var s2 = new QStringRef(_testString.ToUpper());

            Assert.IsTrue(s2 < s);
        }

        [Test]
        public void TestLessEqualOperator()
        {
            var s = new QStringRef(_testString);
            var s2 = new QStringRef(_testString.ToUpper());

            Assert.IsTrue(s2 <= s);
        }

        #endregion

        #region TestUnicode_Data_ConstData
        [Test]
        public void TestUnicode_Data_ConstData()
        {
            var u = _qString.Unicode;
            var d = _qString.Data;
            var cd = _qString.ConstData;

            Assert.AreNotEqual(u.__Instance, IntPtr.Zero);
            Assert.AreNotEqual(d.__Instance, IntPtr.Zero);
            Assert.AreNotEqual(cd.__Instance, IntPtr.Zero);

            Assert.AreEqual(u.Cell, d.Cell);
            Assert.AreEqual(u.Cell, cd.Cell);
        }
        #endregion
    }
}