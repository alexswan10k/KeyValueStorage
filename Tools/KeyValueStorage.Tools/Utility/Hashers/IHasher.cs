namespace KeyValueStorage.Tools
{
	public interface IHasher
	{
		EncryptedData Encrypt(string stringToHash);
		bool Verify(string stringToVerify, EncryptedData data);
	}

	public class NullHasher : IHasher
	{
		public EncryptedData Encrypt(string stringToHash)
		{
			return new EncryptedData() { Hash = stringToHash, EncrType = typeof(NullHasher).FullName };
		}

		public bool Verify(string stringToVerify, EncryptedData data)
		{
			return data.Hash == stringToVerify;
		}
	}
}