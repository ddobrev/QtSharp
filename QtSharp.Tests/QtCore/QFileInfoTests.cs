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
    public class QFileInfoTests
    {
        private readonly string _testFilePath = System.Reflection.Assembly.GetCallingAssembly().Location;

        private QFileInfo _fileInfo;

        [SetUp]
        public void Init()
        {
            // TODO: Add Init code.
            _fileInfo = new QFileInfo(_testFilePath);
        }

        [TearDown]
        public void Dispose()
        {
            // TODO: Add tear down code.
        }

        [Test]
        public void GetAbsoluteFilePathAndTestWithDotNetAbsoluteFilePath()
        {
            var q = _fileInfo.AbsoluteFilePath;
            var exp = _testFilePath.Replace("\\", "/");

            Assert.AreEqual(exp, q);
        }

        [Test]
        public void GetFileNameAndTestWithDotNetFileName()
        {
            var q = _fileInfo.FileName;
            var exp = new FileInfo(_testFilePath).Name;

            Assert.AreEqual(exp, q);
        }
    }
}
