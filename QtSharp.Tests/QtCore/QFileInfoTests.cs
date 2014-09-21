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
        private readonly FileInfo _testFilePath1 = new FileInfo("./TestData/TextFile1.txt");
        private readonly FileInfo _testFilePath2 = new FileInfo("./TestData/TextFile2_1000words.txt");
        private readonly FileInfo _testFilePath3 = new FileInfo("./TestData/TextFile3_50bytes.txt");
        private readonly FileInfo _testFilePath4 = new FileInfo("./TestData/DoubleExtension.ext.txt");

        private QFileInfo _fileInfo;

        [SetUp]
        public void Init()
        {
            // TODO: Add Init code.
            _fileInfo = new QFileInfo(_testFilePath1.FullName);
        }

        [TearDown]
        public void Dispose()
        {
            // TODO: Add tear down code.
        }

        #region Ctor
        [Test]
        public void TestEmptyConstructorNotThrowingAnException()
        {
            var q = new QFileInfo();
        }

        [Test]
        public void TestStringConstructorNotThrowingAnException()
        {
            var q = new QFileInfo(_testFilePath2.FullName);
        }

        [Test]
        public void TestFileConstructorNotThrowingAnException()
        {
            var f = new QFile(_testFilePath2.FullName);
            var q = new QFileInfo(f);
        }

        [Test]
        public void TestDirConstructorNotThrowingAnException()
        {
            var f = new FileInfo(_testFilePath2.FullName);
            var dir = new QDir(f.Directory.FullName);

            var q = new QFileInfo(dir, f.Name);
        }

        [Test]
        public void TestQFileInfoConstructorNotThrowingAnException()
        {
            var q = new QFileInfo(_fileInfo);
        }
        #endregion

        [Test]
        public void TestAbsoluteDir()
        {
            throw new AssertionException("Warning! Execution deletes all files.");
            var ad = _fileInfo.AbsoluteDir;
            var nad = _testFilePath1.Directory;
        }

        [Test]
        public void TestAbsoluteFilePath()
        {
            var q = _fileInfo.AbsoluteFilePath;
            var nad = _testFilePath1.FullName;

            var exp = nad.Replace("\\", "/");

            Assert.AreEqual(exp, q);
        }

        [Test]
        public void TestAbsolutePath()
        {
            var q = _fileInfo.AbsolutePath;
            var nad = _testFilePath1.Directory.FullName;

            var exp = nad.Replace("\\", "/");

            Assert.AreEqual(exp, q);
        }

        [Test]
        public void TestBaseName()
        {
            var q = _fileInfo.BaseName;
            var exp = _testFilePath1.Name.Remove(_testFilePath1.Name.Count() - _testFilePath1.Extension.Count());

            Assert.AreEqual(exp, q);
        }

        // TODO Add BundleName

        [Test]
        public void TestGetCaching()
        {
            _fileInfo.Caching = true;
            var q = _fileInfo.Caching;

            Assert.IsTrue(q);
        }

        [Test]
        public void TestSetCaching()
        {
            _fileInfo.Caching = false;
            var q = _fileInfo.Caching;

            Assert.IsFalse(q);
        }

        [Test]
        public void TestCanonicalFilePath()
        {
            var q = _fileInfo.CanonicalFilePath;

            var nad = _testFilePath1.FullName;

            var exp = nad.Replace("\\", "/");

            Assert.AreEqual(exp, q);
        }

        [Test]
        public void TestCanonicalPath()
        {
            var q = _fileInfo.CanonicalPath;
            var nad = _testFilePath1.Directory.FullName;

            var exp = nad.Replace("\\", "/");

            Assert.AreEqual(exp, q);
        }

        [Test]
        public void TestCompleteBaseName()
        {
            var q = _fileInfo.CompleteBaseName();
            var exp = _testFilePath1.Name.Remove(_testFilePath1.Name.Count() - _testFilePath1.Extension.Count());

            Assert.AreEqual(exp, q);
        }

        [Test]
        public void TestCompleteSuffix()
        {
            var f = new QFileInfo(_testFilePath4.FullName);
            var q = f.CompleteSuffix();
          
            Assert.AreEqual("ext.txt", q);
        }

        [Test]
        public void TestCreated()
        {
            var q = _fileInfo.Created;
            var net = _testFilePath1.CreationTime;

            Assert.AreEqual(net.Day, q.Date.Day);
            Assert.AreEqual(net.Month, q.Date.Month);
            Assert.AreEqual(net.Year, q.Date.Year);
        }

        // TODO Add Dir, but Warning! Can delete all files!!!

        [Test]
        public void TestExists()
        {
            throw new AssertionException("Not Implemented in QtCoreSharp");
        }

        [Test]
        public void TestStaticExists()
        {
            var res = QFileInfo.Exists(_testFilePath1.FullName);
            Assert.IsTrue(res);

            var res2 = QFileInfo.Exists(_testFilePath1.FullName + "sss");
            Assert.IsFalse(res2);
        }

        [Test]
        public void GetFileName()
        {
            var q = _fileInfo.FileName;

            var exp = new FileInfo(_testFilePath1.FullName).Name;

            Assert.AreEqual(exp, q);
        }
    }
}
