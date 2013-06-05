KeyValueStorage is a project aimed at bridging the gap between various NoSql solutions, 
and strives to provide a common interface that can be shared between all providers.

Features

	Get/Set by key operations
	Sequences
	CAS
	Key expiry
	One common interface for all abstractions

Possible features to support in the future:
		
	Locking (or some other mechanism for performing atomic operations)
	Atomic Collections
	Nuget!
	
Future Vision:
	
	Higher level abstractions to support relational data of strongly typed objects


Write code like:

	KVStore.Initialize(new Func<Interfaces.IStoreProvider>(() => new KeyValueStorage.Redis.RedisStoreProvider(new ServiceStack.Redis.RedisClient())));

	using (var context = KVStore.Factory.Get())
	{
		var bo = new TestBO_A()
		{
			Id = 1,
			Description = "Description"
		};

		context.Set(key, bo);

		var bo2Check = context.Get<TestBO_A>(key);

		var bo3 = new TestBO_A();
		bo3.Id = context.GetNextSequenceValue(key+"S");

	//etc
	}



Note: This project is in early stages, and the interface has not completely been defined, so bear with us.

Currently fully supported:

	Redis

Partly supported:

	AzureTable All except expiry
	Couchbase (CRUD)
	FileSystemText (CRUD) - Provides a mechanism to store objects directly on the file system as json or similar
	Oracle (all except CAS, Expiry, and sequence ops)
	SqlServer (all except CAS, Expiry, and sequence ops)

Planned:

	Cassandra?



Implementing a new database provider technology is as simple as implementing the IStoreProvider interface. This can be found in KeyValueStorage.Interfaces.IStoreProvider

There are currently 4 components:

	The Factory - Allows generation of store' contexts. This should be your entry point for creating your store objects.
	The Provider (IStoreProvider) - Bridges the key(string)/value(string) interface to a database implementation
	The Serializer (ITextSerializer) - Provides serialization of a strongly typed object to string. By default we use ServiceStack.Text due to its fantastic performance.
	The Store(IKVStore) - Bridges the key(string)/value(T) interface to the provider via a serializer or other intermediary logic. A standard bridge implementation is provided so normally speaking it is not necessary to implement this interface.
