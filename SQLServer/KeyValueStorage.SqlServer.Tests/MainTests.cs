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
    }
}
