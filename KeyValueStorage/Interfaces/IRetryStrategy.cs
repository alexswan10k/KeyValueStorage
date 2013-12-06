using System;

namespace KeyValueStorage.Interfaces
{
    public interface IRetryStrategy
    {
        T ExecuteFuncWithRetry<T>(Func<T> func);
        void ExecuteDelegateWithRetry(Action action);
    }
}