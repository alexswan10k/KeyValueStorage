using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.ORM;
using KeyValueStorage.ORM.Tests.DB.BO;

namespace KeyValueStorage.ORM.Tests.DB
{
    public class TestKVContext : ContextBase
    {
        public KVSDbSet<TestEntityA> TestEntitiesA { get; set; }
        public KVSDbSet<TestEntityB> TestEntitiesB { get; set; }
        public KVSDbSet<TestEntityC> TestEntitiesC { get; set; }
    }
}
