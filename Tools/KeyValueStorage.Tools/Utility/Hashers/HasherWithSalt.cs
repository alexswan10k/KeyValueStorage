using System.Security.Cryptography;

namespace KeyValueStorage.Tools
{
	public abstract class HasherWithSalt : IHasher
	{
		private HashAlgorithm _algo;
		private IRandomCharacterGenerator _generator;

		public HasherWithSalt(HashAlgorithm algorithm, IRandomCharacterGenerator saltGenerator = null)
		{
			_algo = algorithm;
			_generator = saltGenerator?? new SimpleRandomCharacterGenerator(16);
		}

		public EncryptedData Encrypt(string stringToHash)
		{
			var salt = _generator.Generate();
			var hash = _ComputeHash(stringToHash, salt);
			return new EncryptedData() { Hash = hash, Salt = salt, EncrType = typeof(Md5HasherWithSalt).FullName };
		}

		public bool Verify(string stringToVerify, EncryptedData data)
		{
			return _ComputeHash(stringToVerify, data.Salt) == data.Hash;
		}

		protected virtual string _ComputeHash(string stringToHash, string salt)
		{
			return _algo.ComputeHash((stringToHash + salt).GetBytes()).GetString();
		}
	}
}