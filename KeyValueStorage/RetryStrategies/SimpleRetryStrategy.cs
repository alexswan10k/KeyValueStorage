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
			if (exception is CASException || exception is LockException)
				return false;

			return true;
		}
	}

	public class SimpleLockRetryStrategy : SimpleRetryStrategy
	{
		public SimpleLockRetryStrategy(int maxFails, int retryWaitPeriodMs) 
			: base(maxFails, retryWaitPeriodMs)
		{
		}

		protected override bool ShouldHandleException(Exception exception)
		{
			if (exception is LockException)
				return true;
			return false;
		}
	}
}