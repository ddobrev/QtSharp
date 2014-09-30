using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore.Animation
{
    [TestFixture]
    public class QAnimationGroupTests
    {
        [SetUp]
        public void Init()
        {
            // TODO: Add Init code.
        }

        [TearDown]
        public void Dispose()
        {
            // TODO: Add tear down code.
        }

        [Test]
        public void TestEmptyGroup()
        {

        }

        [Test]
        public void TestSetCurrentTime()
        {

        }

        [Test]
        public void TestSetParentAutoAdd()
        {

        }

        [Test]
        public void TestBeginNestedGroup()
        {

        }

        [Test]
        public void TestAddChildTwice()
        {
            QAnimationGroup parent = new QSequentialAnimationGroup();
            QAbstractAnimation subGroup = new QPropertyAnimation();
            QAbstractAnimation subGroup2 = new QPropertyAnimation();

            subGroup.Parent = parent;
            parent.AddAnimation(subGroup);
            Assert.AreEqual(1, parent.AnimationCount);

            parent.Clear();

            Assert.AreEqual(0, parent.AnimationCount);

            subGroup = new QPropertyAnimation(parent);
            subGroup2 = new QPropertyAnimation(parent);

            Assert.AreEqual(2, parent.AnimationCount);
            Assert.AreSame(subGroup, parent.AnimationAt(0));
            Assert.AreSame(subGroup2, parent.AnimationAt(1));

            parent.AddAnimation(subGroup);

            Assert.AreEqual(2, parent.AnimationCount);
            Assert.AreSame(subGroup2, parent.AnimationAt(0));
            Assert.AreSame(subGroup, parent.AnimationAt(1));
        }

        [Test]
        public void TestLoopWithoutStartValue()
        {

        }

        private class AnimationObject : QObject
        {
            public int Value { get; set; }

            public AnimationObject(int startValue = 0)
            {
                Value = startValue;
            }
        }

        private class TestAnimation : QVariantAnimation
        {
            public virtual void UpdateCurrentValue(QVariant value)
            { }

            public virtual void UpdateState(QAbstractAnimation.State oldState,
                                            QAbstractAnimation.State newState)
            { }

        }

        private class UncontrolledAnimation : QPropertyAnimation
        {
            private int _id;

            public int Duration { get { return -1; } }

            protected void TimerEvent(QTimerEvent e)
            {
                if (e.TimerId == _id)
                    Stop();
            }

            protected void UpdateRunning(bool running)
            {
                if (running)
                {
                    _id = StartTimer(500);
                }
                else
                {
                    KillTimer(_id);
                    _id = 0;
                }
            }


            public UncontrolledAnimation(QObject target, QByteArray propertyName, QObject parent)
                : base(target, propertyName, parent)
            {
                _id = 0;
                SetDuration(250);
            }
        }
    }
}