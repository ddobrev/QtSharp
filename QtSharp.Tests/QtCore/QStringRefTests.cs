using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;
using QtCore;

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

        [Test]
        public void TestQStringToStringAndDotNetStringIfEqual()
        {
            var q = _qString.ToString();
            var exp = this._testString;

            Assert.AreEqual(exp, q);
        }
    }
}