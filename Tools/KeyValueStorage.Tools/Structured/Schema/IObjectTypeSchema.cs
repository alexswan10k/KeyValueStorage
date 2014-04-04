using System;
using System.Collections.Generic;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Utility.Relationships;

namespace KeyValueStorage.Tools.Structured.Schema
{
    public interface IObjectTypeSchema : IEnumerable<KeyValuePair<Type, string>>
    {
        IEnumerable<KeyWithRelationship> BuildKeyRelationships(IKVStore store, Key key);
        void AddRelationship<T>(string conceptualTableAlias = null);
        Type ObjectType { get; }
	    string ConceptualTableKeyPrefix { get; }
	    KeyWithRelationship GetRelationshipFor<T>(IKVStore store, Key key);
        Key GenerateKey(IKVStore store);
    }
}