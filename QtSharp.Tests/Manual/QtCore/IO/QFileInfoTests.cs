using System.IO;
using System.Linq;
using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore.IO
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

        [Platform(Include = "MacOsX")]
        [Test]
        public void TestBundleNameOnMaxOSX()
        {
            var file = new QFileInfo("/Applications/Safari.app");

            Assert.AreEqual("Safari", file.BundleName);
        }

        [Platform(Exclude = "MacOsX")]
        [Test]
        public void TestBundleNameOnAllExceptMaxOSX()
        {
            var l = _fileInfo.BundleName;
            Assert.AreEqual("", _fileInfo.BundleName);
        }

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

        [Test]
        public void TestDir()
        {
            var parentDir = _fileInfo.Dir;

            var exp = _testFilePath1.Directory.FullName.Replace("\\", "/"); ;

            Assert.AreEqual(exp, parentDir.AbsolutePath);
        }

        [Test]
        public void TestExists()
        {
            Assert.IsTrue(_fileInfo.Exists());
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
        public void TestFileName()
        {
            var q = _fileInfo.FileName;

            var exp = new FileInfo(_testFilePath1.FullName).Name;

            Assert.AreEqual(exp, q);
        }

        [Test]
        public void TestFilePath()
        {
            var q = _fileInfo.FilePath;

            var exp = new FileInfo(_testFilePath1.FullName).FullName;

            Assert.AreEqual(exp.Replace("\\", "/"), q);
        }

        [Test]
        public void TestGroup()
        {
            var q = _fileInfo.Group;

            Assert.AreEqual("", q);
        }

        [Test]
        public void TestGroupId()
        {
            var q = (uint)_fileInfo.GroupId;

            Assert.AreEqual(-2, q);
        }

        [Test]
        public void TestIsAbsolute()
        {
            var isAbs = _fileInfo.IsAbsolute;
            Assert.IsTrue(isAbs);

            var isNotAbs = new QFileInfo("./TestData/TextFile1.txt").IsAbsolute;
            Assert.IsTrue(!isNotAbs);
        }

        [Test]
        [Platform(Include = "MacOsX")]
        public void TestIsBundleOnMaxOSX()
        {
            var file = new QFileInfo("/Applications/Safari.app");

            Assert.IsTrue(file.IsBundle);
        }

        [Test]
        [Platform(Exclude = "MacOsX")]
        public void TestIsBundleOnAllExceptMaxOSX()
        {
            var isBundle = _fileInfo.IsBundle;
            Assert.IsFalse(isBundle);
        }

        [Test]
        public void TestIsDir()
        {
            var ad = _fileInfo.IsDir;

            Assert.IsFalse(ad);
        }

        [Test]
        public void TestIsExecutable()
        {
            var isExe = _fileInfo.IsExecutable;
            Assert.IsFalse(isExe);
        }

        [Test]
        public void TestIsFile()
        {
            var isFile = _fileInfo.IsFile;
            Assert.IsTrue(isFile);
        }

        [Test]
        public void TestIsHidden()
        {
            var isFile = _fileInfo.IsHidden;
            Assert.IsFalse(isFile);
        }

        [Test]
        public void TestIsNativePath()
        {
            var isFile = _fileInfo.IsNativePath;
            Assert.IsTrue(isFile);
        }

        [Test]
        public void TestIsReadable()
        {
            var isFile = _fileInfo.IsReadable;
            Assert.IsTrue(isFile);
        }

        [Test]
        public void TestIsRelative()
        {
            var isAbs = _fileInfo.IsRelative;
            Assert.IsFalse(isAbs);

            var isNotAbs = new QFileInfo("./TestData/TextFile1.txt").IsRelative;
            Assert.IsTrue(isNotAbs);
        }

        [Test]
        public void TestIsRoot()
        {
            var isAbs = _fileInfo.IsRoot;
            Assert.IsFalse(isAbs);
        }

        [Test]
        public void TestIsSymLink()
        {
            var file = new QFileInfo("./TestData/DoubleExtensionVerknüpfung.Ink");

            Assert.IsTrue(file.IsSymLink);
        }

        [Test]
        public void TestIsWritable()
        {
            Assert.IsTrue(_fileInfo.IsWritable);
        }

        [Test]
        public void TestLastModified()
        {
            var qd = _fileInfo.LastModified;
            var qday = qd.Date.Day;
        }

        [Test]
        public void TestLastRead()
        {
            var qd = _fileInfo.LastRead;
            var qday = qd.Date.Day;
        }

        [Test]
        public void TestMakeAbsolute()
        {
            var file = new QFileInfo("./TestData/TextFile1.txt");
            var res = file.MakeAbsolute();

            Assert.IsTrue(res);
        }

        [Test]
        public void TestOwner()
        {
            var file = new QFileInfo("./TestData/TextFile1.txt");
            var res = file.Owner;

            Assert.IsNotNull(res);
        }

        [Test]
        public void TestOwnerId()
        {
            var file = new QFileInfo("./TestData/TextFile1.txt");
            var res = file.OwnerId;

            Assert.IsNotNull(res);
        }

        [Test]
        public void TestPath()
        {
            var file = new QFileInfo("./TestData/TextFile1.txt");
            var res = file.Path;

            Assert.AreEqual("./TestData", res);
        }

        [Test]
        public void TestPermission()
        {
            var file = new QFileInfo("./TestData/TextFile1.txt");
            var res = file.Permission(QFileDevice.Permission.ReadOwner | QFileDevice.Permission.WriteOwner);

            Assert.IsTrue(res);
        }

        [Test]
        public void TestPermissions()
        {
            var file = new QFileInfo("./TestData/TextFile1.txt");
            var res = file.Permissions;

            Assert.IsTrue((res & QFileDevice.Permission.ReadOwner) != 0);
        }

        [Test]
        public void TestRefresh()
        {
            var file = new QFileInfo("./TestData/TextFile1.txt");
            file.Refresh();
        }

        [Test]
        public void TestSetFile()
        {
            var file = new QFileInfo("./TestData/TextFile2_1000words.txt");
            file.SetFile(_fileInfo.FilePath);

            Assert.AreEqual(_fileInfo.FilePath, file.FilePath);
        }

        [Test]
        public void TestSize()
        {
            var file = new QFileInfo("./TestData/TextFile3_50bytes.txt");

            var s = file.Size;

            Assert.AreEqual(53, s);
        }

        [Test]
        public void TestSuffix()
        {
            var file = new QFileInfo("./TestData/DoubleExtension.ext.txt");

            var s = file.Suffix;

            Assert.AreEqual("txt", s);
        }

        [Test]
        public void TestSwap()
        {
            var file = new QFileInfo("./TestData/TextFile1.txt");

            file.Swap(_fileInfo);

            Assert.AreEqual(_fileInfo.AbsoluteFilePath, file.AbsoluteFilePath);
        }

        [Test]
        public void TestSymLinkTarget()
        {
            var file = new QFileInfo("./TestData/DoubleExtensionVerknüpfung.Ink");
            var target = file.SymLinkTarget;

            Assert.AreNotEqual("", target);
        }

        [Test]
        public void TestNotEqualOperator()
        {
            var file = new QFileInfo("./TestData/TextFile2_1000words.txt");

            Assert.AreNotEqual(file, _fileInfo);
        }

        [Test]
        public void TestCopyToAnotherOperator()
        {
            QFileInfo file = _fileInfo;
            Assert.NotNull(file);
        }

        [Test]
        public void TestEqualOperator()
        {
            var file = new QFileInfo("./TestData/TextFile1.txt");

            Assert.AreEqual(file, _fileInfo);
        }
    }
}
