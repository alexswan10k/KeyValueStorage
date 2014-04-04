using System;
using System.Linq;
using NUnit.Framework;
using Moq;
using KeyValueStorage.Tools.Utility.Strings;
			 
namespace KeyValueStorage.Tools.Tests.Utility.Strings
{
	[TestFixture]
	public class StringTransformerTests
	{
		private string _transformString = "This is a string to transform";

		[Test]
		public void NullStringTransformerTest()
		{
			var st = new NullStringTransformer();
			Assert.AreEqual(_transformString, st.Transform(_transformString));
		}

		[Test]
		public void StringTransformer()
		{
            Func<Key, Key> transformOperation = s => s.ToString().Replace("is", "isnt");
			var st = new StringTransformer(transformOperation);
			Assert.AreEqual(transformOperation(_transformString), st.Transform(_transformString));
		}


		[Test]
		public void PrefixTransformer()
		{
			string prefix = "prefix ";
			var st = new PrefixTransformer(prefix);
			Assert.IsTrue(st.Transform(_transformString).ToString().StartsWith(prefix));
		}

		[Test]
		public void SuffixTransformer()
		{
			string suffix = " suffix";
			var st = new SuffixTransformer(suffix);
            Assert.IsTrue(st.Transform(_transformString).ToString().EndsWith(suffix));
		}
	}
}