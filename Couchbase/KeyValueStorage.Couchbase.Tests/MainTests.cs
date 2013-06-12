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
            var cluster = new global::Couchbase.Management.CouchbaseCluster(config);
            var stats = client.Stats();
            KVStore.Initialize(new Func<Interfaces.IStoreProvider>(() => new Couchbase.CouchbaseStoreProvider(client, cluster, config.Bucket)));
        }

        [Test]
        public void CRUDSingleSimpleItemTest()
        {
            ReusableTests.CRUDSingleSimpleItemTest();
        }

        [Test]
        public void SequenceTest()
        {
            ReusableTests.SequenceTest();
        }

        [Test]
        public void CASTest()
        {
            
            ReusableTests.CASTest();
        }

        //not implemented as of yet
        //[Test]
        //public void KeysStartingWithTest()
        //{
        //    ReusableTests.KeysStartingWithTest();
        //}

        [Test]
        public void CollectionsTest()
        {
            ReusableTests.CollectionsTest();
        }
    }
}
