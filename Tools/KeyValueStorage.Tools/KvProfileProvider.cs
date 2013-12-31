using System.Collections.Generic;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Stores;
using KeyValueStorage.Tools.Utility.Strings;

namespace KeyValueStorage.Tools
{
	public interface IKVProfileProvider<T> where T : Profile, new()
	{
		T GetProfile(string username);
		void UpdateProfile(string username, T profile);
	}

	public class KVProfileProvider<T> : IKVProfileProvider<T> where T : Profile, new()
	{
		private readonly IKVStore _store;

		public KVProfileProvider(IKVStore store, string namespacePrefix = "UPP:")
		{
            _store = new KeyTransformKVStore(store, new PrefixTransformer(namespacePrefix));
		}

		public T GetProfile(string username)
		{
			return _store.Get<T>(username);
		}

		public void UpdateProfile(string username, T profile)
		{
			_store.Set(username, profile);
		}

		public static KVProfileProvider<StrDictProfile> GetStringDictProfile(IKVStore store)
		{
			return new KVProfileProvider<StrDictProfile>(store);
		}
	}

	public abstract class Profile
	{
		
	}

	public class StrDictProfile : Profile
	{
		public IDictionary<string, object> Settings { get; set; } 
		public StrDictProfile()
		{
			Settings = new Dictionary<string, object>();
		}
	}
}
