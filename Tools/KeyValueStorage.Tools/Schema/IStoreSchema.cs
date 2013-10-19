using System.Collections.Generic;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Utility.Relationships;

namespace KeyValueStorage.Tools.Schema
{
    public interface IStoreSchema : IEnumerable<IObjectTypeSchema>
    {
        IEnumerable<KeyWithRelationship> BuildKeyRelationships<T>(IKVStore underlyingStore, IRelationalKey key);
        IObjectTypeSchema GetObjectSchema<T>();
    }
}