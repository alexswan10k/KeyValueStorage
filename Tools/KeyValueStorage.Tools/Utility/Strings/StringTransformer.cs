using System;

namespace KeyValueStorage.Tools.Utility.Strings
{
    public interface IStringTransformer
    {
        string Transform(string valueToTransform);
    }

    public class NullStringTransformer : IStringTransformer
    {
        public string Transform(string valueToTransform)
        {
            return valueToTransform;
        }
    }

    public class StringTransformer : IStringTransformer
    {
        private readonly Func<string, string> _transformOperation;

        public StringTransformer(Func<string, string> transformOperation)
        {
            _transformOperation = transformOperation;
        }

        public string Transform(string valueToTransform)
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

        public string Transform(string valueToTransform)
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

        public string Transform(string valueToTransform)
        {
            return valueToTransform + _suffix;
        }
    }
}
