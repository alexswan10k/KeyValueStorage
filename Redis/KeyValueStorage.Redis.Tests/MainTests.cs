﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Testing;
using NUnit.Framework;
using KeyValueStorage;

namespace KeyValueStorage.Redis.Tests
{
    [TestFixture]
    public class MainTests
    {
        [SetUp]
        public void SetUp()
        {
            KVStore.Initialize(new Func<Interfaces.IStoreProvider>(() => new KeyValueStorage.Redis.RedisStoreProvider(new ServiceStack.Redis.RedisClient())));
        }

        [Test]
        public void CRUDSingleSimpleItemTest()
        {
            ReusableTests.CRUDSingleSimpleItemTest();
        }
    }
}