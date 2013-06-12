using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage
{

    public class Factory
    {
        public static Factory Instance { get; set; }

        Func<IKVStore> storeInitDel;

        /// <summary>
        /// Initializes a new instance of the <see cref="Factory"/> class where the provider is set via the init delegate. This is usually used for shortlived providers.
        /// </summary>
        /// <param name="providerInit">The provider init.</param>
        public Factory(Func<IStoreProvider> providerInit, bool suppressInitialize = false)
        {
            storeInitDel = () => new KVStore(providerInit());

            if (!suppressInitialize)
                Initialize();
        }

        public Factory(Func<IStoreProvider> providerInit, ITextSerializer serializer, bool suppressInitialize = false)
        {
            storeInitDel = () => new KVStore(providerInit());

            if (!suppressInitialize)
                Initialize();
        }

        public Factory(Func<IKVStore> storeInit, bool suppressInitialize = false)
        {
            storeInitDel = storeInit;

            if (!suppressInitialize)
                Initialize();
        }

        private void Initialize()
        {
            Get().StoreProvider.Initialize();
        }

        public IKVStore Get()
        {
            return storeInitDel();
        }
    }
}
