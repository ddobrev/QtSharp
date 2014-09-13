using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;
using QtCore;
using QtCore.Qt;

namespace QtSharp.Tests.QtCore
{
    [TestFixture]
    public class QStringRefTests
    {
        private readonly string _testString = System.Reflection.Assembly.GetCallingAssembly().Location;

        private QStringRef _qString;

        [TestFixtureSetUp]
        public void Init()
        {
            // TODO: Add Init code.
            _qString = new QStringRef(_testString);
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            // TODO: Add tear down code.
        }

        [Ignore("Bug")]
        [Test]
        public void TestAppendToStringToQString()
        {
            var old = new QStringRef(_testString);
            var app = "added";

            var appended = old.AppendTo(app);

            var q = appended.ToString();

            var exp = this._testString + app;

            Assert.AreEqual(exp, q);
        }

        [Test]
        public void TestAtToGetQChar()
        {
            for (var j = 0; j < _testString.Count(); j++)
            {
                char net = _testString.ElementAt(0);
                QChar q = _qString.At(0);

                Assert.AreEqual(net.ToString(), q.ToString());
            }
        }

        [Test]
        public void TestClear()
        {
            var s = new QStringRef(_testString);
            s.Clear();

            Assert.True(s.IsEmpty);
        }
        
        private string RandomString(int size)
        {
            var random = new Random((int)DateTime.Now.Ticks);
            var builder = new StringBuilder();
            char ch;

            for (var i = 0; i < size; i++)
            {
                ch = (char)random.Next('0', 'z');
                builder.Append(ch);
            }
            return builder.ToString();
        }

        [Ignore("Bug")]
        [Test]
        public void TestCompareQStringRefAndQStringLatin()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 50);

            var netString1 = RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var r2 = new Random();
            var i2 = r2.Next(0, 50);

            var netString2 = RandomString(i2);
            var qLString2 = new QLatin1String(netString2);

            var netRes = string.Compare(netString1, netString2, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase);
            
            var qRes = QStringRef.Compare(qString1, qLString2, CaseSensitivity.CaseInsensitive);

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
            var netString1 = RandomString(i1);
            var qString1 = new QStringRef(netString1);

            Thread.Sleep(50);

            var r2 = new Random();
            var i2 = r2.Next(0, 60);
            var netString2 = RandomString(i2);
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
            var netString1 = RandomString(i1);
            var qString1 = new QStringRef(netString1);

            Thread.Sleep(50);

            var r2 = new Random();
            var i2 = r2.Next(0, 60);
            var netString2 = RandomString(i2);
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
            var netString1 = RandomString(i1);
            var qString1 = new QStringRef(netString1);

            Thread.Sleep(50);

            var r2 = new Random();
            var i2 = r2.Next(0, 60);
            var netString2 = RandomString(i2);
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
            var netString1 = RandomString(i1);
            var qString1 = new QStringRef(netString1);

            Thread.Sleep(50);

            var r2 = new Random();
            var i2 = r2.Next(0, 60);
            var netString2 = RandomString(i2);
            var qString2 = new QStringRef(netString2);

            var netRes = string.Compare(netString1, netString2, false);

            var qRes = QStringRef.Compare(qString1, netString2, CaseSensitivity.CaseSensitive);

            if (netRes == qRes)
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void TestContainsQStringRef_AString()
        {
            var r1 = new Random();
            var i1 = r1.Next(100, 150);
            var netString1 = RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var r = new Random();
            var i = r.Next(0, 30);

            var look = netString1.Substring(i, 4);

            var net = netString1.Contains(look);
            var q = qString1.Contains(look);

            Assert.AreEqual(net, q);
        }

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
            var charac = _testString.ElementAt(rx);

            var net = Regex.Matches(_testString, @charac.ToString(), RegexOptions.IgnoreCase).Count;

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

        [Ignore("Bug")]
        [Test]
        public void TestEndsWith_WithQStringRefArgs()
        {
            var i = _testString.Last();

            var net = _testString.EndsWith(i.ToString());

            var j = new QStringRef(i.ToString());

            var q = _qString.EndsWith(j);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestEndsWith_WithQCharArgs()
        {
            var i = _testString.Last();

            var net = _testString.EndsWith(i.ToString());

            var j = new QChar(i);

            var q = _qString.EndsWith(j);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestEndsWith_WithQLatinStringArgs()
        {
            var i = _testString.Last();

            var net = _testString.EndsWith(i.ToString());

            var j = new QLatin1String(i.ToString());

            var q = _qString.EndsWith(j);

            Assert.AreEqual(net, q);
        }


        [Test]
        public void TestEndsWith_WithStringArgs()
        {
            var i = _testString.Last();

            var net = _testString.EndsWith(i.ToString());

            var q = _qString.EndsWith(i.ToString());

            Assert.AreEqual(net, q);
        }
        
        [Test]
        public void TestIndexOfQCharInQStringRef()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 60);
            var netString1 = RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var r = new Random();
            var rx = r.Next(0, netString1.Count());
            var charac = netString1.ElementAt(rx);
            var net = netString1.IndexOf(charac);

            var qChar = new QChar(charac);
            var q = qString1.IndexOf(qChar);

            Assert.AreEqual(net, q);
        }

