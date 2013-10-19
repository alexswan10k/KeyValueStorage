using System;

namespace KeyValueStorage.Tools.Utility.Relationships
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
}