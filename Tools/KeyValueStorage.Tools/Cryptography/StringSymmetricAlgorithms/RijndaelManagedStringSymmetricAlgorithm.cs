using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace KeyValueStorage.Tools.Cryptography.StringSymmetricAlgorithms
{
    public class RijndaelManagedStringSymmetricAlgorithm : IStringSymmetricAlgorithm
    {
        private readonly string _initVector;
        private string _strPassword;

        public RijndaelManagedStringSymmetricAlgorithm(string strPassword, string initVector = "gure892309idfn18")
        {
            _strPassword = strPassword;
            _initVector = initVector;
        }

        private const int Keysize = 256;

        public string Encrypt(string plainText)
        {
            var initVectorBytes = Encoding.UTF8.GetBytes(_initVector);
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] cipherTextBytes;

            SymmetricAlgorithm symmetricAlgo = new RijndaelManaged();
            symmetricAlgo.Mode = CipherMode.CBC;
            var encryptor = CreateEncryptor(symmetricAlgo);

            using(var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                cipherTextBytes = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();
            }

            return Convert.ToBase64String(cipherTextBytes);
        }

        public string Decrypt(string cipherText)
        {
            var cipherTextBytes = Convert.FromBase64String(cipherText);
            var plainTextBytes = new byte[cipherTextBytes.Length];

            SymmetricAlgorithm symmetricAlgo = new RijndaelManaged();

            var decryptor = CreateDecryptor(symmetricAlgo);
            symmetricAlgo.Mode = CipherMode.CBC;

            int decryptedByteCount;

            using(var memoryStream = new MemoryStream(cipherTextBytes))
            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            {
                decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
            }

            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }

        private ICryptoTransform CreateEncryptor(SymmetricAlgorithm symmetricAlgo)
        {
            var initVectorBytes = Encoding.ASCII.GetBytes(_initVector);
            var password = new PasswordDeriveBytes(_strPassword, null);
            var keyBytes = password.GetBytes(Keysize / 8);

            return symmetricAlgo.CreateEncryptor(keyBytes, initVectorBytes);
        }

        private ICryptoTransform CreateDecryptor(SymmetricAlgorithm symmetricAlgo)
        {
            var initVectorBytes = Encoding.ASCII.GetBytes(_initVector);
            var password = new PasswordDeriveBytes(_strPassword, null);
            var keyBytes = password.GetBytes(Keysize/8);

            return symmetricAlgo.CreateDecryptor(keyBytes, initVectorBytes);
        }
    }
}