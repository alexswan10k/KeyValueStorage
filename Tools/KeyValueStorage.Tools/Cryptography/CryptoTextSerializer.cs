using System.Collections.Generic;
using System.Linq;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Cryptography.StringSymmetricAlgorithms;

namespace KeyValueStorage.Tools.Cryptography
{
    public class CryptoTextSerializer : ITextSerializer
    {
        private readonly ITextSerializer _baseSerializer;
        private readonly IStringSymmetricAlgorithm _symmetricAlgorithm;

        public CryptoTextSerializer(ITextSerializer baseSerializer, IStringSymmetricAlgorithm symmetricAlgorithm)
        {
            _baseSerializer = baseSerializer;
            _symmetricAlgorithm = symmetricAlgorithm;
        }

        public string Serialize<T>(T item)
        {
            return _symmetricAlgorithm.Encrypt(_baseSerializer.Serialize(item));
        }

        public T Deserialize<T>(string itemSerialized)
        {
            return _baseSerializer.Deserialize<T>(_symmetricAlgorithm.Decrypt(itemSerialized));
        }
    }
}
