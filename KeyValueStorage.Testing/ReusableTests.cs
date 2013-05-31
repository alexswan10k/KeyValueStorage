
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
        public static void GetSetSingleSimpleItemTest(string key = "T1")
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
            }
        }
    }
}
