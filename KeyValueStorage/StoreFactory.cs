using System;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage
{
    public class StoreFactory<TStore, TStoreProvider> 
        where TStore : ISimpleKVStore<TStoreProvider>
        where TStoreProvider : ISimpleStoreProvider
    {
        readonly Func<TStore> _storeInitDel;

        public StoreFactory(Func<TStore> storeInitDel)
        {
            _storeInitDel = storeInitDel;
        }

        private void _Initialize()
        {
            Get().StoreProvider.Initialize();
        }

        public TStore Get()
        {
            return _storeInitDel();
        }
    }
}