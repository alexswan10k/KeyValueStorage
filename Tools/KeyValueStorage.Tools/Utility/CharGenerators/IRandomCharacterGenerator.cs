using System;
using System.Text;

namespace KeyValueStorage.Tools.Utility.CharGenerators
{
	public interface IRandomCharacterGenerator
	{
		string Generate();
	}

	public class SimpleRandomCharacterGenerator : IRandomCharacterGenerator
	{
		private readonly int _length;
		private readonly Random _random = new Random();

		public SimpleRandomCharacterGenerator(int length = 8)
		{
			_length = length;
		}

		public string Generate()
		{
			var sb = new StringBuilder(_length);

			char ch;
			for (int i = 0; i < _length; i++)
			{
				ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * _random.NextDouble() + 65)));
				sb.Append(ch);
			}
			return sb.ToString();
		}
	}
}