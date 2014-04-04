using System;

namespace KeyValueStorage.Tools.Utility.Strings
{
    public interface IStringTransformer
    {
        Key Transform(Key valueToTransform);
    }

    public class NullStringTransformer : IStringTransformer
    {
        public Key Transform(Key valueToTransform)
        {
            return valueToTransform;
        }
    }

    public class StringTransformer : IStringTransformer
    {
		private readonly Func<Key, Key> _transformOperation;

		public StringTransformer(Func<Key, Key> transformOperation)
        {
            _transformOperation = transformOperation;
        }

        public Key Transform(Key valueToTransform)
        {
            return _transformOperation(valueToTransform);
        }
    }

    public class PrefixTransformer : IStringTransformer
    {
        private readonly string _prefix;

        public PrefixTransformer(string prefix)
        {
            _prefix = prefix;
        }

        public Key Transform(Key valueToTransform)
        {
            return _prefix + valueToTransform;
        }
    }

    public class SuffixTransformer : IStringTransformer
    {
        private readonly string _suffix;

        public SuffixTransformer(string suffix)
        {
            _suffix = suffix;
        }

        public Key Transform(Key valueToTransform)
        {
            return valueToTransform + _suffix;
        }
    }
}
