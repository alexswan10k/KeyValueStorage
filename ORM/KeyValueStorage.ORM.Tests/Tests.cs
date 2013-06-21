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
        public void ContextAddAndDeleteTestSimple()
        {
            using (var context = new TestKVContext())
            {
                var a1 = new DB.BO.TestEntityA(){ Description = "a1"};
                var a2 = new DB.BO.TestEntityA(){ Description = "a2"};
                
                context.TestEntitiesA.Add(a1);
                context.TestEntitiesA.Add(a2);
                context.SaveChanges();
            }

            using (var context = new TestKVContext())
            {
                Assert.AreEqual(2, context.TestEntitiesA.Count);
                var a1 = context.TestEntitiesA.Single(q => q.Description == "a1");
                var a2 = context.TestEntitiesA.Single(q => q.Description == "a2");
                Assert.Greater(a1.Id, 0);
                Assert.Greater(a2.Id, 0);

                Assert.AreEqual(a1, context.TestEntitiesA.GetById((ulong)a1.Id));
                Assert.AreEqual(a2, context.TestEntitiesA.GetById((ulong)a2.Id));

                context.TestEntitiesA.Remove(a1);
                context.TestEntitiesA.Remove(a2);
                context.SaveChanges();
            }

            using (var context = new TestKVContext())
            {
                Assert.AreEqual(0, context.TestEntitiesA.Count);
            }
        }

        [Test]
        public void ContextAddTestRelational()
        {
            using (var context = new TestKVContext())
            {
                var a1 = new DB.BO.TestEntityA() { Description = "a1" };
                var a2 = new DB.BO.TestEntityA() { Description = "a2" };
                context.TestEntitiesA.CreateProxy(a1);
                var b1 = new DB.BO.TestEntityB() { Description = "b1" };
                var c1 = new DB.BO.TestEntityC() { Description = "c1", EntitiesA = new KVSCollection<DB.BO.TestEntityA>() { a1, a2 }, EntitiesB = new KVSCollection<DB.BO.TestEntityB>(){ b1 } };

                context.TestEntitiesC.Add(c1);
            }

            using (var context = new TestKVContext())
            {
                Assert.AreEqual(2, context.TestEntitiesA.Count);
                Assert.AreEqual(1, context.TestEntitiesB.Count);
                Assert.AreEqual(1, context.TestEntitiesC.Count);

                var a1 = context.TestEntitiesA.Single(q => q.Description == "a1");
                var a2 = context.TestEntitiesA.Single(q => q.Description == "a2");

                var b1 = context.TestEntitiesB.Single(q => q.Description == "b1");
                var c1 = context.TestEntitiesC.Single(q => q.Description == "c1");

                Assert.AreEqual(c1, b1.EntityC);
                Assert.AreEqual(c1.EntitiesB.First(), b1);
                Assert.AreEqual(a1, c1.EntitiesA.Single(q => q.Description == "a1"));
                Assert.AreEqual(a2, c1.EntitiesA.Single(q => q.Description == "a2"));

                Assert.AreEqual(c1, a1.EntitiesC.Single());
                Assert.AreEqual(c1, a2.EntitiesC.Single());

                context.TestEntitiesA.Remove(a1);

                Assert.AreEqual(1, c1.EntitiesA.Count);
                context.TestEntitiesB.Remove(b1);
                Assert.AreEqual(0, context.TestEntitiesB.Count);
                Assert.AreEqual(0, c1.EntitiesB.Count);

                context.TestEntitiesC.Remove(c1);
                Assert.AreEqual(0, context.TestEntitiesC.Count);
                Assert.AreEqual(0, a2.EntitiesC.Count);

                context.TestEntitiesA.Remove(a2);

                Assert.AreEqual(0, context.TestEntitiesA.Count);
            }
        }
    }
}
