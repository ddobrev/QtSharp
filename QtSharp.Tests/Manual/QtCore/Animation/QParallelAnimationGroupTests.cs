using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore.Animation
{
    [TestFixture]
    public class QParallelAnimationGroupTests
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
        public void TestSetCurrentTime()
        {
            var p_o1 = new AnimationObject();
            var p_o2 = new AnimationObject();
            var p_o3 = new AnimationObject();
            var t_o1 = new AnimationObject();
            var t_o2 = new AnimationObject();

            // parallel operating on different object/properties
            QAnimationGroup parallel = new QParallelAnimationGroup();
            var a1_p_o1 = new QPropertyAnimation(p_o1, new QByteArray("value"));
            var a1_p_o2 = new QPropertyAnimation(p_o2, new QByteArray("value"));
            var a1_p_o3 = new QPropertyAnimation(p_o3, new QByteArray("value"));
            a1_p_o2.LoopCount = 3;
            parallel.AddAnimation(a1_p_o1);
            parallel.AddAnimation(a1_p_o2);
            parallel.AddAnimation(a1_p_o3);

            var notTimeDriven = new UncontrolledAnimation(t_o1, new QByteArray("value"));
            Assert.AreEqual(-1, notTimeDriven.TotalDuration);

            QVariantAnimation loopsForever = new QPropertyAnimation(t_o2, new QByteArray("value"));
            loopsForever.LoopCount = -1;
            Assert.AreEqual(-1, loopsForever.TotalDuration);

            var group = new QParallelAnimationGroup();
            group.AddAnimation(parallel);
            group.AddAnimation(notTimeDriven);
            group.AddAnimation(loopsForever);

            // Current time = 1
            group.CurrentTime = 1;
            Assert.AreEqual(QAnimationGroup.State.Stopped, group.state);
            Assert.AreEqual(QAnimationGroup.State.Stopped, parallel.state);
            Assert.AreEqual(QAnimationGroup.State.Stopped, a1_p_o1.state);
            Assert.AreEqual(QAnimationGroup.State.Stopped, a1_p_o2.state);
            Assert.AreEqual(QAnimationGroup.State.Stopped, a1_p_o3.state);
            Assert.AreEqual(QAnimationGroup.State.Stopped, notTimeDriven.state);
            Assert.AreEqual(QAnimationGroup.State.Stopped, loopsForever.state);

            Assert.AreEqual(1, group.CurrentLoopTime);
            Assert.AreEqual(1, a1_p_o1.CurrentLoopTime);
            Assert.AreEqual(1, a1_p_o2.CurrentLoopTime);
            Assert.AreEqual(1, a1_p_o3.CurrentLoopTime);
            Assert.AreEqual(1, notTimeDriven.CurrentLoopTime);
            Assert.AreEqual(1, loopsForever.CurrentLoopTime);

            // Current time = 250
            group.CurrentTime = 250;
            Assert.AreEqual(250, group.CurrentLoopTime);
            Assert.AreEqual(250, a1_p_o1.CurrentLoopTime);
            Assert.AreEqual(0, a1_p_o2.CurrentLoopTime);
            Assert.AreEqual(1, a1_p_o2.CurrentLoop);
            Assert.AreEqual(250, a1_p_o3.CurrentLoopTime);
            Assert.AreEqual(250, notTimeDriven.CurrentLoopTime);
            Assert.AreEqual(0, loopsForever.CurrentLoopTime);
            Assert.AreEqual(1, loopsForever.CurrentLoop);

            // Current time = 251
            group.CurrentTime = 251;
            Assert.AreEqual(251, group.CurrentLoopTime);

            Assert.AreEqual(250, a1_p_o1.CurrentLoopTime);
            Assert.AreEqual(1, a1_p_o2.CurrentLoopTime);
            Assert.AreEqual(1, a1_p_o2.CurrentLoop);

            Assert.AreEqual(250, a1_p_o3.CurrentLoopTime);

            Assert.AreEqual(251, notTimeDriven.CurrentLoopTime);
            Assert.AreEqual(1, loopsForever.CurrentLoopTime);
        }

        [Test]
        public void TestStateChanged()
        {
            //this ensures that the correct animations are started when starting the group
            var anim1 = new TestAnimation();
            var anim2 = new TestAnimation();
            var anim3 = new TestAnimation();
            var anim4 = new TestAnimation();
            anim1.SetDuration(1000);
            anim2.SetDuration(2000);
            anim3.SetDuration(3000);
            anim4.SetDuration(3000);

            var group = new QParallelAnimationGroup();
            group.AddAnimation(anim1);
            group.AddAnimation(anim2);
            group.AddAnimation(anim3);
            group.AddAnimation(anim4);

            group.Start();

            var spy1 = 0;
            anim1.StateChanged += (arg1, arg2) => { spy1++; };
            var spy2 = 0;
            anim2.StateChanged += (arg1, arg2) => { spy2++; };
            var spy3 = 0;
            anim3.StateChanged += (arg1, arg2) => { spy3++; };
            var spy4 = 0;
            anim4.StateChanged += (arg1, arg2) => { spy4++; };

            Assert.AreEqual(1, spy1);
            Assert.AreEqual(QAbstractAnimation.State.Running, anim1.state);
            Assert.AreEqual(1, spy2);
            Assert.AreEqual(QAbstractAnimation.State.Running, anim2.state);
            Assert.AreEqual(1, spy3);
            Assert.AreEqual(QAbstractAnimation.State.Running, anim3.state);
            Assert.AreEqual(1, spy4);
            Assert.AreEqual(QAbstractAnimation.State.Running, anim4.state);

            group.CurrentTime = 1500;
            Assert.AreEqual(QAbstractAnimation.State.Running, group.state);
            Assert.AreEqual(2, spy1);
            Assert.AreEqual(QAbstractAnimation.State.Stopped, anim1.state);
            Assert.AreEqual(1, spy2);
            Assert.AreEqual(1, spy3);
            Assert.AreEqual(1, spy4);

            group.CurrentTime = 2500;
            Assert.AreEqual(QAbstractAnimation.State.Running, group.state);
            Assert.AreEqual(2, spy1);
            Assert.AreEqual(2, spy2);
            Assert.AreEqual(QAbstractAnimation.State.Stopped, anim2.state);
            Assert.AreEqual(1, spy3);
            Assert.AreEqual(1, spy4);

            group.CurrentTime = 3500;
            Assert.AreEqual(QAbstractAnimation.State.Running, group.state);
            Assert.AreEqual(2, spy1);
            Assert.AreEqual(2, spy2);
            Assert.AreEqual(2, spy3);
            Assert.AreEqual(QAbstractAnimation.State.Stopped, anim3.state);
            Assert.AreEqual(1, spy4);
            Assert.AreEqual(QAbstractAnimation.State.Stopped, anim4.state);

            group.direction = QAbstractAnimation.Direction.Backward;
            group.Start();

            spy1 = spy2 = spy3 = spy4 = 0;

            Assert.AreEqual(QAbstractAnimation.State.Running, group.state);
            Assert.AreEqual(0, spy1);
            Assert.AreEqual(0, spy2);
            Assert.AreEqual(0, spy3);
            Assert.AreEqual(QAbstractAnimation.State.Running, anim3.state);
            Assert.AreEqual(1, spy4);
            Assert.AreEqual(QAbstractAnimation.State.Running, anim4.state);

            group.CurrentTime = 1500;
            Assert.AreEqual(QAbstractAnimation.State.Running, group.state);
            Assert.AreEqual(0, spy1);
            Assert.AreEqual(1, spy2);
            Assert.AreEqual(QAbstractAnimation.State.Running, anim2.state);
            Assert.AreEqual(1, spy3);
            Assert.AreEqual(1, spy4);

            group.CurrentTime = 500;
            Assert.AreEqual(QAbstractAnimation.State.Running, group.state);
            Assert.AreEqual(1, spy1);
            Assert.AreEqual(QAbstractAnimation.State.Running, anim1.state);
            Assert.AreEqual(1, spy2);
            Assert.AreEqual(1, spy3);
            Assert.AreEqual(1, spy4);

            group.CurrentTime = 0;
            Assert.AreEqual(QAbstractAnimation.State.Stopped, group.state);
            Assert.AreEqual(2, spy1);
            Assert.AreEqual(QAbstractAnimation.State.Stopped, anim1.state);
            Assert.AreEqual(2, spy2);
            Assert.AreEqual(QAbstractAnimation.State.Stopped, anim2.state);
            Assert.AreEqual(1, spy3);
            Assert.AreEqual(QAbstractAnimation.State.Stopped, anim3.state);
            Assert.AreEqual(1, spy4);
            Assert.AreEqual(QAbstractAnimation.State.Stopped, anim4.state);
        }

        [Test]
        public unsafe void TestClearGroup()
        {
            var group = new QParallelAnimationGroup();
            const int animationCount = 10;

            for (int i = 0; i < animationCount; ++i)
            {
                new QParallelAnimationGroup(group);
            }

            Assert.AreEqual(animationCount, group.AnimationCount);

            throw new AssertionException("User so: QPointer<QAbstractAnimation>[] children = new QPointer[]animationCount");

            var children = new QAbstractAnimation[animationCount];
            for (var i = 0; i < animationCount; ++i)
            {
                Assert.AreNotEqual(0, group.AnimationAt(i));
                children[i] = group.AnimationAt(i);
            }

            group.Clear();
            Assert.AreNotEqual(0, group.AnimationCount);
            Assert.AreNotEqual(0, group.CurrentLoopTime);

            for (int i = 0; i < animationCount; ++i)
                Assert.IsTrue(children[i] == null);
                // Assert.IsTrue(children[i].IsNull);
        }

        // TODO

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
            public virtual void UpdateCurrentValue(ref QVariant value)
            { }

            public virtual void UpdateState(QAbstractAnimation.State oldState,
                                            QAbstractAnimation.State newState)
            { }

        }

        private class TestAnimation2 : QVariantAnimation
        {
            int _duration;
            public override int Duration
            {
                get { return _duration; }
            }

            public virtual void UpdateCurrentValue(ref QVariant value)
            { }

            public virtual void UpdateState(QAbstractAnimation.State oldState,
                                            QAbstractAnimation.State newState)
            { }

            public TestAnimation2(QAbstractAnimation animation)
                : base(animation)
            { }

            public TestAnimation2(int duration, QAbstractAnimation animation)
                : base(animation)
            {
                _duration = duration;
            }

        }

        private class UncontrolledAnimation : QPropertyAnimation
        {
            private int _id;

            public override int Duration { get { return -1; } }

            public new void TimerEvent(QTimerEvent e)
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

            public UncontrolledAnimation(QObject target, QByteArray propertyName, QObject parent = null)
                : base(target, propertyName, parent)
            {
                _id = 0;
                SetDuration(250);
                EndValue = new QVariant(0);
            }
        }
    }
}
