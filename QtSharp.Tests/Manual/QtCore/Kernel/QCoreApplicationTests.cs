using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore.Kernel
{
    [TestFixture]
    public class QCoreApplicationTests
    {
        private QCoreApplication _qCoreApp;

        [SetUp]
        public void Init()
        {
            // TODO: Add Init code.
            _qCoreApp = Helper.CreateQCoreApplicationInstance(new[] { "" });
        }

        [TearDown]
        public void Dispose()
        {
            // TODO: Add tear down code.
            QCoreApplication.Quit();
        }

        [Test]
        public void TestApplicationName()
        {
            var n = QCoreApplication.ApplicationName;

            Assert.IsNotNull(n);
        }

        [Test]
        public void TestApplicationVersion()
        {
            var n = QCoreApplication.ApplicationVersion;

            Assert.IsNotNull(n);
        }

        [Test]
        public void TestOrganizationDomain()
        {
            var n = QCoreApplication.OrganizationDomain;

            Assert.IsNotNull(n);
        }

        [Test]
        public void TestOrganizationName()
        {
            var n = QCoreApplication.OrganizationName;

            Assert.IsNotNull(n);
        }

        [Test]
        public void TestQuitLockEnabled()
        {
            var n = QCoreApplication.QuitLockEnabled;

            Assert.IsNotNull(n);
        }

        [Test]
        public void TestAboutToQuit()
        {
            var wasFired = false;

            _qCoreApp.AboutToQuit += () => wasFired = true;

            QCoreApplication.Quit();

            Assert.IsTrue(wasFired);
        }
    }
}