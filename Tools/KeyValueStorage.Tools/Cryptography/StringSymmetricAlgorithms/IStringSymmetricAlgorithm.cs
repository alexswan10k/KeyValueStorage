namespace KeyValueStorage.Tools.Cryptography.StringSymmetricAlgorithms
{
    public interface IStringSymmetricAlgorithm
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}