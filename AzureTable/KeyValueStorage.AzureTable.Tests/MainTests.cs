using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.AzureTable;
using KeyValueStorage.Testing;
using NUnit.Framework;

namespace KeyValueStorage.Oracle.Tests
{
    [TestFixture]
    public class MainTests
    {
        [SetUp]
        public void SetUp()
        {
            //string connString = "data source=ORCL;password=x;persist security info=True;user id=x";

            //var initStore = new OracleStoreProvider(connString);
            //initStore.SetupWorkingTable();

            KVStore.Initialize(new Func<Interfaces.IStoreProvider>(() => new AzureTableStoreProvider()));
        }

        [Test]
        public void CRUDSingleSimpleItemTest()
        {
            ReusableTests.CRUDSingleSimpleItemTest();
        }
    }
}
