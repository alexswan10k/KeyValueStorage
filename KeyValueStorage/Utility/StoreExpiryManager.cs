using KeyValueStorage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Exceptions;

namespace KeyValueStorage.Utility
{
    public class StoreExpiryManager
    {
        public IExpiredKeyCleaner KeyCleaner {get;protected set;}

        public StoreExpiryManager(IExpiredKeyCleaner keyCleaner)
        {
            KeyCleaner = keyCleaner; 
        }

        public IDisposable BeginTaskCleanupKeys(int period_ms)
        {
            tickTimer = new System.Threading.Timer(new System.Threading.TimerCallback(o =>
            {

                if (cleanupTask == null || cleanupTask.IsCompleted || cleanupTask.IsFaulted)
                    Task.Factory.StartNew(new Action(KeyCleaner.CleanupKeys));
            }), null, 0, period_ms);

            return tickTimer;
        }

        System.Threading.Timer tickTimer;
        Task cleanupTask = null;
    }
}
