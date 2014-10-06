using System;
using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore.Kernel
{
    [TestFixture]
    public class QAbstractEventDispatcherTests
    {
        internal const int PreciseTimerInterval = 10;
        internal const int CoarseTimerInterval = 200;
        internal const int VeryCoarseTimerInterval = 1000;

        private QAbstractEventDispatcher _eventDispatcher;
        private int _receivedEventType;
        private int _timerIdFromEvent;

        [SetUp]
        public void Init()
        {
            // TODO: Add Init code.
            _eventDispatcher = QAbstractEventDispatcher.Instance();
            _receivedEventType = -1;
            _timerIdFromEvent = -1;

            throw new AssertionException("QElapsedTimer: not default ctor.");
            //var elapsedTimer = new QElapsedTimer();
            //elapsedTimer.Start();

            //while (!elapsedTimer.HasExpired(CoarseTimerInterval) && 
            //    _eventDispatcher.ProcessEvents(QEventLoop.ProcessEventsFlag.AllEvents))
            //{ }
        }

        [TearDown]
        public void Dispose()
        {
            // TODO: Add tear down code.
        }

        [Test]
        public void TestRegisterTimer()
        {

        }
    }
}