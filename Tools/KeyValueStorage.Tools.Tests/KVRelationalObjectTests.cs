using System;
using System.Linq;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Schema;
using KeyValueStorage.Tools.Utility.Relationships;
using NUnit.Framework;
using Moq;
using KeyValueStorage.Tools;
			 
namespace KeyValueStorage.Tools.Tests
{
	[TestFixture]
	public class KVRelationalObjectTests
	{
		private readonly Mock<IRelationalKey> _key;
		private readonly Mock<IStoreSchema> _schema;
		private readonly Mock<IKVStore> _store;

		public KVRelationalObjectTests()
		{
			_key = new Mock<IRelationalKey>();
			_key.Setup(m => m.Value).Returns("A:1");

			_schema = new Mock<IStoreSchema>();
			_store = new Mock<IKVStore>();
			_schema.DefaultValue = DefaultValue.Mock;
		}

		[Test]
		public void ShouldLoadRelatedObjectCorrectly()
		{
			var foreignKeyRelationshipProviderMock = new Mock<IKVForeignKeyRelationshipProvider>();

			foreignKeyRelationshipProviderMock.Setup(m => m.GetKeys(_key.Object))
				.Returns(new IRelationalKey[]
				{
					new RelationalKey("B:1"), 
					new RelationalKey("B:2")
				});

			_schema.Setup(s => s.GetObjectSchema<TestObjA>()
				.GetRelationshipFor<TestObjB>(_store.Object, _key.Object))
				.Returns(new KeyWithRelationship(_key.Object, foreignKeyRelationshipProviderMock.Object));

			
			var o1 = new KVRelationalObject<TestObjA>(_key.Object, _schema.Object, _store.Object);

			var items = o1.Get<TestObjB>().ToList();

			Assert.AreEqual(2, items.Count());

			Assert.IsTrue(items.First().Key.Value == "B:1");
			Assert.IsTrue(items.Last().Key.Value == "B:2");
		}

		[Test]
		public void ShouldCorrectlyAddRelatedObject()
		{
			var keyMock2 = new Mock<IRelationalKey>();
			keyMock2.Setup(m => m.Value).Returns("B:1");

			var foreignKeyRelationshipProvider = new Mock<IKVForeignKeyRelationshipProvider>();

			foreignKeyRelationshipProvider.Setup(m => m.GetKeys(_key.Object))
				.Returns(new IRelationalKey[]
				{
					new RelationalKey("B:1"), 
					new RelationalKey("B:2")
				});

			var generatedRelationshipProvider = new Mock<IKVForeignKeyRelationshipProvider>();

			_schema.Setup(s => s.GetObjectSchema<TestObjA>()
				.GetRelationshipFor<TestObjB>(_store.Object, _key.Object))
				.Returns(new KeyWithRelationship(_key.Object, foreignKeyRelationshipProvider.Object));

			_schema.Setup(m => m.GetObjectSchema<TestObjA>().BuildKeyRelationships(_store.Object, _key.Object)).Returns(new[]
			{
				new KeyWithRelationship(keyMock2.Object, generatedRelationshipProvider.Object)
			});

			var o1 = new KVRelationalObject<TestObjA>(_key.Object, _schema.Object, _store.Object);

			//run
			o1.AddRelationship(new KVRelationalObject<TestObjB>(keyMock2.Object, _schema.Object, _store.Object));


			generatedRelationshipProvider.Verify(m => m.Add(keyMock2.Object, _key.Object));
			foreignKeyRelationshipProvider.Verify(s => s.Add(_key.Object, keyMock2.Object));
		}

		[Test]
		public void ShouldCorrectlyRemoveRelatedObject()
		{
			var keyMock2 = new Mock<IRelationalKey>();
			keyMock2.Setup(m => m.Value).Returns("B:1");

			var foreignKeyRelationshipProvider = new Mock<IKVForeignKeyRelationshipProvider>();

			foreignKeyRelationshipProvider.Setup(m => m.GetKeys(_key.Object))
				.Returns(new IRelationalKey[]
				{
					new RelationalKey("B:1"), 
					new RelationalKey("B:2")
				});

			var generatedRelationshipProvider = new Mock<IKVForeignKeyRelationshipProvider>();

			_schema.Setup(s => s.GetObjectSchema<TestObjA>()
				.GetRelationshipFor<TestObjB>(_store.Object, _key.Object))
				.Returns(new KeyWithRelationship(_key.Object, foreignKeyRelationshipProvider.Object));

			_schema.Setup(m => m.GetObjectSchema<TestObjA>().BuildKeyRelationships(_store.Object, _key.Object)).Returns(new[]
			{
				new KeyWithRelationship(keyMock2.Object, generatedRelationshipProvider.Object)
			});

			var o1 = new KVRelationalObject<TestObjA>(_key.Object, _schema.Object, _store.Object);

			//run
			o1.RemoveRelationship(new KVRelationalObject<TestObjB>(keyMock2.Object, _schema.Object, _store.Object));


			generatedRelationshipProvider.Verify(m => m.Remove(keyMock2.Object, _key.Object));
			foreignKeyRelationshipProvider.Verify(s => s.Remove(_key.Object, keyMock2.Object));
		}
	}
}