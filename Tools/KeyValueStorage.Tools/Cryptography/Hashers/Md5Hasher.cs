using System.Security.Cryptography;
using KeyValueStorage.Tools.Extensions;

namespace KeyValueStorage.Tools.Cryptography.Hashers
{
	public class Md5Hasher : IHasher
	{
		private MD5 _algo;

		public Md5Hasher()
		{
			_algo = MD5.Create();
		}

		public HashedData ComputeHash(string stringToHash)
		{
			var hash = _algo.ComputeHash(stringToHash.GetBytes());
			return new HashedData() { Hash = hash.GetString(), EncrType = typeof(Md5Hasher).FullName };
		}

		public bool Verify(string stringToVerify, HashedData data)
		{
			var hash = _algo.ComputeHash(stringToVerify.GetBytes());
			return data.Hash == hash.GetString();
		}
	}
}