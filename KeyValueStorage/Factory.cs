using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Utility;

namespace KeyValueStorage
{

    public class Factory
    {
        public static Factory Instance { get; set; }

        readonly Func<IKVStore> _storeInitDel;
        private readonly Func<IExportableStore> _exportableStoreDel;

        /// <summary>
        /// Initializes a new instance of the <see cref="Factory"/> class where the provider is set via the init delegate. This is usually used for shortlived providers.
        /// </summary>
        public Factory(Func<IStoreProvider> providerInit, 
            ITextSerializer serializer = null, 
            IRetryStrategy retryStrategy = null, 
            Func<IExportableStore> exportableStoreDel = null, 
            bool suppressInitialize = false)
        {
	        _storeInitDel = () => new KVStore(providerInit(), serializer, retryStrategy);
	        _exportableStoreDel = exportableStoreDel ?? (() => new KVStoreProviderExportableStore(providerInit()));

	        if (!suppressInitialize)
                Initialize();
        }

        public Factory(Func<IKVStore> storeInit, 
            Func<IExportableStore> exportableStoreDel = null,
            bool suppressInitialize = false)
        {
            _storeInitDel = storeInit;
			_exportableStoreDel = exportableStoreDel ?? (() => new KVStoreProviderExportableStore(storeInit().StoreProvider));

            if (!suppressInitialize)
                Initialize();
        }

        private void Initialize()
        {
            Get().StoreProvider.Initialize();
        }

        public IKVStore Get()
        {
            return _storeInitDel();
        }

        public IExportableStore GetExportableStore()
        {
            if (_exportableStoreDel != null)
                return _exportableStoreDel();
            return null;
        }
    }
}
