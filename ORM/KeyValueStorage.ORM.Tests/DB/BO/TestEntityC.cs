using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.ORM.Tests.DB.BO
{
    public class TestEntityC
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public virtual KVSCollection<TestEntityB> EntitiesB { get; set; }
        public virtual KVSCollection<TestEntityA> EntitiesA { get; set; }
    }
}
