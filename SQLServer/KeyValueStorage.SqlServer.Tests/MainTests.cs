using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Testing;
using NUnit.Framework;

namespace KeyValueStorage.SqlServer.Tests
{
    [TestFixture]
    public class MainTests
    {
        [SetUp]
        public void SetUp()
        {
            string connString = @"data source=localhost\SQLEXPRESS;initial catalog=Test;User Id=Test;Password=test";
            KVStore.Initialize(new Func<Interfaces.IStoreProvider>(() => new SqlServerStoreProvider(connString)));
        }

        [Test]
        public void CRUDSingleSimpleItemTest()
        {
            ReusableTests.CRUDSingleSimpleItemTest();
        }
    }
}
