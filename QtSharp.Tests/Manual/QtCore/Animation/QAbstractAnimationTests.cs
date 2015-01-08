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
        
        [Test]
        public void TestEmptyConstructor()
        {
            var s = new TestableQAbstractAnimation();
        }
        
        [Test]
        public void TestCurrentLoop()
        {
        	var anim = new TestableQAbstractAnimation();
        	
        	Assert.AreEqual(0, anim.CurrentLoop);
        }
        
        [Test]
        public void TestCurrentLoopTime()
        {
        	var anim = new TestableQAbstractAnimation();
        	
        	Assert.AreEqual(0, anim.CurrentLoopTime);
        }
        
        [Test]
        public void TestCurrentTime()
        {
        	var anim = new TestableQAbstractAnimation();        	
        	Assert.AreEqual(0, anim.CurrentTime);
        	
        	anim.CurrentTime = 10;
        	Assert.AreEqual(10, anim.CurrentTime);
        }
        
        [Test]
        public void TestDirection()
        {
        	var anim = new TestableQAbstractAnimation();    
        	
        	Assert.AreEqual(QAbstractAnimation.Direction.Forward.ToString(), anim.direction.ToString());
        	
        	anim.direction = QAbstractAnimation.Direction.Backward;
        	Assert.AreEqual(QAbstractAnimation.Direction.Backward.ToString(), anim.direction.ToString());
        	
        	anim.direction = QAbstractAnimation.Direction.Forward;
        	Assert.AreEqual(QAbstractAnimation.Direction.Forward.ToString(), anim.direction.ToString());
        }
        
        [Test]
        public unsafe void TestGroup()
        {            	
        	var anim = new TestableQAbstractAnimation();
        	var group = new DummyQAnimationGroup();
        	
        	group.AddAnimation(anim);         
        	
        	Assert.AreSame(group, anim.Group);	
        }
        
        [Test]
        public unsafe void TestLoopCount()
        {            	
        	var anim = new TestableQAbstractAnimation();
        	Assert.AreEqual(1, anim.LoopCount);
        	
        	anim.LoopCount = 10;
        	Assert.AreEqual(10, anim.LoopCount);
        }
        
        [Test]
        public unsafe void TestState()
        {          
        	var anim = new TestableQAbstractAnimation();
        	Assert.AreEqual(QAbstractAnimation.State.Stopped, anim.state);
        }
        
        [Test]
        public void TestTotalDuration()
        {
        	var anim = new TestableQAbstractAnimation();        	
        	Assert.AreEqual(10, anim.TotalDuration);
        	
        	anim.LoopCount = 5;
        	Assert.AreEqual(50, anim.TotalDuration);
        }
        
        [Test]
        public void TestAvoidJumpAtStart()
        {
        	var anim = new TestableQAbstractAnimation();      
        	anim.SetDuration(1000);
        	
        	anim.Start();
        	
        	System.Threading.Thread.Sleep(300);
        	
        	QCoreApplication.ProcessEvents();
        	Assert.Less(anim.CurrentTime, 50);
        }
        
        [Test]
        public void TestAvoidJumpAtStartWithStop()
        {
        	var anim = new TestableQAbstractAnimation();      
        	anim.SetDuration(1000);
        	
        	var anim2 = new TestableQAbstractAnimation();      
        	anim2.SetDuration(1000);
        	
        	var anim3 = new TestableQAbstractAnimation();      
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
        
        [Test]
        public void TestAvoidJumpAtStartWithRunning()
        {
        	var anim = new TestableQAbstractAnimation();      
        	anim.SetDuration(2000);
        	
        	var anim2 = new TestableQAbstractAnimation();      
        	anim2.SetDuration(1000);
        	
        	var anim3 = new TestableQAbstractAnimation();      
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

        private class TestableQAbstractAnimation : QAbstractAnimation
        {
        	private int _duration = 10;
        	
        	public override int Duration
        	{
        		get { return _duration; }
        	}
        	
        	protected override void UpdateCurrentTime(int value)
        	{
        		
        	}
        	
        	public void SetDuration(int val)
        	{
        		_duration = val;
        	}
        }

        private class DummyQAnimationGroup : QAnimationGroup
        {
            private int _duration = 10;

            public override int Duration
            {
                get { return _duration; }
            }

            protected override void UpdateCurrentTime(int value)
            {

            }

            public void SetDuration(int val)
            {
                _duration = val;
            }
        }
    }
}