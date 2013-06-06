using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.AzureTable;
using KeyValueStorage.Testing;
using Microsoft.WindowsAzure.Storage;
using NUnit.Framework;

namespace KeyValueStorage.Oracle.Tests
{
    [TestFixture]
    public class MainTests
    {
        [SetUp]
        public void SetUp()
        {
            var initStore = new AzureTableStoreProvider(CloudStorageAccount.DevelopmentStorageAccount);
            initStore.SetupWorkingTable();

            KVStore.Initialize(new Func<Interfaces.IStoreProvider>(() => new AzureTableStoreProvider(CloudStorageAccount.DevelopmentStorageAccount)));
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
        public void CollectionsTest()
        {
            ReusableTests.CollectionsTest();
        }
    }
}
