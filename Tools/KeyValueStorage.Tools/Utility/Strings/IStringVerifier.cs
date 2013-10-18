namespace KeyValueStorage.Tools.Utility.Strings
{
	public interface IStringVerifier
	{
		bool Verify(string text);
	}


	public class NullStringVerifier : IStringVerifier
	{
		public bool Verify(string text)
		{
			return true;
		}
	}
}