
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

                context.Set(key, bo2, cas);
                ulong cas2 = cas;

                var bo3 = context.Get<TestBO_A>(key, out cas2);

                Assert.AreNotEqual(cas2, cas);

                Assert.AreEqual(bo2.Dump(), bo3.Dump());
                bo.Description = "DescriptionC";

                Assert.Throws<CASException>(() => context.Set(key, bo3, 0));
            }
        }
    }
}
