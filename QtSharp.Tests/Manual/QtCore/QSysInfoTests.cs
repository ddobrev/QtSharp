using System;
using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore
{
    [TestFixture]
    public class QSysInfoTests
    {
        [Platform(Exclude = "Unix,Linux")]
        [Test]
        public void TestWinVersion()
        {
            var s = QSysInfo.windowsVersion;

            Assert.That(s.ToString(), Is.Not.Null.Or.Empty);
        }

        [Platform(Exclude = "Win,Linux")]
        [Test]
        public void TestMacintoshVersion()
        {
            var s = QSysInfo.macVersion;

            Assert.That(s.ToString(), Is.Not.Null.Or.Empty);
        }
    }
}