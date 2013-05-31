using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Testing;
using NUnit.Framework;
using KeyValueStorage;

namespace KeyValueStorage.Couchbase.Tests
{
    [TestFixture]
    public class MainTests
    {
        [SetUp]
        public void SetUp()
        {
            var client = new global::Couchbase.CouchbaseClient("Test", "test");
            KVStore.Initialize(new Func<Interfaces.IStoreProvider>(() => new Couchbase.CouchbaseStoreProvider(client)));
        }

        [Test]
        public void GetSetSingleSimpleItemTest()
        {
            ReusableTests.GetSetSingleSimpleItemTest();
        }
    }
}
