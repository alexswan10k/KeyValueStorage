using System.Security.Cryptography;

namespace KeyValueStorage.Tools
{
	public class Md5Hasher : IHasher
	{
		private MD5 _algo;

		public Md5Hasher()
		{
			_algo = MD5.Create();
		}

		public EncryptedData Encrypt(string stringToHash)
		{
			var hash = _algo.ComputeHash(stringToHash.GetBytes());
			return new EncryptedData() { Hash = hash.GetString(), EncrType = typeof(Md5Hasher).FullName };
		}

		public bool Verify(string stringToVerify, EncryptedData data)
		{
			var hash = _algo.ComputeHash(stringToVerify.GetBytes());
			return data.Hash == hash.GetString();
		}
	}
}