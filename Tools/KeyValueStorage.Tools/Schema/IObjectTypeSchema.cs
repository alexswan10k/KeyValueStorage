using System;
using System.Collections.Generic;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Utility.Relationships;

namespace KeyValueStorage.Tools.Schema
{
    public interface IObjectTypeSchema : IEnumerable<KeyValuePair<Type, string>>
    {
        IEnumerable<KeyWithRelationship> BuildKeyRelationships(IKVStore store, IRelationalKey key);
        void AddRelationship<T>(string conceptualTableAlias = null);
        Type ObjectType { get; }
        KeyWithRelationship GetRelationshipFor<T>(IKVStore store, IRelationalKey key);
        string GetIncerementorKey();
        IRelationalKey GenerateKey(ulong sequenceValue);
    }
}