
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Testing.TestObjects;
using NUnit.Framework;

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
    }
}
