using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore.Animation
{
    [TestFixture]
    public class QAbstractAnimationTests
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

        private class TestableQAbstractAnimation1 : QAbstractAnimation
        {
            private int duration = 10;

            public override int Duration
            {
                get { return this.duration; }
            }

            protected override void UpdateCurrentTime(int value)
            {

            }

            public void SetDuration(int val)
            {
                this.duration = val;
            }
        }

        [Test]
        public void TestEmptyConstructor()
        {
            using (new TestableQAbstractAnimation1())
            {
            }
        }

        [Test]
        public void TestCurrentLoop()
        {
            using (var anim = new TestableQAbstractAnimation())
            {
                Assert.AreEqual(0, anim.CurrentLoop);
            }
        }
        
        [Test]
        public void TestCurrentLoopTime()
        {
            using (var anim = new TestableQAbstractAnimation())
            {
                Assert.AreEqual(0, anim.CurrentLoopTime);
            }
        }
        
        [Test]
        public void TestCurrentTime()
        {
            using (var anim = new TestableQAbstractAnimation())
            {
                Assert.AreEqual(0, anim.CurrentTime);
        	
                anim.CurrentTime = 10;
                Assert.AreEqual(10, anim.CurrentTime);
            }
        }
        
        [Test]
        public void TestDirection()
        {
            using (var anim = new TestableQAbstractAnimation())
            {
                Assert.AreEqual(QAbstractAnimation.Direction.Forward.ToString(), anim.direction.ToString());
        	
                anim.direction = QAbstractAnimation.Direction.Backward;
                Assert.AreEqual(QAbstractAnimation.Direction.Backward.ToString(), anim.direction.ToString());
        	
                anim.direction = QAbstractAnimation.Direction.Forward;
                Assert.AreEqual(QAbstractAnimation.Direction.Forward.ToString(), anim.direction.ToString());
            }
        }
        
        [Test]
        public void TestGroup()
        {
            var anim = new TestableQAbstractAnimation();
            using (var group = new DummyQAnimationGroup())
            {
                @group.AddAnimation(anim);

                Assert.AreSame(@group, anim.Group);
            }
        }

        [Test]
        public void TestLoopCount()
        {
            using (var anim = new TestableQAbstractAnimation())
            {
                Assert.AreEqual(1, anim.LoopCount);
        	
                anim.LoopCount = 10;
                Assert.AreEqual(10, anim.LoopCount);
            }
        }
        
        [Test]
        public void TestState()
        {
            using (var anim = new TestableQAbstractAnimation())
            {
                Assert.AreEqual(QAbstractAnimation.State.Stopped, anim.state);
            }
        }
        
        [Test]
        public void TestTotalDuration()
        {
            using (var anim = new TestableQAbstractAnimation())
            {
                Assert.AreEqual(10, anim.TotalDuration);
        	
                anim.LoopCount = 5;
                Assert.AreEqual(50, anim.TotalDuration);
            }
        }
        
        [Test]
        public void TestAvoidJumpAtStart()
        {
            using (var anim = new TestableQAbstractAnimation())
            {
                anim.SetDuration(1000);
        	
                anim.Start();
        	
                System.Threading.Thread.Sleep(300);
        	
                QCoreApplication.ProcessEvents();
                Assert.Less(anim.CurrentTime, 50);
            }
        }
        
        [Test]
        public void TestAvoidJumpAtStartWithStop()
        {
            using (var anim = new TestableQAbstractAnimation())
            {
                anim.SetDuration(1000);

                using (var anim2 = new TestableQAbstractAnimation())
                {
                    anim2.SetDuration(1000);

                    using (var anim3 = new TestableQAbstractAnimation())
                    {
                        anim3.SetDuration(1000);
        	
                        anim.Start();            	
                        System.Threading.Thread.Sleep(300);
                        anim.Stop();
        	
                        anim2.Start();            	
                        System.Threading.Thread.Sleep(300);
                        anim3.Start();
        	
                        QCoreApplication.ProcessEvents();
                        Assert.Less(anim2.CurrentTime, 50);
                        Assert.Less(anim3.CurrentTime, 50);
                    }
                }
            }
        }
        
        [Test]
        public void TestAvoidJumpAtStartWithRunning()
        {
            using (var anim = new TestableQAbstractAnimation())
            {
                anim.SetDuration(2000);

                using (var anim2 = new TestableQAbstractAnimation())
                {
                    anim2.SetDuration(1000);

                    using (var anim3 = new TestableQAbstractAnimation())
                    {
                        anim3.SetDuration(1000);
        	
                        anim.Start();            	
                        System.Threading.Thread.Sleep(300);
        	
                        anim2.Start();            	
                        System.Threading.Thread.Sleep(300);
                        anim3.Start();
        	
                        QCoreApplication.ProcessEvents();
                        Assert.Less(anim2.CurrentTime, 50);
                        Assert.Less(anim3.CurrentTime, 50);
                    }
                }
            }
        }

        private class TestableQAbstractAnimation : QAbstractAnimation
        {
        	private int duration = 10;
        	
        	public override int Duration
        	{
        		get { return this.duration; }
        	}
        	
        	protected override void UpdateCurrentTime(int value)
        	{
        		
        	}
        	
        	public void SetDuration(int val)
        	{
        		this.duration = val;
        	}
        }

        private class DummyQAnimationGroup : QAnimationGroup
        {
            private int duration = 10;

            public override int Duration
            {
                get { return this.duration; }
            }

            protected override void UpdateCurrentTime(int value)
            {

            }
        }
    }
}