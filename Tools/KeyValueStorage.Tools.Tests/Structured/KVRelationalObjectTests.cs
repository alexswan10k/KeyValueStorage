using System.Linq;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Structured;
using KeyValueStorage.Tools.Structured.Schema;
using KeyValueStorage.Tools.Utility.Relationships;
using Moq;
using NUnit.Framework;

namespace KeyValueStorage.Tools.UnitTests.Structured
{
	[TestFixture]
	public class KVRelationalObjectTests
	{
        private readonly Key _key = "A:1";
		private readonly Mock<IStoreSchema> _schema;
		private readonly Mock<IKVStore> _store;

		public KVRelationalObjectTests()
		{
			_schema = new Mock<IStoreSchema>();
			_store = new Mock<IKVStore>();
			_schema.DefaultValue = DefaultValue.Mock;
		}

		[Test]
		public void ShouldLoadRelatedObjectCorrectly()
		{
			var foreignKeyRelationshipProviderMock = new Mock<IKVForeignKeyRelationshipProvider>();

			foreignKeyRelationshipProviderMock.Setup(m => m.GetKeys(_key))
				.Returns(new Key[]
				{
					"B:1", 
					"B:2"
				});

			_schema.Setup(s => s.GetObjectSchema<TestObjA>()
				.GetRelationshipFor<TestObjB>(_store.Object, _key))
				.Returns(new KeyWithRelationship(_key, foreignKeyRelationshipProviderMock.Object));

			
			var o1 = new KVRelationalObject<TestObjA>(_key, _schema.Object, _store.Object);

			var items = o1.Get<TestObjB>().ToList();

			Assert.AreEqual(2, items.Count());

			Assert.IsTrue(items.First().Key.Value == "B:1");
			Assert.IsTrue(items.Last().Key.Value == "B:2");
		}

		[Test]
		public void ShouldCorrectlyAddRelatedObject()
		{

			Key keyMock2 = "B:1";

			var foreignKeyRelationshipProvider = new Mock<IKVForeignKeyRelationshipProvider>();

			foreignKeyRelationshipProvider.Setup(m => m.GetKeys(_key))
				.Returns(new Key[]
				{
					"B:1", 
					"B:2"
				});

			var generatedRelationshipProvider = new Mock<IKVForeignKeyRelationshipProvider>();

			_schema.Setup(s => s.GetObjectSchema<TestObjA>()
				.GetRelationshipFor<TestObjB>(_store.Object, _key))
				.Returns(new KeyWithRelationship(_key, foreignKeyRelationshipProvider.Object));

			_schema.Setup(m => m.GetObjectSchema<TestObjA>().BuildKeyRelationships(_store.Object, _key)).Returns(new[]
			{
				new KeyWithRelationship(keyMock2, generatedRelationshipProvider.Object)
			});

			var o1 = new KVRelationalObject<TestObjA>(_key, _schema.Object, _store.Object);

			//run
			o1.AddRelationship(new KVRelationalObject<TestObjB>(keyMock2, _schema.Object, _store.Object));


			generatedRelationshipProvider.Verify(m => m.Add(keyMock2, _key));
			foreignKeyRelationshipProvider.Verify(s => s.Add(_key, keyMock2));
		}

		[Test]
		public void ShouldCorrectlyRemoveRelatedObject()
		{
			var keyMock2 = "B:1";

			var foreignKeyRelationshipProvider = new Mock<IKVForeignKeyRelationshipProvider>();

			foreignKeyRelationshipProvider.Setup(m => m.GetKeys(_key))
				.Returns(new Key[]
				{
					"B:1", 
					"B:2"
				});

			var generatedRelationshipProvider = new Mock<IKVForeignKeyRelationshipProvider>();

			_schema.Setup(s => s.GetObjectSchema<TestObjA>()
				.GetRelationshipFor<TestObjB>(_store.Object, _key))
				.Returns(new KeyWithRelationship(_key, foreignKeyRelationshipProvider.Object));

			_schema.Setup(m => m.GetObjectSchema<TestObjA>().BuildKeyRelationships(_store.Object, _key)).Returns(new[]
			{
				new KeyWithRelationship(keyMock2, generatedRelationshipProvider.Object)
			});

			var o1 = new KVRelationalObject<TestObjA>(_key, _schema.Object, _store.Object);

			//run
			o1.RemoveRelationship(new KVRelationalObject<TestObjB>(keyMock2, _schema.Object, _store.Object));


			generatedRelationshipProvider.Verify(m => m.Remove(keyMock2, _key));
			foreignKeyRelationshipProvider.Verify(s => s.Remove(_key, keyMock2));
		}
	}
}