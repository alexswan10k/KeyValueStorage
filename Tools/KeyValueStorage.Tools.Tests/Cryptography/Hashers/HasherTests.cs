using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Tools.Cryptography.Hashers;
using NUnit.Framework;

namespace KeyValueStorage.Tools.UnitTests.Cryptography.Hashers
{
    [TestFixture]
    public class HasherTests
    {
        [Test]
        public void NullHasherCanVerifyPassword()
        {
            CanVerifyPassword(new NullHasher());
        }

        [Test]
        public void Md5HasherCanHashNewPassword()
        {
            CanHashNewPassword(new Md5Hasher());
        }

        [Test]
        public void Md5HasherCanVerifyPassword()
        {
            CanVerifyPassword(new Md5Hasher());
        }

        [Test]
        public void Md5HasherWithSaltCanHashNewPassword()
        {
            CanHashNewPassword(new Md5HasherWithSalt());
        }

        [Test]
        public void Md5HasherWithSaltCanVerifyPassword()
        {
            CanVerifyPassword(new Md5HasherWithSalt());
        }

        void CanHashNewPassword(IHasher hasher)
        {
            string password = "some password";
            var computedData = hasher.ComputeHash(password);

            Assert.AreNotEqual(computedData.Hash, password);
            Assert.AreNotEqual(computedData.Salt, password);
        }

        void CanVerifyPassword(IHasher hasher)
        {
            string password = "some password";
            var computedData = hasher.ComputeHash(password);

            Assert.IsTrue(hasher.Verify(password, computedData));
        }
    }
}
