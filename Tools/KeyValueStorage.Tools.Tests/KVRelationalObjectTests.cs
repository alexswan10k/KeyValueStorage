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
		private Mock<IRelationalKey> _key;
		private Mock<IStoreSchema> _schema;
		private Mock<IKVStore> _store;

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

			

			//_schema.Setup(s => s.GetObjectSchema<TestObjA>()
			//	.GetRelationshipFor<TestObjA>()(_store.Object, newkey)

			//for both providers
			foreignKeyRelationshipProviderMock.Setup(m => m.Add(It.IsAny<IRelationalKey>(), It.IsAny<IRelationalKey>()));

			var o1 = new KVRelationalObject<TestObjA>(_key.Object, _schema.Object, _store.Object);

			Mock<IRelationalKey> keyMock2 = new Mock<IRelationalKey>();
			keyMock2.Setup(m => m.Value).Returns("B:1");

			//new KVForeignKeyStoreRelationshipProvider(_store.Object)

			Mock<IKVForeignKeyRelationshipProvider> mock = new Mock<IKVForeignKeyRelationshipProvider>();
			
			_schema.Setup(m => m.GetObjectSchema<TestObjA>().BuildKeyRelationships(_store.Object, _key.Object)).Returns(new KeyWithRelationship[]
			{
				new KeyWithRelationship(keyMock2.Object, mock.Object)
			});

			o1.AddRelationship(new KVRelationalObject<TestObjB>(keyMock2.Object, _schema.Object, _store.Object));

			mock.Verify(m => m.Add(keyMock2.Object, _key.Object));
			//verify local one
		}

		[Test]
		public void ShouldCorrectlyRemoveRelatedObject()
		{
			
		}
	}
}