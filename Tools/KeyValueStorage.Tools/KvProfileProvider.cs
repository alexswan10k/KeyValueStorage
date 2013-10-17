using System.Collections.Generic;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Tools
{
	public class ProfileProvider<T>  where T : new()
	{
		private readonly IKVStore _store;

		public ProfileProvider(IKVStore store, string namespacePrefix = "UPP:")
		{
			_store = new PrefixKVStore(namespacePrefix, store);
		}

		public ProfileWrapper<T> GetProfile(string username)
		{
			return _store.Get<ProfileWrapper<T>>(username);
		}

		public void UpdateProfile(string username, ProfileWrapper<T> profileWrapper)
		{
			_store.Set(username, profileWrapper);
		}

		public static ProfileProvider<StrDictProfile> GetStrDictProfile(IKVStore store)
		{
			return new ProfileProvider<StrDictProfile>(store);
		}
	}

	public class StrDictProfile
	{
		IDictionary<string, object> Settings { get; set; } 
		public StrDictProfile()
		{
			Settings = new Dictionary<string, object>();
		}
	}

	public class ProfileWrapper<T>
	{
		public T Profile { get; set; }
	}
}
