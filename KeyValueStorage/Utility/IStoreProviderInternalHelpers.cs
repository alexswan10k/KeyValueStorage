using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using KeyValueStorage.Exceptions;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Utility
{
    public static class IStoreProviderInternalHelpers
    {
        public static ulong GetNextSequenceValueViaCASWithRetries(IStoreProvider provider, string key, int increment, int tryCount)
        {
            return getNextSequenceValueViaCASRet(provider, key, increment, tryCount);
        }

        private static ulong getNextSequenceValueViaCASRet(IStoreProvider provider, string key, int increment, int tryCount)
        {
            try
            {
                ulong cas;
                var obj = provider.Get(key, out cas);
                ulong seqVal;

                if (!UInt64.TryParse(obj, out seqVal))
                {
                    seqVal = 0;
                }
                seqVal = seqVal + (ulong)increment;
                provider.Set(key, seqVal.ToString(), cas);
                return seqVal;
            }
            catch (CASException casEx)
            {
                if (tryCount >= 10)
                    throw new Exception("Could not get sequence value", casEx);

                Thread.Sleep(20);
                //retry
                return getNextSequenceValueViaCASRet(provider, key, increment, tryCount++);
            }
            return 0;
        }

        public static ulong GetNextSequenceValueViaCAS(IStoreProvider fsTextStoreProvider, string key, int increment)
        {
            ulong cas;
            var obj = fsTextStoreProvider.Get(key, out cas);
            ulong seqVal;

            if (!UInt64.TryParse(obj, out seqVal))
            {
                seqVal = 0;
            }
            seqVal = seqVal + (ulong) increment;
            fsTextStoreProvider.Set(key, seqVal.ToString(), cas);
            return seqVal;
        }
    }
}
