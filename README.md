KeyValueStorage is a project aimed at bridging the gap between various NoSql solutions, 
and strives to provide a common interface that can be shared between all providers.

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

		var boCheck = context.Get<TestBO_A>(key);

		Assert.AreEqual(bo.Id, boCheck.Id);
		Assert.AreEqual(bo.Description, boCheck.Description);
	}



Note: This project is in early stages, and the interface has not completely been defined, so bear with us.

Currently fully supported:
-nothing yet, help us out and fork!


Currently partly supported:
Redis (CRUD)
Couchbase (CRUD)
FileSystemText (CRUD)
Oracle (all except sequence ops)
SqlServer (all except sequence ops)

Planned:
AzureTable
Cassandra?

Implementing a new database provider technology is as simple as implementing the IStoreProvider interface. This can be found in KeyValueStorage.Interfaces.IStoreProvider

There are currently 4 components:

The Factory - Allows generation of store' contexts. This should be your entry point for creating your store objects.
The Provider (IStoreProvider) - Bridges the key(string)/value(string) interface to a database implementation
The Serializer (ITextSerializer) - Provides serialization of a strongly typed object to string. By default we use ServiceStack.Text due to its fantastic performance.
The Store(IKVStore) - Bridges the key(string)/value(T) interface to the provider via a serializer or other intermediary logic. A standard bridge implementation is provided so normally speaking it is not necessary to implement this interface.
