using System;
using System.Collections;
using System.Collections.Generic;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Utility.Relationships;

namespace KeyValueStorage.Tools.Schema
{
    public class StoreSchema : IStoreSchema
    {
        private readonly Dictionary<Type, IObjectTypeSchema> _schemaObjects;

        public StoreSchema(Dictionary<Type, IObjectTypeSchema> schemaObjects = null)
        {
            _schemaObjects = schemaObjects ?? new Dictionary<Type, IObjectTypeSchema>();
        }

        public IEnumerable<KeyWithRelationship> BuildKeyRelationships<T>(IKVStore underlyingStore, IRelationalKey key)
        {
            IObjectTypeSchema objectTypeSchema;
            if(!_schemaObjects.TryGetValue(typeof(T), out objectTypeSchema))
                throw new InvalidOperationException("No object schema available for "+ typeof(T));

            return objectTypeSchema.BuildKeyRelationships(underlyingStore, key);
        }

        public IObjectTypeSchema GetObjectSchema<T>()
        {
            IObjectTypeSchema objectTypeSchema;

            if (!_schemaObjects.TryGetValue(typeof(T), out objectTypeSchema))
                return null;

            return objectTypeSchema;
        }

        public void Add(IObjectTypeSchema objectTypeSchema)
        {
            _schemaObjects.Add(objectTypeSchema.ObjectType, objectTypeSchema);
        }

        public IEnumerator<IObjectTypeSchema> GetEnumerator()
        {
            return _schemaObjects.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _schemaObjects.Values.GetEnumerator();
        }
    }
}