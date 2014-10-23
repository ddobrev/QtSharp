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
            var group = new QSequentialAnimationGroup();

            int i = 0;
            group.StateChanged += (arg1, arg2) => { i++; };

            Assert.AreEqual(QAnimationGroup.State.Stopped, group.state);
            group.Start();

            // TODO Add test.					
        }

        [Test]
        public void TestSetCurrentTime()
        {
            var s_o1 = new AnimationObject();
            var s_o2 = new AnimationObject();
            var s_o3 = new AnimationObject();
            var p_o1 = new AnimationObject();
            var p_o2 = new AnimationObject();
            var p_o3 = new AnimationObject();
            var t_o1 = new AnimationObject();
            var t_o2 = new AnimationObject();

            // sequence operating on same object/property
            var sequence = new QSequentialAnimationGroup();
            var a1_s_o1 = new QPropertyAnimation(s_o1, new QByteArray("value"));
            var a2_s_o1 = new QPropertyAnimation(s_o1, new QByteArray("value"));
            var a3_s_o1 = new QPropertyAnimation(s_o1, new QByteArray("value"));
            a2_s_o1.LoopCount = 3;
            sequence.AddAnimation(a1_s_o1);
            sequence.AddAnimation(a2_s_o1);
            sequence.AddAnimation(a3_s_o1);

            // sequence operating on different object/properties
            var sequence2 = new QSequentialAnimationGroup();
            var a1_s_o2 = new QPropertyAnimation(s_o2, new QByteArray("value"));
            var a1_s_o3 = new QPropertyAnimation(s_o3, new QByteArray("value"));
            sequence2.AddAnimation(a1_s_o2);
            sequence2.AddAnimation(a1_s_o3);

            // parallel operating on different object/properties
            var parallel = new QParallelAnimationGroup();
            var a1_p_o1 = new QPropertyAnimation(p_o1, new QByteArray("value"));
            var a1_p_o2 = new QPropertyAnimation(p_o2, new QByteArray("value"));
            var a1_p_o3 = new QPropertyAnimation(p_o3, new QByteArray("value"));
            a1_p_o2.LoopCount = 3;
            parallel.AddAnimation(a1_p_o1);
            parallel.AddAnimation(a1_p_o2);
            parallel.AddAnimation(a1_p_o3);

            var notTimeDriven = new UncontrolledAnimation(t_o1, new QByteArray("value"));
            Assert.AreEqual(-1, notTimeDriven.TotalDuration);

            var loopsForever = new QPropertyAnimation(t_o2, new QByteArray("value"));
            loopsForever.LoopCount = -1;
            Assert.AreEqual(-1, loopsForever.TotalDuration);

            var group = new QParallelAnimationGroup();
            group.AddAnimation(sequence);
            group.AddAnimation(sequence2);
            group.AddAnimation(parallel);
            group.AddAnimation(notTimeDriven);
            group.AddAnimation(loopsForever);

            // Current time = 1
            group.CurrentTime = 1;
            Assert.AreEqual(QAnimationGroup.State.Stopped, group.state);
            Assert.AreEqual(QAnimationGroup.State.Stopped, sequence.state);
            Assert.AreEqual(QAnimationGroup.State.Stopped, a1_s_o1.state);
            Assert.AreEqual(QAnimationGroup.State.Stopped, sequence2.state);
            Assert.AreEqual(QAnimationGroup.State.Stopped, a1_s_o2.state);
            Assert.AreEqual(QAnimationGroup.State.Stopped, parallel.state);
            Assert.AreEqual(QAnimationGroup.State.Stopped, a1_p_o1.state);
            Assert.AreEqual(QAnimationGroup.State.Stopped, a1_p_o2.state);
            Assert.AreEqual(QAnimationGroup.State.Stopped, a1_p_o3.state);
            Assert.AreEqual(QAnimationGroup.State.Stopped, notTimeDriven.state);
            Assert.AreEqual(QAnimationGroup.State.Stopped, loopsForever.state);

            Assert.AreEqual(1, group.CurrentLoopTime);
            Assert.AreEqual(1, sequence.CurrentLoopTime);
            Assert.AreEqual(1, a1_s_o1.CurrentLoopTime);
            Assert.AreEqual(0, a2_s_o1.CurrentLoopTime);
            Assert.AreEqual(0, a3_s_o1.CurrentLoopTime);
            Assert.AreEqual(1, a1_s_o2.CurrentLoopTime);
            Assert.AreEqual(0, a1_s_o3.CurrentLoopTime);
            Assert.AreEqual(1, a1_p_o1.CurrentLoopTime);
            Assert.AreEqual(1, a1_p_o2.CurrentLoopTime);
            Assert.AreEqual(1, a1_p_o3.CurrentLoopTime);
            Assert.AreEqual(1, notTimeDriven.CurrentLoopTime);
            Assert.AreEqual(1, loopsForever.CurrentLoopTime);

            // Current time = 250
            group.CurrentTime = 250;
            Assert.AreEqual(250, group.CurrentLoopTime);
            Assert.AreEqual(250, sequence.CurrentLoopTime);
            Assert.AreEqual(250, a1_s_o1.CurrentLoopTime);
            Assert.AreEqual(0, a2_s_o1.CurrentLoopTime);
            Assert.AreEqual(0, a3_s_o1.CurrentLoopTime);
            Assert.AreEqual(250, a1_s_o2.CurrentLoopTime);
            Assert.AreEqual(0, a1_s_o3.CurrentLoopTime);
            Assert.AreEqual(250, a1_p_o1.CurrentLoopTime);
            Assert.AreEqual(0, a1_p_o2.CurrentLoopTime);
            Assert.AreEqual(1, a1_p_o2.CurrentLoop);
            Assert.AreEqual(250, a1_p_o3.CurrentLoopTime);
            Assert.AreEqual(250, notTimeDriven.CurrentLoopTime);
            Assert.AreEqual(0, loopsForever.CurrentLoopTime);
            Assert.AreEqual(1, loopsForever.CurrentLoop);
            Assert.AreSame(a2_s_o1, sequence.CurrentAnimation);

            // Current time = 251
            group.CurrentTime = 251;
            Assert.AreEqual(251, group.CurrentLoopTime);
            Assert.AreEqual(251, sequence.CurrentLoopTime);
            Assert.AreEqual(251, a1_s_o1.CurrentLoopTime);
            Assert.AreEqual(1, a2_s_o1.CurrentLoopTime);
            Assert.AreEqual(0, a2_s_o1.CurrentLoop);
            Assert.AreEqual(0, a3_s_o1.CurrentLoopTime);
            Assert.AreEqual(251, sequence2.CurrentLoopTime);

            Assert.AreEqual(250, a1_s_o2.CurrentLoopTime);
            Assert.AreEqual(1, a1_s_o3.CurrentLoopTime);

            Assert.AreEqual(250, a1_p_o1.CurrentLoopTime);
            Assert.AreEqual(1, a1_p_o2.CurrentLoopTime);
            Assert.AreEqual(1, a1_p_o2.CurrentLoop);

            Assert.AreEqual(250, a1_p_o3.CurrentLoopTime);

            Assert.AreEqual(251, notTimeDriven.CurrentLoopTime);
            Assert.AreEqual(1, loopsForever.CurrentLoopTime);
            Assert.AreSame(a2_s_o1, sequence.CurrentAnimation);
        }

        [Test]
        public void TestSetParentAutoAdd()
        {
            var group = new QParallelAnimationGroup();
            var animation = new QPropertyAnimation(group);

            Assert.AreEqual(animation.Group, (QAnimationGroup)group);
        }

        [Test]
        public void TestBeginNestedGroup()
        {
            QAnimationGroup parent = new QParallelAnimationGroup();

            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 1)
                {
                    new QParallelAnimationGroup(parent);
                }
                else
                {
                    new QSequentialAnimationGroup(parent);
                }

                Assert.AreEqual(1, parent.AnimationCount);
                QAnimationGroup child = (QAnimationGroup)parent.AnimationAt(0);

                Assert.AreSame(child.Parent, parent);

                if (i % 2 == 1)
                    Assert.IsTrue((child as QParallelAnimationGroup) != null);
                else
                    Assert.IsTrue((child as QSequentialAnimationGroup) != null);
                parent = child;
            }
        }

        [Test]
        public void TestAddChildTwice()
        {
            QAnimationGroup parent = new QSequentialAnimationGroup();
            QAbstractAnimation subGroup = new QPropertyAnimation();

            subGroup.Parent = parent;
            parent.AddAnimation(subGroup);
            Assert.AreEqual(1, parent.AnimationCount);

            parent.Clear();

            Assert.AreEqual(0, parent.AnimationCount);

            // adding the same item twice to a group will remove the item from its current position
            // and append it to the end
            subGroup = new QPropertyAnimation(parent);
            QAbstractAnimation subGroup2 = new QPropertyAnimation(parent);

            Assert.AreEqual(2, parent.AnimationCount);
            Assert.AreSame(subGroup, parent.AnimationAt(0));
            Assert.AreSame(subGroup2, parent.AnimationAt(1));

            parent.AddAnimation(subGroup);

            Assert.AreEqual(2, parent.AnimationCount);
            Assert.AreSame(subGroup2, parent.AnimationAt(0));
            Assert.AreSame(subGroup, parent.AnimationAt(1));
        }

        [Test]
        public unsafe void TestLoopWithoutStartValue()
        {
            QAnimationGroup parent = new QSequentialAnimationGroup();
            var o = new QObject();
            o.SetProperty("ole", new QVariant(0));
            Assert.AreEqual(0, o.Property("ole").ToInt());

            var anim1 = new QPropertyAnimation(o, new QByteArray("ole"));
            anim1.EndValue = new QVariant(-50);
            anim1.SetDuration(100);

            var anim2 = new QPropertyAnimation(o, new QByteArray("ole"));
            anim2.EndValue = new QVariant(50);
            anim2.SetDuration(100);

            parent.AddAnimation(anim1);
            parent.AddAnimation(anim2);

            parent.LoopCount = -1;
            parent.Start();

            Assert.IsTrue(anim1.StartValue.IsNull);
            Assert.AreEqual(0, anim1.CurrentValue.ToInt());
            Assert.AreEqual(0, parent.CurrentLoop);

            parent.CurrentTime = 200;
            Assert.AreEqual(1, parent.CurrentLoop);
            Assert.AreEqual(50, anim1.CurrentValue.ToInt());
            parent.Stop();
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
            public virtual void UpdateCurrentValue(ref QVariant value)
            { }

            public virtual void UpdateState(QAbstractAnimation.State oldState,
                                            QAbstractAnimation.State newState)
            { }

        }

        private class UncontrolledAnimation : QPropertyAnimation
        {
            private int _id;

            public override int Duration { get { return -1; } }

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

            public UncontrolledAnimation(QObject target, QByteArray propertyName, QObject parent = null)
                : base(target, propertyName, parent)
            {
                _id = 0;
                SetDuration(250);
            }
        }
    }
}