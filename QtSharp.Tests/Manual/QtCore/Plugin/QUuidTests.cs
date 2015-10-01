using System;
using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore.Plugin
{
    [TestFixture]
    public class QUuidTests
    {
        QUuid _uuidNs;
        QUuid _uuidA;
        QUuid _uuidB;
        QUuid _uuidC;
        QUuid _uuidD;

        [SetUp]
        public void Init()
        {
            _uuidNs = new QUuid("{6ba7b810-9dad-11d1-80b4-00c04fd430c8}"); // _uuidNs = new QUuid(0x6ba7b810, 0x9dad, 0x11d1, 0x80, 0xb4, 0x00, 0xc0, 0x4f, 0xd4, 0x30, 0xc8);
            _uuidA = new QUuid("{fc69b59e-cc34-4436-a43c-ee95d128b8c5}"); // _uuidA = new QUuid(0xfc69b59e, 0xcc34, 0x4436, 0xa4, 0x3c, 0xee, 0x95, 0xd1, 0x28, 0xb8, 0xc5);
            _uuidB = new QUuid("{1ab6e93a-b1cb-4a87-ba47-ec7e99039a7b}"); // _uuidB = new QUuid(0x1ab6e93a, 0xb1cb, 0x4a87, 0xba, 0x47, 0xec, 0x7e, 0x99, 0x03, 0x9a, 0x7b);
            _uuidC = new QUuid("{3d813cbb-47fb-32ba-91df-831e1593ac29}"); // _uuidC = new QUuid(0x3d813cbb, 0x47fb, 0x32ba, 0x91, 0xdf, 0x83, 0x1e, 0x15, 0x93, 0xac, 0x29);
            _uuidD = new QUuid("{21f7f8de-8051-5b89-8680-0195ef798b6a}"); // _uuidD = new QUuid(0x21f7f8de, 0x8051, 0x5b89, 0x86, 0x80, 0x01, 0x95, 0xef, 0x79, 0x8b, 0x6a);
        }

        [TearDown]
        public void Dispose()
        {
            // TODO: Add tear down code.
        }

        [Test]
        public void TestEmpyConstructor()
        {
            var uid = new QUuid();
            Assert.AreEqual(Guid.Empty.ToString(), uid.ToString().Trim('{', '}'));
        }

        [Test]
        public void TestHexConstructor()
        {
            uint l = 0x67c8770b;
            ushort w1 = 0x44f1;
            ushort w2 = 0x410a;
            byte b1 = 0xab;
            byte b2 = 0x9a;
            byte b3 = 0xf9;
            byte b4 = 0xb5;
            byte b5 = 0x44;
            byte b6 = 0x6f;
            byte b7 = 0x13;
            byte b8 = 0xee;

            var uid = new QUuid(l, w1, w2, b1, b2, b3, b4, b5, b6, b7, b8);

            throw new AssertionException("Hex ctor not implemented!");
        }

        [Test]
        public void TestStringConstructor()
        {
            var uid = new QUuid("{67C8770B-44F1-410A-AB9A-F9B5446F13EE}");
        }

        [Test]
        public void TestFromString()
        {
            Assert.AreEqual(_uuidA.ToString(), (new QUuid("{fc69b59e-cc34-4436-a43c-ee95d128b8c5}")).ToString());
            Assert.AreEqual(_uuidA.ToString(), (new QUuid("fc69b59e-cc34-4436-a43c-ee95d128b8c5}")).ToString());
            Assert.AreEqual(_uuidA.ToString(), (new QUuid("{fc69b59e-cc34-4436-a43c-ee95d128b8c5")).ToString());
            Assert.AreEqual(_uuidA.ToString(), (new QUuid("fc69b59e-cc34-4436-a43c-ee95d128b8c5")).ToString());

            Assert.AreEqual(new QUuid(), new QUuid("{fc69b59e-cc34-4436-a43c-ee95d128b8c"));

            Assert.AreEqual(_uuidB.ToString(), (new QUuid("{1ab6e93a-b1cb-4a87-ba47-ec7e99039a7b}")).ToString());
        }

        [Test]
        public void TestQByteArrayConstructor()
        {
            var uid = new QUuid(new QByteArray("{67C8770B-44F1-410A-AB9A-F9B5446F13EE}"));
            Assert.NotNull(uid);
        }

        [Test]
        public void TestWindowsGuidConstructor()
        {
            //var uid = new QUuid(Guid.NewGuid());

            throw new AssertionException("GUID ctor not implemented!");
        }

        [Test]
        public void TestCreateUuidConstructor()
        {
            var uid = QUuid.CreateUuid();
            Assert.IsFalse(uid.IsNull);
        }

        [Test]
        public void TestCreateUuidV3()
        {
            var uid = QUuid.CreateUuidV3(_uuidNs, new QByteArray("www.widgets.com"));
            Assert.AreEqual(_uuidC, uid);

            var uid2 = QUuid.CreateUuidV3(_uuidNs, "www.widgets.com");
            Assert.AreEqual(_uuidC, uid2);
        }

        [Test]
        public void TestCreateUuidV5()
        {
            var uid = QUuid.CreateUuidV5(_uuidNs, new QByteArray("www.widgets.com"));
            Assert.AreEqual(_uuidD, uid);

            //throw new AssertionException("string para not implemented!");
            var uid2 = QUuid.CreateUuidV5(_uuidNs, "www.widgets.com");
            Assert.AreEqual(_uuidD, uid2);
        }

        [Test]
        public void TestFromRfc4122()
        {
            Assert.AreEqual(_uuidA,
                QUuid.FromRfc4122(QByteArray.FromHex(new QByteArray("fc69b59ecc344436a43cee95d128b8c5"))));

            Assert.AreEqual(_uuidB,
                QUuid.FromRfc4122(QByteArray.FromHex(new QByteArray("1ab6e93ab1cb4a87ba47ec7e99039a7b"))));
        }

        [Test]
        public void TestIsNull()
        {
            Assert.IsFalse(_uuidA.IsNull);

            var shouldNull = new QUuid();
            Assert.IsTrue(shouldNull.IsNull);
        }

        [Test]
        public unsafe void TestToByteArray()
        {
            Assert.AreEqual(_uuidA.ToByteArray().ToInt(), (new QByteArray("{fc69b59e-cc34-4436-a43c-ee95d128b8c5}")).ToInt());
            Assert.AreEqual(_uuidB.ToByteArray().ToInt(), (new QByteArray("{1ab6e93a-b1cb-4a87-ba47-ec7e99039a7b}")).ToInt());
        }

        [Test]
        public unsafe void TestToRfc4122()
        {
            Assert.AreEqual(_uuidA.ToRfc4122().ToInt(), (new QByteArray("fc69b59ecc344436a43cee95d128b8c5")).ToInt());
            Assert.AreEqual(_uuidB.ToRfc4122().ToInt(), (new QByteArray("1ab6e93ab1cb4a87ba47ec7e99039a7b")).ToInt());
        }

        [Test]
        public void TestToString()
        {
            Assert.AreEqual(_uuidA.ToString(), "{fc69b59e-cc34-4436-a43c-ee95d128b8c5}");
            Assert.AreEqual(_uuidB.ToString(), "{1ab6e93a-b1cb-4a87-ba47-ec7e99039a7b}");
        }

        [Test]
        public void TestVariant()
        {
            Assert.IsTrue(_uuidA.variant == QUuid.Variant.DCE);
            Assert.IsTrue(_uuidB.variant == QUuid.Variant.DCE);

            var ncs = new QUuid("{3a2f883c-4000-000d-0000-00fb40000000}");
            Assert.IsTrue(ncs.variant == QUuid.Variant.NCS);
        }

        [Test]
        public void TestVersion()
        {
            Assert.IsTrue(_uuidA.version == QUuid.Version.Random);
            Assert.IsTrue(_uuidB.version == QUuid.Version.Random);

            var dceTime = new QUuid("{406c45a0-3b7e-11d0-80a3-0000c08810a7}");
            Assert.IsTrue(dceTime.version == QUuid.Version.Time);

            var ncs = new QUuid("{3a2f883c-4000-000d-0000-00fb40000000}");
            Assert.IsTrue(ncs.version == QUuid.Version.VerUnknown);
        }

        [Test]
        public void TestGuid()
        {
            throw new AssertionException("GUID not implemented!");
        }

        [Ignore("Bug!")]
        [Test]
        public void TestNotEqualOperator()
        {
            Assert.IsTrue(_uuidA != _uuidB);

            var g = new Guid("1ab6e93a-b1cb-4a87-ba47-ec7e99039a7b");
            throw new AssertionException("GUID not implemented!");
            //Assert.IsTrue(_uuidA != g);
        }

        [Test]
        public void TestLessOperator()
        {
            Assert.IsFalse(_uuidA < _uuidB);
        }

        [Test]
        public void TestIsOperator()
        {
            throw new AssertionException("GUID not implemented!");
            //QUuid uid = Guid.NewGuid();
            //Assert.NotNull(uid);
        }

        [Test]
        public void TestEqualOperator()
        {
            Assert.IsFalse(_uuidA == _uuidB);

            QUuid assigned = _uuidA;
            Assert.IsTrue(_uuidA == assigned);

            var g = new Guid("fc69b59e-cc34-4436-a43c-ee95d128b8c5");
            throw new AssertionException("GUID not implemented!");
            //Assert.IsTrue(_uuidA == g);
        }

        [Test]
        public void TestGreaterOperator()
        {
            Assert.IsTrue(_uuidA > _uuidB);

            Assert.IsFalse(new QUuid() > _uuidB);
        }

        [Test]
        public void TestQHash()
        {
            var u = QUuid.QHash(_uuidA);

            Assert.AreEqual(QUuid.QHash(_uuidA), u);
        }
    }
}