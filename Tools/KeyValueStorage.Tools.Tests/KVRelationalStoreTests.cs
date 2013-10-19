using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Schema;
using Moq;
using NUnit.Framework;

namespace KeyValueStorage.Tools.Tests
{
    [TestFixture]
    public class KVRelationalStoreTests
    {
        [Test]
        public void T1()
        {
            var mockStore = new Mock<IKVStore>();
            var storeSchema = new StoreSchema();
            storeSchema.Add(
                new ObjectTypeSchema(typeof (TestObjA),
                                         new Dictionary<Type, string>()
                                             {
                                                 {typeof (TestObjB), "ObjB"}
                                             }));

            storeSchema.Add(
    new ObjectTypeSchema(typeof(TestObjB),
                             new Dictionary<Type, string>()
                                             {
                                                 {typeof (TestObjA), "ObjA"}
                                             }));


            var store = new KVRelationalStore(mockStore.Object, storeSchema);


            var obj = store.Build<TestObjA>();
            obj.AddRelationship<TestObjA>();

            obj.Get<TestObjB>();
        }
    }

    public class TestObjA
    {
        private string SomeProp1 { get; set; }
    }

    public class TestObjB
    {
        public string SomeProp2 { get; set; }
    }
}
