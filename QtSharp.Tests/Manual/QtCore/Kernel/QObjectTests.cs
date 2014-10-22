using System;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore.Kernel
{
    [TestFixture]
    public class QObjectTests
    {
        private QObject _qObject;

        [SetUp]
        public void Init()
        {
            // TODO: Add Init code.
            _qObject = new QObject();
        }

        [TearDown]
        public void Dispose()
        {
            // TODO: Add tear down code.
        }

        #region Ctor
        //[Ignore("Bug!")]
        //[Test]
        //public void TestQStringRefConstructor()
        //{
        //    var s = new QStringRef(_qString);

        //    Assert.AreEqual(_testString, s.ToString());
        //}

        [Test]
        public void TestParentChildConstructor()
        {
            var o = new QObject(_qObject);

            Assert.IsTrue(_qObject.Children.Contains(o));
        }

        [Test]
        public void TestNativeConstructor()
        {
            var o = QObject.__CreateInstance(_qObject.__Instance);

            Assert.AreEqual(_qObject.__Instance, o.__Instance);
        }
        #endregion

        #region BlockSignals
        [Test]
        public void TestBlockSignals()
        {
            _qObject.BlockSignals(true);

            Assert.IsTrue(_qObject.SignalsBlocked);
        }
        #endregion

        // Todo Add Connect

        #region DeleteLater
        [Test]
        public unsafe void TestDeleteLater()
        {
            var argc = 0;
            string[] argv = new string[argc + 1];
            var a = Assembly.GetEntryAssembly();

            if (a == null)
                a = Assembly.GetExecutingAssembly();

            var attrs = a.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);

            if (attrs.Length > 0)
            {
                argv[0] = ((AssemblyTitleAttribute)attrs[0]).Title;
            }
            else
            {
                QFileInfo info = new QFileInfo(a.Location);
                argv[0] = info.BaseName;
            }

            var p = Marshal.StringToHGlobalAuto(argv[0]);
            var k = (char*)p;

            var core = new QCoreApplication(&argc, &k);



            _qObject.DeleteLater();

            Assert.IsNotNull(_qObject);

            QCoreApplication.Exec();

            Assert.IsNull(_qObject);
        }
        #endregion

        // Todo Add Disconnect

        #region DumpObjectInfo
        [Ignore("How to test?")]
        [Test]
        public void TestDumpObjectInfo()
        {
            _qObject.DumpObjectInfo();
        }
        #endregion

        #region DumpObjectTree
        [Ignore("How to test?")]
        [Test]
        public void TestDumpObjectTree()
        {
            _qObject.DumpObjectTree();
        }
        #endregion

        // Todo Add Filter events

        #region Inherits
        [Test]
        public void TestInherits()
        {
            var timer = new QTimer();
            Assert.IsTrue(timer.Inherits("QTimer")); // Docu says should return true, but return false
            Assert.IsTrue(timer.Inherits("QObject"));
            Assert.IsFalse(timer.Inherits("QAbstractButton"));
        }
        #endregion

        // Todo Add InstallEventFilter
        // Todo Add IsSignalConnected

        #region Timer
        [Test]
        public void TestTimer()
        {
            var i = _qObject.StartTimer(0);

            Assert.Greater(i, 0);

            var wasFired = false;
            _qObject.TimerEvent += (o, e) => wasFired = true;

            Assert.IsTrue(wasFired);

            _qObject.KillTimer(i);
            Assert.AreEqual(0, i);
        }
        #endregion

        #region MoveToThread
        [Test]
        public void TestMoveToThread()
        {
            var curT = _qObject.Thread;

            var t = new QThread();

            IntPtr p = curT.__Instance;
            IntPtr p2 = t.__Instance;

            Assert.AreNotEqual(p, p2);

            _qObject.MoveToThread(t);

            var curT2 = _qObject.Thread;
            Assert.AreEqual(curT2.__Instance, t.__Instance);
        }
        #endregion

        // Todo Add OnChildEvent
        // Todo Add OnCustomEvent
        // Todo Add OnEvent
        // Todo Add OnTimerEvent

        #region Property
        [Test]
        public void TestProperty()
        {
            // Todo Add Property
        }
        #endregion

        // Todo Add Qt_metacall
        // Todo Add Qt_metacast
        // Todo Add Qt_qFindChild_helper
        // Todo Add Qt_qFindChildren_helper
        // Todo Add Receivers
        // Todo Add RegisterUserData
        // Todo Add RemoveEventFilter
        // Todo Add SetProperty
        // Todo Add SetUserData
        // Todo Add Tr
        // Todo Add TrUtf8
        // Todo Add UserData

        #region Children
        [Test]
        public void TestChildren()
        {
            var o = new QObject(_qObject);
            var childs = _qObject.Children;

            Assert.IsTrue(childs.Contains(o));
        }
        #endregion

        // Todo Add DynamicPropertyNames

        #region IsWidgetType
        [Test]
        public void TestIsWidgetType()
        {
            var isW = _qObject.IsWidgetType;

            Assert.IsFalse(isW);
        }
        #endregion

        #region IsWindowType
        [Test]
        public void TestIsWindowType()
        {
            var isW = _qObject.IsWindowType;

            Assert.IsFalse(isW);
        }
        #endregion

        // Todo Add MetaObject 

        #region ObjectName
        [Test]
        public void TestObjectName()
        {
            var isW = _qObject.ObjectName;

            Assert.IsNullOrEmpty(isW);

            _qObject.ObjectName = "MyObject";

            Assert.AreEqual("MyObject", _qObject.ObjectName);
        }
        #endregion

        #region Parent
        [Test]
        public void TestParent()
        {
            var o = new QObject(_qObject);

            Assert.AreEqual(_qObject.__Instance, o.Parent.__Instance);
        }
        #endregion

        // Todo Add Sender 
        // Todo Add SenderSignalIndex 
        // Todo Add staticMetaObject 
        // Todo Add dynamicQObject
        // Todo Add eventFilters
        // Todo Add ChildEvent
        // Todo Add CustomEvent
        // Todo Add Destroyed
        // Todo Add Event
        // Todo Add ObjectNameChanged
    }
}