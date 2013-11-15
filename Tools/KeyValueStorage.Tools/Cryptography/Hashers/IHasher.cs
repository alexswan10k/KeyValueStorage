namespace KeyValueStorage.Tools.Cryptography.Hashers
{
	public interface IHasher
	{
		HashedData ComputeHash(string stringToHash);
		bool Verify(string stringToVerify, HashedData data);
	}

	public class NullHasher : IHasher
	{
		public HashedData ComputeHash(string stringToHash)
		{
			return new HashedData() { Hash = stringToHash, EncrType = typeof(NullHasher).FullName };
		}

		public bool Verify(string stringToVerify, HashedData data)
		{
			return data.Hash == stringToVerify;
		}
	}
}