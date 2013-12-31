using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;
using ServiceStack.Text;

namespace KeyValueStorage.Utility.Logging
{
    public static class KVLoggerExtensions
    {
        public static void LogSet(this IKVLogger logger, StoreRow item)
        {
            
        }
    }

	public interface IKVLogger
	{
		void LogMessage(string message);
		void LogError(Exception ex);
		void LogStoreCall(string command, string key = null, object value = null, ulong? cas = null, object expiry = null);
	}

	public class NullLogger : IKVLogger
	{
		public void LogMessage(string message)
		{
			
		}

		public void LogError(Exception ex)
		{
			throw ex;
		}

		public void LogStoreCall(string command, string key = null, object value = null, ulong? cas = null, object expiry = null)
		{
			
		}
	}

	public class SimpleKVLogger : IKVLogger
	{
		private readonly bool _logValues;
		private readonly ILogWriter _logWriter;

		public SimpleKVLogger(ILogWriter logWriter, bool logValues = false)
		{
			_logValues = logValues;
			_logWriter = logWriter;
		}

		public void LogMessage(string message)
		{
			_logWriter.Write(message);
		}

		public void LogError(Exception ex)
		{
			_logWriter.Write("Error: \r\n" + ex.Dump());
		}

		public void LogStoreCall(string command, string key = null, object value = null, ulong? cas = null, object expiry = null)
		{
			_logWriter.Write(string.Format("Called {0} \r\nKey {1}\r\nCas {2}\r\nExpiry {3}", command, key, cas, expiry));

			if (_logValues && value != null)
				_logWriter.Write("Value \r\n " + value.Dump());
		}
	}
}
