using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Tools.Encryption
{
	public class CryptoSerialiser : ITextSerializer
	{
		private ITextSerializer _baseSerialiser;
		private readonly SymmetricAlgorithm _encryptionAlgorithm;

		public CryptoSerialiser(ITextSerializer baseSerialiser, System.Security.Cryptography.SymmetricAlgorithm encryptionAlgorithm)
		{
			_baseSerialiser = baseSerialiser;
			_encryptionAlgorithm = encryptionAlgorithm;
		}

		public string Serialize<T>(T item)
		{
			var enc = _encryptionAlgorithm.CreateEncryptor();
			return _baseSerialiser.Serialize(item);
		}

		public T Deserialize<T>(string itemSerialized)
		{
			var dec = _encryptionAlgorithm.CreateDecryptor()
			return _baseSerialiser.Deserialize<T>(itemSerialized);
		}

		private static byte[] _RsaEncrypt(byte[] byteEncrypt, RSAParameters rsaInfo, bool isOaep)
		{
			try
			{
				byte[] encryptedData;
				//Create a new instance of RSACryptoServiceProvider.
				using (var rsa = new RSACryptoServiceProvider())
				{

					//Import the RSA Key information. This only needs
					//toinclude the public key information.
					rsa.ImportParameters(rsaInfo);

					//Encrypt the passed byte array and specify OAEP padding.
					encryptedData = rsa.Encrypt(byteEncrypt, isOaep);
				}
				return encryptedData;
			}
			//Catch and display a CryptographicException
			//to the console.
			catch (CryptographicException e)
			{
				Console.WriteLine(e.Message);

				return null;
			}

		}
	}


}
