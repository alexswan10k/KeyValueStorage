using System;
using System.Linq;
using KeyValueStorage.Interfaces;
using NUnit.Framework;
using Moq;
using KeyValueStorage.Tools.Collections;
			 
namespace KeyValueStorage.Tools.Tests.Collections
{
	[TestFixture]
	public class QueueTests
	{
		[Test]
		public void ShouldEnqueue()
		{
			var mockStore = new Mock<IKVStore>();

			mockStore.Setup(m => m.GetNextSequenceValue("Q:iA")).Returns(1);

			var queue = new Queue<TestObjA>(mockStore.Object, "Q");
			var a = new TestObjA() { SomeProp1 = "test1"};
			var b = new TestObjA() { SomeProp1 = "test2" };
			var c = new TestObjA() { SomeProp1 = "test3" };
			queue.Enqueue(a);

			mockStore.Verify(m => m.Set("Q:1", a));
		}

		public void ShouldDequeue()
		{
			var mockStore = new Mock<IKVStore>();
			var a = new TestObjA() { SomeProp1 = "test1" };

			mockStore.Setup(m => m.GetNextSequenceValue("Q:iA")).Returns(3);
			mockStore.Setup(m => m.Get<TestObjA>("Q:3")).Returns(a);

			var queue = new Queue<TestObjA>(mockStore.Object, "Q");

			var res = queue.Dequeue();
			Assert.AreEqual(a, res);
			mockStore.Verify(m => m.Delete("Q:3"),Times.Once);
		}
	}
}