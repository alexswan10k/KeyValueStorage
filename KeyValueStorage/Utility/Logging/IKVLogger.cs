using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;

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
		void LogSet(StoreRow item);
	}

	public class NullLogger : IKVLogger
	{
		public void LogMessage(string message)
		{
			
		}

		public void LogSet(StoreRow item)
		{
			
		}
	}
}
