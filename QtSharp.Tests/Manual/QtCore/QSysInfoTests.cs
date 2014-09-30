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
            var s = QSysInfo.WindowsVersion;

            Assert.IsNotNullOrEmpty(s.ToString());
        }

        [Platform(Exclude = "Win,Linux")]
        [Test]
        public void TestMacintoshVersion()
        {
            throw new NotImplementedException("Add in QtCore");

            //var s = QSysInfo.MacintoshVersion;

            //Assert.IsNotNullOrEmpty(s.ToString());
        }
    }
}