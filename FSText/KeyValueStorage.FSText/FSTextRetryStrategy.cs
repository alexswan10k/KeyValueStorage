using System;
using System.IO;
using KeyValueStorage.RetryStrategies;

namespace KeyValueStorage.FSText
{
    public class FSTextRetryStrategy : SimpleRetryStrategy
    {
        public FSTextRetryStrategy(int maxFails, int retryWaitPeriodMs) : 
            base(maxFails, retryWaitPeriodMs)
        {
        }

        protected override bool ShouldHandleException(Exception exception)
        {
            if (exception is IOException)
                return true;
            return false;
        }
    }
}