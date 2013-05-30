
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Testing.TestObjects;
using NUnit.Framework;

namespace KeyValueStorage.Testing
{
    public class ReusableTests
    {
        public void GetSetSingleSimpleItemTest(string key = "T1")
        {
            var bo = new TestBO_A()
            {
                Id = 1,
                Description = "Description"
            };

            KVStore.Instance.Set(key, bo);

            var boCheck = KVStore.Instance.Get<TestBO_A>(key);

            Assert.AreEqual(bo.Id, boCheck.Id);
            Assert.AreEqual(bo.Description, boCheck.Description);
        }
    }
}
