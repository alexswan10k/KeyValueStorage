using System.Security.Cryptography;
using KeyValueStorage.Tools.Extensions;
using KeyValueStorage.Tools.Utility.CharGenerators;

namespace KeyValueStorage.Tools.Cryptography.Hashers
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

		public HashedData ComputeHash(string stringToHash)
		{
			var salt = _generator.Generate();
			var hash = _ComputeHash(stringToHash, salt);
			return new HashedData() { Hash = hash, Salt = salt, EncrType = typeof(Md5HasherWithSalt).FullName };
		}

		public bool Verify(string stringToVerify, HashedData data)
		{
			return _ComputeHash(stringToVerify, data.Salt) == data.Hash;
		}

		protected virtual string _ComputeHash(string stringToHash, string salt)
		{
			return _algo.ComputeHash((stringToHash + salt).GetBytes()).GetString();
		}
	}
}