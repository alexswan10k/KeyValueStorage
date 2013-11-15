
using KeyValueStorage.Tools.Cryptography.StringSymmetricAlgorithms;
using NUnit.Framework;

namespace KeyValueStorage.Tools.Tests.Cryptography.StringSymmetricAlgorithms
{
    [TestFixture]
    public class RijndaelManagedStringCryptoSymmetricTests
    {
        [Test]
        public void ShouldEncrypt()
        {
            string strPassword = "SomePwd";
            string document = "someText to encrypt";

            var encrAlgo = new RijndaelManagedStringSymmetricAlgorithm(strPassword);
            var encryptedDocument = encrAlgo.Encrypt(document);

            Assert.AreNotEqual(document, encryptedDocument);
            Assert.AreEqual("W259NpVEpSJZDnp1XGab17NzVKF9qsKcB1szMVpgDoA=", encryptedDocument);
        }

        [Test]
        public void ShouldDecrypt()
        {
            string strPassword = "SomePwd";
            var encryptedDocument = "W259NpVEpSJZDnp1XGab17NzVKF9qsKcB1szMVpgDoA=";

            var encrAlgo = new RijndaelManagedStringSymmetricAlgorithm(strPassword);
            var decryptedDocument = encrAlgo.Decrypt(encryptedDocument);

            Assert.AreEqual(decryptedDocument, "someText to encrypt");
        }

        [Test]
        public void ShouldRoundTripComplex()
        {
            string strPassword = "This is some complex password blah blah blah balh342340909 0902384023948 ";
            string document = @"This
                                is
                                a
                                multiline
                                complex
                                document
                                to
                                encrypt.";

            var encrAlgo = new RijndaelManagedStringSymmetricAlgorithm(strPassword);
            var encryptedDocument = encrAlgo.Encrypt(document);
            var decryptedDocument = encrAlgo.Decrypt(encryptedDocument);

            Assert.AreEqual(decryptedDocument, document);
        }

    }
}
