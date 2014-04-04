using System;
using System.Collections.Generic;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Structured;
using KeyValueStorage.Tools.Structured.Schema;
using Moq;
using NUnit.Framework;

namespace KeyValueStorage.Tools.UnitTests.Structured
{
    [TestFixture]
    public class KVRelationalStoreTests
    {
	    private StoreSchema _storeSchema;

		public KVRelationalStoreTests()
		{
			_storeSchema = new StoreSchema();
			_storeSchema.Add(
				new ObjectTypeSchema(typeof(TestObjA),
										 new Dictionary<Type, string>()
                                             {
                                                 {typeof (TestObjB), "ObjB"}
                                             }));

			_storeSchema.Add(
	new ObjectTypeSchema(typeof(TestObjB),
							 new Dictionary<Type, string>()
                                             {
                                                 {typeof (TestObjA), "ObjA"}
                                             }));
		}

	    [Test]
        public void ShouldSaveObject()
        {
            var mockStore = new Mock<IKVStore>();

			mockStore.Setup(m => m.GetNextSequenceValue("TestObjAs:i")).Returns(10).Verifiable();
	        var testObjA = new TestObjA() { SomeProp1 = "Something" };
			mockStore.Setup(m => m.Set("TestObjAs:10", testObjA)).Verifiable();

	        var store = new KVRelationalStore(mockStore.Object, _storeSchema);

	        var obj = store.New<TestObjA>();
	        obj.Value = testObjA;
            store.Save(obj);

            obj.Get<TestObjB>();


			mockStore.Verify();
        }

		[Test]
		public void ShouldLoadExistingObject()
		{
			var mockStore = new Mock<IKVStore>();
			var testObjA = new TestObjA() { SomeProp1 = "Something" };
			mockStore.Setup(m => m.Get<TestObjA>("TestObjAs:10")).Returns(testObjA);
			
			var store = new KVRelationalStore(mockStore.Object, _storeSchema);
			
			var item = store.Get<TestObjA>(10);

			Assert.IsNotNull(item.Value);
			Assert.AreEqual(testObjA.SomeProp1, item.Value.SomeProp1);
		}

		[Test]
		public void ShouldCreateNewObject()
		{
			
		}

		[Test]
		public void ShouldDeleteExistingObjectAndReferences()
		{
			
		}
    }

    public class TestObjA
    {
        public string SomeProp1 { get; set; }
    }

    public class TestObjB
    {
        public string SomeProp2 { get; set; }
    }
}
