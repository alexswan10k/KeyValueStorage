using System;
using System.Collections.Generic;
using System.Linq;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Stores;
using KeyValueStorage.Tools.Utility;
using KeyValueStorage.Tools.Utility.Strings;

namespace KeyValueStorage.Tools
{
	public class KvRoleProvider
	{
		private readonly IKVStore _store;
	    private readonly KeyWithRelationshipFactory _relationshipFactory;

	    public KvRoleProvider(IKVStore store, string namespacePrefix = "URP:")
		{
			_store = new KeyTransformKVStore(store, new PrefixTransformer(namespacePrefix));
	        _relationshipFactory = new KeyWithRelationshipFactory(s => new KeyWithRelationship(s, new KVForeignKeyRelationshipProvider(_store)));

		}

		public IEnumerable<string> GetRoles()
		{
            throw new NotImplementedException();
		}

		public IEnumerable<string> GetUserRoles(string username)
		{
		    return _relationshipFactory
                .Get(username)
                .GetReferences();
		}

		public IEnumerable<string> GetUsersInRole(string rolename)
		{
		    return _relationshipFactory.Get(rolename)
                .GetReferences();
		}

		public bool UserIsInRole(string username, string rolename)
		{
            return GetUserRoles(username).Any(q => q == rolename);
		}

		public void AddUserToRole(string username, string rolename)
		{
		    var relationship = _relationshipFactory.Get(username);
            relationship.Add(rolename);
		}

		public void RemoveUserFromRole(string username, string rolename)
		{
		    var relationship = _relationshipFactory.Get(username);
            relationship.Remove(rolename);
		}
	}

	public class Role
	{
		public string Name { get; set; }
		IEnumerable<string> Users { get; set; } 
	}
}
