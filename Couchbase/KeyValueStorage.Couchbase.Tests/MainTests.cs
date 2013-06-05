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
            var config = new global::Couchbase.Configuration.CouchbaseClientConfiguration()
            {
                Bucket = "default",
                Password="password"
                
            };
            config.Urls.Add(new Uri("http://127.0.0.1:8091/pools"));

            var client = new global::Couchbase.CouchbaseClient(config);
            var stats = client.Stats();
            KVStore.Initialize(new Func<Interfaces.IStoreProvider>(() => new Couchbase.CouchbaseStoreProvider(client)));
        }

        [Test]
        public void CRUDSingleSimpleItemTest()
        {
            ReusableTests.CRUDSingleSimpleItemTest();
        }

        [Test]
        public void CASTest()
        {
            ReusableTests.CASTest();
        }

        [Test]
        public void SequenceTest()
        {
            ReusableTests.SequenceTest();
        }
    }
}
