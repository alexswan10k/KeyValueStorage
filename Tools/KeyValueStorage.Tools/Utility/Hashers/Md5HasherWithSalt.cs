using System.Security.Cryptography;

namespace KeyValueStorage.Tools
{
	public class Md5HasherWithSalt : HasherWithSalt
	{
		public Md5HasherWithSalt(IRandomCharacterGenerator saltGenerator = null)
			:base(MD5.Create(), saltGenerator)
		{
		}
	}
}