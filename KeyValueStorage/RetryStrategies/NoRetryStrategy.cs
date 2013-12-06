using System;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.RetryStrategies
{
    public class NoRetryStrategy : IRetryStrategy
    {
        public T ExecuteFuncWithRetry<T>(Func<T> func)
        {
            return func();
        }

        public void ExecuteDelegateWithRetry(Action action)
        {
            action();
        }
    }
}