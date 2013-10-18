using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Tools.Utility;

namespace KeyValueStorage.Tools
{
    public class KeyWithRelationshipFactory
    {
        private readonly Func<string, KeyWithRelationship> _relationshipFunc;

        public KeyWithRelationshipFactory(Func<string, KeyWithRelationship> relationshipFunc)
        {
            _relationshipFunc = relationshipFunc;
        }

        public KeyWithRelationship Get(string key)
        {
            return _relationshipFunc(key);
        }
    }

    public class KeyWithRelationship
    {
        private readonly string _key;
        private readonly IKVForeignKeyRelationshipProvider _kvForeignKeyRelationshipProvider;

        public KeyWithRelationship(string key, IKVForeignKeyRelationshipProvider kvForeignKeyRelationshipProvider)
        {
            _key = key;
            _kvForeignKeyRelationshipProvider = kvForeignKeyRelationshipProvider;
        }

        public string Key
        {
            get { return _key; }
        }

        public void Add(string foreignKey)
        {
            _kvForeignKeyRelationshipProvider.Add(Key, foreignKey);
            _kvForeignKeyRelationshipProvider.Add(foreignKey, Key);
        }

        public void Remove(string foreignKey)
        {
            _kvForeignKeyRelationshipProvider.Remove(Key, foreignKey);
            _kvForeignKeyRelationshipProvider.Remove(foreignKey, Key);
        }

        public void Add(KeyWithRelationship keyWithRelationship)
        {
            Add(keyWithRelationship.Key);
        }

        public void Remove(KeyWithRelationship keyWithRelationship)
        {
            Remove(keyWithRelationship.Key);
        }

        public IEnumerable<KeyWithRelationship> GetRelationships()
        {
            return _kvForeignKeyRelationshipProvider.GetRelationships(Key);
        }

        public IEnumerable<string> GetReferences()
        {
            return _kvForeignKeyRelationshipProvider.GetKeys(Key);
        }
    }
}
