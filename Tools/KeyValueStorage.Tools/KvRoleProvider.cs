using System;
using System.Collections.Generic;
using System.Linq;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Tools
{
	public class KvRoleProvider
	{
		private readonly IKVStore _store;
		private readonly string _usersNamespacePrefix;
		private string _rolesNamespacePrefix;

		public KvRoleProvider(IKVStore store, string namespacePrefix = "URP:")
		{
			_store = store;
			_usersNamespacePrefix = namespacePrefix + "U:";
			_rolesNamespacePrefix = namespacePrefix + "R:";
		}

		public IEnumerable<string> GetRoles()
		{
            throw new NotImplementedException();
		}

		public IEnumerable<string> GetUserRoles(string username)
		{
			return _store.GetCollection<string>(_usersNamespacePrefix + username);
		}

		public IEnumerable<string> GetUsersInRole(string rolename)
		{
			return _store.GetCollection<string>(rolename);
		}

		public bool UserIsInRole(string username, string rolename)
		{
			return _store.GetCollection<string>(_usersNamespacePrefix + username).Any(q=>q.Equals(rolename, StringComparison.OrdinalIgnoreCase));
		}

		public void AddUserToRole(string username, string rolename)
		{
			
		}

		public void RemoveUserFromRole(string username, string rolename)
		{
			
		}
	}

	public class Role
	{
		public string Name { get; set; }
		IEnumerable<string> Users { get; set; } 
	}
}
