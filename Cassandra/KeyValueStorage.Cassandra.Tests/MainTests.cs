
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cassandra;
using KeyValueStorage.Testing;
using NUnit.Framework;

namespace KeyValueStorage.Cassandra.Tests
{
    [TestFixture]
    public class MainTests
    {
        [SetUp]
        public void SetUp()
        {
            global::Cassandra.Cluster cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
            Session session = cluster.Connect();
            KVStore.Initialize(new Func<Interfaces.IStoreProvider>(() => new KeyValueStorage.Cassandra.CassandraStoreProvider(session)));
        }

        [Test]
        public void CRUDSingleSimpleItemTest()
        {
            ReusableTests.CRUDSingleSimpleItemTest();
        }
    }
}
