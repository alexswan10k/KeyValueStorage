using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Memory;
using NUnit.Framework;

namespace KeyValueStorage.Testing.Memory
{
    [TestFixture]
    class SimpleMemoryStoreProviderRelationalTests
    {
        [SetUp]
        public void SetUp()
        {
            KVStore.Initialize(new Func<Interfaces.IStoreProvider>(() => new SimpleMemoryStoreProvider()));
        }

        [Test]
        public void ShouldAddToSingleRelationship()
        {
            RelationalReusableTests.ShouldHandleSingleRelationship();
        }

        [Test]
        public void ShouldBuildRelatedObjectFromFetchedKey()
        {
            RelationalReusableTests.ShouldBuildRelatedObjectFromFetchedKey();
        }

        [Test]
        public void ShouldHandleMultipleRelationshipsIndependently()
        {
            RelationalReusableTests.ShouldHandleMultipleRelationshipsIndependently();
        }
    }
}
