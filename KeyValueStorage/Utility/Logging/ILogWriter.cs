using System.Data;
using System.IO;
using System.Text;
using KeyValueStorage.Interfaces.Utility;

namespace KeyValueStorage.Utility.Logging
{
	public interface ILogWriter
	{
		void Write(string message);
	}

	public class StringBuilderLogWriter : ILogWriter
	{
		private readonly StringBuilder _builder;

		public StringBuilderLogWriter(StringBuilder builder)
		{
			_builder = builder;
		}

		public void Write(string message)
		{
			_builder.Append(message);
			_builder.AppendLine();
		}
	}

	public class StreamLogWriter : ILogWriter
	{
		private readonly StreamWriter _writer;

		public StreamLogWriter(StreamWriter writer)
		{
			_writer = writer;
		}

		public void Write(string message)
		{
			_writer.Write(message);
			_writer.WriteLine();
		}
	}

	public class FileLogWriter : ILogWriter
	{
		private readonly FileInfo _file;

		public FileLogWriter(FileInfo file)
		{
			_file = file;
		}

		public void Write(string message)
		{
			using(var streamWriter = _file.AppendText())
			{
				streamWriter.Write(message);
				streamWriter.WriteLine();
			}
		}
	}
}