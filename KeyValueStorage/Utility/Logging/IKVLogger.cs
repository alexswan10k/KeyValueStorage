using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Utility.Logging
{
	public interface IKVLogger
	{
		void LogMessage(string message);
		void LogSet(StoreBackupRow item);
	}

	public class NullLogger : IKVLogger
	{
		public void LogMessage(string message)
		{
			
		}

		public void LogSet(StoreBackupRow item)
		{
			
		}
	}
}
