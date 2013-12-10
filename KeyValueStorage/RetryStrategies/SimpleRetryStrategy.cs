using System;
using System.Threading;
using KeyValueStorage.Exceptions;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.RetryStrategies
{
    public class SimpleRetryStrategy : IRetryStrategy
    {
        private readonly int _maxFails;
        private readonly int _retryWaitPeriodMs;

        public SimpleRetryStrategy(int maxFails, int retryWaitPeriodMs)
        {
            _maxFails = maxFails;
            _retryWaitPeriodMs = retryWaitPeriodMs;
        }

        public T ExecuteFuncWithRetry<T>(Func<T> func)
        {
            return ExecuteFuncWithRetry(func, 0);
        }

        public void ExecuteDelegateWithRetry(Action action)
        {
            ExecuteDelegateWithRetry(action, 0);
        }

        private void ExecuteDelegateWithRetry(Action action, int fails)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (ex is CASException || ex is LockException)
                    throw;

                if (ShouldHandleException(ex))
                {
                    fails++;

                    if (fails < _maxFails)
                    {
                        Thread.Sleep(_retryWaitPeriodMs);
                        ExecuteDelegateWithRetry(action, fails);
                        return;
                    }
                    else
                        throw new Exception("Exceeded max fails", ex);
                }
                throw;
            }
        }

        private T ExecuteFuncWithRetry<T>(Func<T> func, int fails)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                if (ex is CASException || ex is LockException)
                    throw;

                if (ShouldHandleException(ex))
                {
                    fails++;

                    if (fails < _maxFails)
                    {
                        Thread.Sleep(_retryWaitPeriodMs);
                        ExecuteFuncWithRetry(func, fails);
                    }
                    else
                        throw new Exception("Exceeded max fails", ex);
                }
                throw;
            }
        }

        protected virtual bool ShouldHandleException(Exception exception)
        {
            return true;
        }
    }

    //public class RetryStrategyParams
    //{
    //    private int _maxFails;
    //    private TimeSpan _retryWaitPeriod;

    //    public RetryStrategyParams(int maxFails = 4, int retryWaitPeriodMs = 1000)
    //    {
    //        _maxFails = maxFails;
    //        _retryWaitPeriod = TimeSpan.FromMilliseconds(retryWaitPeriodMs);
    //    }

    //    public int MaxFails
    //    {
    //        get { return _maxFails; }
    //    }

    //    public TimeSpan RetryWaitPeriod
    //    {
    //        get { return _retryWaitPeriod; }
    //    }
    //}
}