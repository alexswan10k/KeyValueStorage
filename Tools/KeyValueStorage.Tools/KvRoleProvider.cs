using System;
using System.Collections.Generic;
using System.Linq;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Utility;

namespace KeyValueStorage.Tools
{
	public class KvRoleProvider
	{
		private readonly IKVStore _store;
	    private readonly string _namespacePrefix;
	    private readonly string _userPrefix = "U:";
	    private readonly string _rolePrefix = "R:";
	    private KeyWithRelationshipFactory _relationshipFactory;

	    public KvRoleProvider(IKVStore store, string namespacePrefix = "URP:")
		{
			_store = store;
	        _namespacePrefix = namespacePrefix;
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

        private string GetTransformedUsername(string username)
        {
            return _namespacePrefix + username + _userPrefix;
        }

        private string GetTransformedRoleName(string roleName)
        {
            return _namespacePrefix + roleName + _rolePrefix;
        }
	}

	public class Role
	{
		public string Name { get; set; }
		IEnumerable<string> Users { get; set; } 
	}
}
