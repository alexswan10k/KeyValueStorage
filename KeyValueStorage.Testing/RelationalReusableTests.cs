using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;
using KeyValueStorage.Testing.TestObjects;
using NUnit.Framework;

namespace KeyValueStorage.Testing
{
    public static class RelationalReusableTests
    {
        public static void ShouldHandleSingleRelationship()
        {
            using (var context = KVStore.Factory.Get())
            {
                Guid boAKey = Guid.NewGuid();

                try
                {
                    Guid KeyRel1 = Guid.NewGuid();
                    Guid KeyRel2 = Guid.NewGuid();
                    Guid KeyRel3 = Guid.NewGuid();

                    context.AddRelationship<TestBO_A, TestBO_B>(boAKey, KeyRel1);
                    context.AddRelationship<TestBO_A, TestBO_B>(boAKey, KeyRel2);
                    context.AddRelationship<TestBO_A, TestBO_B>(boAKey, KeyRel3);

                    var relatedKeys = context.GetRelatedKeysFor<TestBO_A, TestBO_B>(boAKey).ToList();

                    Assert.AreEqual(3, relatedKeys.Count());

                    Assert.IsTrue(relatedKeys.Contains(KeyRel1.ToString()));
                    Assert.IsTrue(relatedKeys.Contains(KeyRel2.ToString()));
                    Assert.IsTrue(relatedKeys.Contains(KeyRel3.ToString()));

                    var relatedKeysAssoc1 = context.GetRelatedKeysFor<TestBO_B, TestBO_A>(KeyRel1);

                    Assert.AreEqual(1, relatedKeysAssoc1.Count());
                    Assert.IsTrue(relatedKeysAssoc1.Contains(boAKey.ToString()));
                }
                finally
                {
                    context.ClearRelationships<TestBO_A, TestBO_B>(boAKey);
                }
            }
        }

        public static void ShouldBuildRelatedObjectFromFetchedKey()
        {
            using (var context = KVStore.Factory.Get())
            {
                Guid boAKey = Guid.NewGuid();
                Guid KeyRel1 = Guid.NewGuid();
                try
                {
                    var testBoB = new TestBO_B() {SomeInfo = "This is some info"};
                    context.Set(KeyRel1, testBoB);
                    context.AddRelationship<TestBO_A, TestBO_B>(boAKey, KeyRel1);

                    var relatedObjs = context.GetRelatedFor<TestBO_A, TestBO_B>(boAKey);

                    Assert.AreEqual(testBoB.SomeInfo, relatedObjs.First().Value.SomeInfo);
                }
                finally
                {
                    context.ClearRelationships<TestBO_A, TestBO_B>(boAKey);
                    context.Delete(KeyRel1);
                }
            }
        }

        public static void ShouldHandleMultipleRelationshipsIndependently()
        {
            using (var context = KVStore.Factory.Get())
            {
                Guid boAKey = Guid.NewGuid();
                Guid KeyRel1 = Guid.NewGuid();
                Guid KeyRel2 = Guid.NewGuid();
                try
                {
                    context.AddRelationship<TestBO_A, TestBO_B>(boAKey, KeyRel1);
                    context.AddRelationship<TestBO_A, TestBO_C>(boAKey, KeyRel2);

                    var relatedKeysAB = context.GetRelatedKeysFor<TestBO_A, TestBO_B>(boAKey).ToList();

                    Assert.AreEqual(1, relatedKeysAB.Count);
                    Assert.Contains(KeyRel1.ToString(), relatedKeysAB);

                    var relatedKeysAC = context.GetRelatedKeysFor<TestBO_A, TestBO_C>(boAKey).ToList();

                    Assert.AreEqual(1, relatedKeysAC.Count);
                    Assert.Contains(KeyRel2.ToString(), relatedKeysAC);

                    var relatedKeysBA = context.GetRelatedKeysFor<TestBO_B, TestBO_A>(KeyRel1).ToList();
                    var relatedKeysCA = context.GetRelatedKeysFor<TestBO_C, TestBO_A>(KeyRel2).ToList();

                    Assert.Contains(boAKey.ToString(), relatedKeysBA);
                    Assert.Contains(boAKey.ToString(), relatedKeysCA);    
                }
                finally
                {
                    context.ClearRelationships<TestBO_A, TestBO_B>(boAKey);
                    context.ClearRelationships<TestBO_A, TestBO_C>(boAKey);
                    context.Delete(KeyRel1);
                    context.Delete(KeyRel2);
                }
            }
        }
    }
}
