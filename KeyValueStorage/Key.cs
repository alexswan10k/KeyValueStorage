using System;

namespace KeyValueStorage
{
    public interface IKey
    {
        string Value { get; }
    }

    public struct Key : IKey
    {
        private readonly string _value;

        public string Value { get { return _value; } }

        internal Key(string value)
        {
            _value = value;
        }

        public override bool Equals(object obj)
        {
            return string.Equals(this.Value, ((IKey)obj).Value);
        }

        public bool Equals(Key other)
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

		public  static  implicit operator string(Key key)
		{
			return key.ToString();
		}

		public static implicit  operator  Key(string key)
		{
			return new Key(key);
		}

		public static implicit operator Key(int key)
		{
			return new Key(key.ToString());
		}

		public static implicit operator Key(uint key)
		{
			return new Key(key.ToString());
		}

		public static implicit operator Key(long key)
		{
			return new Key(key.ToString());
		}

		public static implicit operator Key(ulong key)
		{
			return new Key(key.ToString());
		}

		public static implicit operator Key(short key)
		{
			return new Key(key.ToString());
		}

		public static implicit operator Key(ushort key)
		{
			return new Key(key.ToString());
		}

		public static implicit operator Key(double key)
		{
			return new Key(key.ToString());
		}

		public static implicit operator Key(decimal key)
		{
			return new Key(key.ToString());
		}

		public static implicit operator Key(Guid key)
		{
			return new Key(key.ToString());
		}
    }
}
