using System.Collections.Generic;

namespace KeyValueStorage.Tools.Utility.Relationships
{
    public interface IKVForeignKeyRelationshipProvider
    {
        void Add(Key key, Key p1);
        void Remove(Key key, Key p1);
        IEnumerable<KeyWithRelationship> GetRelationships(Key key);
        IEnumerable<Key> GetKeys(Key key);
    }
}