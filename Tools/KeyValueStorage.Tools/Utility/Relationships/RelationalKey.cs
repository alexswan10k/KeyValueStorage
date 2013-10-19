using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyValueStorage.Tools.Utility.Relationships
{
    public interface IRelationalKey
    {
        string Value { get; }
    }

    public struct RelationalKey : IRelationalKey
    {
        private readonly string _value;

        public string Value { get { return _value; } }

        internal RelationalKey(string value)
        {
            _value = value;
        }

        public override bool Equals(object obj)
        {
            return string.Equals(this.Value, ((IRelationalKey)obj).Value);
        }

        public bool Equals(RelationalKey other)
        {
            return string.Equals(_value, other._value);
        }

        public override int GetHashCode()
        {
            return (_value != null ? _value.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
