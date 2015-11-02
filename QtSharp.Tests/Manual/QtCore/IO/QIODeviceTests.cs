using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore.IO
{
    [TestFixture]
    public class QIODeviceTests
    {
        [SetUp]
        public void Init()
        {
        }

        [TearDown]
        public void Dispose()
        {
        }

        [Test]
        public void TestOpenMode()
        {
            var obj = new MyIODevice();
            obj.SetOpenMode(QIODevice.OpenModeFlag.NotOpen);
            Assert.AreEqual(QIODevice.OpenModeFlag.NotOpen, obj.OpenMode);

            obj.SetOpenMode(QIODevice.OpenModeFlag.ReadWrite);
            Assert.AreEqual(QIODevice.OpenModeFlag.ReadWrite, obj.OpenMode);
            // BUG: calling Dispose here causes a crash - the original function pointer is null
        }

        public class MyIODevice : QIODevice 
        {
            public void SetOpenMode(OpenModeFlag openMode)
            {
                base.OpenMode = openMode;
            }

            protected unsafe override long ReadData(char* data, long maxlen)
            {
                throw new System.NotImplementedException();
            }

            protected override long WriteData(string data, long len)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}