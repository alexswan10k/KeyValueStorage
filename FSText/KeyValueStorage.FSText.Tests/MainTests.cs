﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Testing;
using NUnit.Framework;
using KeyValueStorage;

namespace KeyValueStorage.FSText.Tests
{
    [TestFixture]
    public class MainTests
    {
        [SetUp]
        public void SetUp()
        {
            KVStore.Initialize(new Func<Interfaces.IStoreProvider>(() => new KeyValueStorage.FSText.FSTextStoreProvider(@"C:\FSTextStore")));
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

    [TestFixture]
    class RelationalTests
    {
        [SetUp]
        public void SetUp()
        {
            KVStore.Initialize(new Func<Interfaces.IStoreProvider>(() => new KeyValueStorage.FSText.FSTextStoreProvider(@"C:\FSTextStore")));
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
