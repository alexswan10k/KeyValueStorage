KeyValueStorage is a project aimed at making storage of data simple, bridging the gap between various NoSql solutions, 
and strives to provide a common interface that can be shared between all providers. Tools are also available to deal with more complex scenarios such as relational data and encryption.

(http://nuget.org/List/Packages/KeyValueStorage.FSText)

Features

	Get/Set complex types by key operations
	Sequences
	CAS
	Key expiry
	One common interface for all abstractions
	Tools for Membership/Role/Profile substitutes
	Tools for encryption and decryption of keys on insertion
	Tools for storing relational data (currently somewhat experimental)
	Locking
	Written using TDD and loosely coupled code. Plenty of extension points to customize behaviour.
	Available on NuGet!
	

Possible features to support in the future:
		
	Locking (or some other mechanism for performing atomic operations)
	Atomic Collections
	


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

	Couchbase
	Redis
	SqlServer
	FileSystem

Partly supported:

	AzureTable (expiry early proto)
	Oracle (all except CAS, Expiry, and sequence ops)
	Cassandra (CRUD support)



Implementing a new database provider technology is as simple as implementing the IStoreProvider interface. This can be found in KeyValueStorage.Interfaces.IStoreProvider

There are currently 4 components:

	The Factory - Allows generation of store' contexts. This should be your entry point for creating your store objects.
	The Provider (IStoreProvider) - Bridges the key(string)/value(string) interface to a database implementation
	The Serializer (ITextSerializer) - Provides serialization of a strongly typed object to string. By default we use ServiceStack.Text due to its fantastic performance.
	The Store(IKVStore) - Bridges the key(string)/value(T) interface to the provider via a serializer or other intermediary logic. A standard bridge implementation is provided so normally speaking it is not necessary to implement this interface.
	

