using System;
using System.Collections.Generic;
using System.Linq;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Stores;
using KeyValueStorage.Tools.Utility;
using KeyValueStorage.Tools.Utility.Relationships;
using KeyValueStorage.Tools.Utility.Strings;

namespace KeyValueStorage.Tools
{
	public interface IKvRoleProvider
	{
		IEnumerable<string> GetRoles();
		IEnumerable<string> GetUserRoles(string username);
		IEnumerable<string> GetUsersInRole(string rolename);
		bool UserIsInRole(string username, string rolename);
		void AddUserToRole(string username, string rolename);
		void RemoveUserFromRole(string username, string rolename);
	}

	public class KvRoleProvider : IKvRoleProvider
	{
		private readonly IKVStore _store;
	    private KVForeignKeyStoreRelationshipProvider _usersToRolesFK;
	    private KVForeignKeyStoreRelationshipProvider _rolesToUsersFK;

	    public KvRoleProvider(IKVStore store, string namespacePrefix = "URP:")
		{
			_store = new KeyTransformKVStore(store, new PrefixTransformer(namespacePrefix));
	        _usersToRolesFK = new KVForeignKeyStoreRelationshipProvider(_store, "Roles");
            _rolesToUsersFK = new KVForeignKeyStoreRelationshipProvider(_store, "Users");
		}

		public IEnumerable<string> GetRoles()
		{
            throw new NotImplementedException();
		}

		public IEnumerable<string> GetUserRoles(string username)
		{
            return new KeyWithRelationship(new RelationalKey(username), _usersToRolesFK)
                .GetReferences().Select(s => s.Value);
		}

		public IEnumerable<string> GetUsersInRole(string rolename)
		{
            return new KeyWithRelationship(new RelationalKey(rolename), _rolesToUsersFK)
                .GetReferences().Select(s => s.Value);
		}

		public bool UserIsInRole(string username, string rolename)
		{
            return GetUserRoles(username).Any(q => q == rolename);
		}

		public void AddUserToRole(string username, string rolename)
		{
		    var relationship = new KeyWithRelationship(new RelationalKey(username), _usersToRolesFK);
            relationship.Add(new RelationalKey(rolename), _rolesToUsersFK);
		}

		public void RemoveUserFromRole(string username, string rolename)
		{
		    var relationship = new KeyWithRelationship(new RelationalKey(username), _usersToRolesFK);
            relationship.Remove(new RelationalKey(rolename), _rolesToUsersFK);
		}
	}

	public class Role
	{
		public string Name { get; set; }
		IEnumerable<string> Users { get; set; } 
	}
}
