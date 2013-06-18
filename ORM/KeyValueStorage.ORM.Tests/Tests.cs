using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.ORM.Tests.DB;
using NUnit.Framework;

namespace KeyValueStorage.ORM.Tests
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void SetUp()
        {
            KVStore.Initialize(new Func<Interfaces.IStoreProvider>(() => new KeyValueStorage.Redis.RedisStoreProvider(new ServiceStack.Redis.RedisClient())));
        }

        [Test]
        public void ContextTest()
        {
            using (var context = new TestKVContext())
            {
                context.TestEntitiesA.Add(new DB.BO.TestEntityA());
            }
        }
    }
}
