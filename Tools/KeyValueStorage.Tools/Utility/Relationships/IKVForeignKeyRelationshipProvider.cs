using System.Collections.Generic;

namespace KeyValueStorage.Tools.Utility.Relationships
{
    public interface IKVForeignKeyRelationshipProvider
    {
        void Add(IRelationalKey key, IRelationalKey p1);
        void Remove(IRelationalKey key, IRelationalKey p1);
        IEnumerable<KeyWithRelationship> GetRelationships(IRelationalKey key);
        IEnumerable<IRelationalKey> GetKeys(IRelationalKey key);
    }
}