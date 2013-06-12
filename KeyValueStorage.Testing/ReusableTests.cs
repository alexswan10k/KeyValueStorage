
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Exceptions;
using KeyValueStorage.Testing.TestObjects;
using NUnit.Framework;
using ServiceStack.Text;

namespace KeyValueStorage.Testing
{
    public static class ReusableTests
    {
        public static void CRUDSingleSimpleItemTest(string key = "T1")
        {
            using (var context = KVStore.Factory.Get())
            {
                var bo = new TestBO_A()
                {
                    Id = 1,
                    Description = "Description"
                };

                context.Set(key, bo);

                var boCheck = context.Get<TestBO_A>(key);

                Assert.AreEqual(bo.Id, boCheck.Id);
                Assert.AreEqual(bo.Description, boCheck.Description);

                var bo2 = new TestBO_A()
                {
                    Id = 4,
                    Description = "Description2nd"
                };

                context.Set(key, bo2);

                var bo2Check = context.Get<TestBO_A>(key);

                Assert.AreEqual(bo2.Id, bo2Check.Id);
                Assert.AreEqual(bo2.Description, bo2Check.Description);

                context.Delete(key);

                var bo3Check = context.Get<TestBO_A>(key);

                Assert.IsNull(bo3Check);
            }
        }

        public static void SequenceTest()
        {
            using (var context = KVStore.Factory.Get())
            {
                context.Delete("seq1");
                context.Delete("seq2");
                var val = context.GetNextSequenceValue("seq1");
                Assert.AreEqual(1, val);
                val = context.GetNextSequenceValue("seq1");
                Assert.AreEqual(2, val);
                val = context.GetNextSequenceValue("seq1");
                Assert.AreEqual(3, val);
                val = context.GetNextSequenceValue("seq1", 2);
                Assert.AreEqual(5, val);
                val = context.GetNextSequenceValue("seq2");
                Assert.AreEqual(1, val);
                val = context.GetNextSequenceValue("seq2",17);
                Assert.AreEqual(18, val);
                val = context.GetNextSequenceValue("seq1");
                Assert.AreEqual(6, val);
            }
        }

        public static void CASTest(string key = "CT1")
        {
            using (var context = KVStore.Factory.Get())
            {
                var bo = new TestBO_A()
                {
                    Id = 1,
                    Description = "Description"
                };

                context.Set(key, bo);

                ulong cas;
                var bo2 = context.Get<TestBO_A>(key, out cas);
                Assert.AreEqual(bo.Dump(), bo2.Dump());
                bo.Description = "DescriptionB";

                context.Set(key, bo, cas);
                ulong cas2 = cas;

                var bo3 = context.Get<TestBO_A>(key, out cas2);

                Assert.AreNotEqual(cas2, cas);

                Assert.AreEqual(bo.Dump(), bo3.Dump());
                bo.Description = "DescriptionC";

                //push on the CAS by 1 to fix cb
                context.Get<TestBO_A>(key, out cas);
                context.Set(key, bo, cas);

                Assert.Throws<CASException>(() => context.Set(key, bo3, 1));
            }
        }

        public static void KeysStartingWithTest()
        {
            //insert some keys

            using (var context = KVStore.Factory.Get())
            {
                context.Set("K1A:1", true);
                context.Set("K1A:2", true);
                context.Set("K1A:3", true);

                context.Set("K1B:1", 1);
                context.Set("K1B:2", 2);
                context.Set("K1B:3", 3);
                context.Set("K1B:4", 4);

                context.Set("K2A:1", "one");
                context.Set("K2A:2", "two");
                context.Set("K2A:3", "three");

                context.Set("K2A:2Sub:1", "subVal");

                context.Set("K3:1", true);
                context.Set("K3:2", true);

                var keys = context.GetKeysStartingWith("K1A");

                Assert.AreEqual(3, keys.Count());
                Assert.IsTrue(keys.All(q => q.StartsWith("K1A")));


                keys = context.GetKeysStartingWith("K1B");

                Assert.AreEqual(4, keys.Count());
                Assert.IsTrue(keys.All(q => q.StartsWith("K1B")));


                keys = context.GetKeysStartingWith("K2A");

                Assert.AreEqual(4, keys.Count());
                Assert.IsTrue(keys.All(q => q.StartsWith("K2A")));

                keys = context.GetKeysStartingWith("K2A:2Sub");

                Assert.AreEqual(1, keys.Count());
            }
        }

        public static void ValuesStartingWithTest()
        {
            using (var context = KVStore.Factory.Get())
            {
                var k1a1 = new TestBO_A() { Description = "k1a1" };
                var k1a2 = new TestBO_B() { SomeInfo = "k1a2" };
                var k1a3 = new TestBO_A() { Description = "k1a3" };

                var k1b1 = new TestBO_A() { Description = "k1b1" };
                var k1b2 = new TestBO_B() { SomeInfo = "k1b2" };
                var k1b3 = new TestBO_A() { Description = "k1b3" };
                var k1b4 = true;

                var k2a1 = new TestBO_A() { Description = "k2a1" };
                var k2a2 = new TestBO_B() { SomeInfo = "k2a2" };
                var k2a3 = new TestBO_A() { Description = "k2a3" };

                context.Set("K1A:1", k1a1);
                context.Set("K1A:2", k1a2);
                context.Set("K1A:3", k1a3);

                context.Set("K1B:1", k1b1);
                context.Set("K1B:2", k1b2);
                context.Set("K1B:3", k1b3);
                context.Set("K1B:4", k1b4);

                context.Set("K2A:1", k2a1);
                context.Set("K2A:2", k2a2);
                context.Set("K2A:3", k2a3);

                var k1A = context.GetStartingWith<TestBO_A>("K1A");

                Assert.AreEqual(2, k1A.Count());

                Assert.AreEqual(k1A.ElementAt(0).Dump(), k1a1.Dump());
                Assert.AreEqual(k1A.ElementAt(1).Dump(), k1a3.Dump());

                var k1B_1st = context.GetStartingWith<TestBO_A>("K1B");

                Assert.AreEqual(2, k1B_1st.Count());

                Assert.AreEqual(k1B_1st.ElementAt(0).Dump(), k1b1.Dump());
                Assert.AreEqual(k1B_1st.ElementAt(1).Dump(), k1b3.Dump());

                var k1B_2nd = context.GetStartingWith<TestBO_B>("K1B:");

                Assert.AreEqual(1, k1B_2nd.Count());
                Assert.AreEqual(k1B_2nd.ElementAt(0).Dump(), k1b2.Dump());

                var k1b_3rd = context.GetStartingWith<bool>("K1B");

                Assert.AreEqual(1, k1b_3rd.Count());
            }
        }

        public static void CollectionsTest(string key = "coll")
        {
            using (var context = KVStore.Factory.Get())
            {
                IList<string> collection = new List<string>();
                collection.Add("this");
                collection.Add("is");
                collection.Add("a");
                collection.Add("collection");
                context.SetCollection(key, collection);

                var collCheck = context.GetCollection<string>(key);
                Assert.AreEqual(collection.Dump(), collCheck.Dump());

                context.AppendToCollection<string>(key, "of");
                context.AppendToCollection<string>(key, "strings");

                Assert.AreEqual("strings", context.GetCollection<string>(key).Last());
                Assert.AreEqual(6, context.GetCollection<string>(key).Count());

                context.RemoveFromCollection(key, "is");
                context.RemoveFromCollection(key, "a");

                Assert.AreEqual(4, context.GetCollection<string>(key).Count());
            }
        }
    }
}