        [Test]
        public void TestLastIndexOfQCharInQStringRef()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 60);
            var netString1 = RandomString(i1);
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
        public void TestLeft()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 50);
            var netString1 = RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var subNet = netString1.Substring(0, i1/5);
            var subQ = qString1.Left(i1 / 5);

            Assert.AreEqual(subNet, subQ.ToString());
        }

        [Ignore("Bug")]
        [Test]
        public void TestLocaleAwareCompare()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 60);
            var netString1 = RandomString(i1);
            var qString1 = new QStringRef(netString1);

            Thread.Sleep(50);

            var r2 = new Random();
            var i2 = r2.Next(0, 60);
            var netString2 = RandomString(i2);
            var qString2 = new QStringRef(netString2);

            var netRes = string.Compare(netString1, netString2);

            var qRes = QStringRef.LocaleAwareCompare(qString1, netString2);

            Assert.AreEqual(netRes, qRes);
        }

        [Test]
        public void TestMid()
        {
            var r1 = new Random();
            var i1 = r1.Next(0, 50);
            var netString1 = RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var subNet = netString1.Substring(i1 / 5);
            var subQ = qString1.Mid(i1 / 5);

            Assert.AreEqual(subNet, subQ.ToString());
        }

        [Test]
        public void TestRight()
        {
            var r1 = new Random();
            var i1 = r1.Next(10, 50);
            var netString1 = RandomString(i1);
            var qString1 = new QStringRef(netString1);

            var subNet = netString1.Substring(netString1.Length - 5);
            var subQ = qString1.Right(netString1.Length - 5);

            Assert.AreEqual(subNet, subQ.ToString());
        }

        [Test]
        public void TestStartsWith_WithStringArgs()
        {
            var i = _testString.Last();

            var net = _testString.StartsWith(i.ToString());

            var q = _qString.StartsWith(i.ToString());

            Assert.AreEqual(net, q);
        }

        [Test]
        public unsafe void TestToDouble()
        {
            var d = 566.15;
            var q = new QStringRef("566.15");
            var w = q.ToDouble();

            Assert.AreEqual(d, w);
        }

        [Test]
        public unsafe void TestToFloat()
        {
            var d = 566.15f;
            var q = new QStringRef("566.15");
            var w = q.ToFloat();

            Assert.AreEqual(d, w);
        }

        [Test]
        public unsafe void TestToInt()
        {
            var d = -566;
            var q = new QStringRef("-566");
            var w = q.ToInt();

            Assert.AreEqual(d, w);
        }

        [Test]
        public unsafe void TestToLong()
        {
            var d = 566;
            var q = new QStringRef("566");
            var w = q.ToLong();

            Assert.AreEqual(d, w);
        }

        [Test]
        public unsafe void TestToLongLong()
        {
            var d = 566;
            var q = new QStringRef("566");
            var w = q.ToLongLong();

            Assert.AreEqual(d, w);
        }

        [Test]
        public unsafe void TestToLatin1()
        {
            var q = new QStringRef("something");
            var w = q.ToLatin1();

            Assert.AreEqual(q.ToString(), w.ToString());
        }

        [Test]
        public void TestQStringToStringAndDotNetStringIfEqual()
        {
            var q = _qString.ToString();
            var exp = this._testString;

            Assert.AreEqual(exp, q);
        }

        [Test]
        public unsafe void TestToUInt()
        {
            var q = new QStringRef("50");
            var w = q.ToUInt();

            Assert.AreEqual(50, w);
        }

        [Test]
        public unsafe void TestToULong()
        {
            var q = new QStringRef("50");
            var w = q.ToULong();

            Assert.AreEqual(50, w);
        }

        [Test]
        public unsafe void TestToULongLong()
        {
            var q = new QStringRef("50");
            var w = q.ToULongLong();

            Assert.AreEqual(50, w);
        }

        [Test]
        public unsafe void TestToUShort()
        {
            var q = new QStringRef("50");
            var w = q.ToUShort();

            Assert.AreEqual(50, w);
        }
    }
}