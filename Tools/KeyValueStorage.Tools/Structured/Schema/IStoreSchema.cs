using System.Collections.Generic;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Utility.Relationships;

namespace KeyValueStorage.Tools.Structured.Schema
{
    public interface IStoreSchema : IEnumerable<IObjectTypeSchema>
    {
        IEnumerable<KeyWithRelationship> BuildKeyRelationships<T>(IKVStore underlyingStore, Key key);
        IObjectTypeSchema GetObjectSchema<T>();
    }
}