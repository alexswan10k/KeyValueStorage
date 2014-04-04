using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Memory;
using NUnit.Framework;

namespace KeyValueStorage.Testing.Memory
{
    [TestFixture]
    class SimpleMemoryStoreProviderTests
    {
        [SetUp]
        public void SetUp()
        {
            KVStore.Initialize(new Func<Interfaces.IStoreProvider>(() => new SimpleMemoryStoreProvider()));
        }

        [Test]
        public void CRUDSingleSimpleItemTest()
        {
            ReusableTests.CRUDSingleSimpleItemTest();
        }

        [Test]
        public void SequenceTest()
        {
            ReusableTests.SequenceTest();
        }

        [Test]
        public void CASTest()
        {
            ReusableTests.CASTest();
        }

        [Test]
        public void KeysStartingWithTest()
        {
            ReusableTests.KeysStartingWithTest();
        }

        [Test]
        public void ValuesStartingWithTest()
        {
            ReusableTests.ValuesStartingWithTest();
        }

        [Test]
        public void CollectionsTest()
        {
            ReusableTests.CollectionsTest();
        }

        [Test]
        public void DoesNotExistTest()
        {
            ReusableTests.DoesNotExistTest();
        }
    }
}
