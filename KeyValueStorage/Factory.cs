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

        Func<IStoreProvider> providerInitDel;
        Func<IKVStore> storeInitDel;

        /// <summary>
        /// Initializes a new instance of the <see cref="Factory"/> class where the provider is set via the init delegate. This is usually used for shortlived providers.
        /// </summary>
        /// <param name="providerInit">The provider init.</param>
        public Factory(Func<IStoreProvider> providerInit)
        {
            providerInitDel = providerInit;
            storeInitDel = () => new KVStore(providerInitDel());
        }

        public Factory(Func<IStoreProvider> providerInit, ITextSerializer serializer)
        {
            providerInitDel = providerInit;
            storeInitDel = () => new KVStore(providerInitDel());
        }

        public Factory(Func<IKVStore> storeInit)
        {
            storeInitDel = storeInit;
        }

        public IKVStore Get()
        {
            return storeInitDel();
        }
    }
}
