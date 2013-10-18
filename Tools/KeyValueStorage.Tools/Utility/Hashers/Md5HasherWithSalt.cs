using System.Security.Cryptography;
using KeyValueStorage.Tools.Utility.CharGenerators;

namespace KeyValueStorage.Tools.Utility.Hashers
{
	public class Md5HasherWithSalt : HasherWithSalt
	{
		public Md5HasherWithSalt(IRandomCharacterGenerator saltGenerator = null)
			:base(MD5.Create(), saltGenerator)
		{
		}
	}
}