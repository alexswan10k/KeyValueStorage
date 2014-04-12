using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using KeyValueStorage.Testing.TestObjects;
using NUnit.Framework;

namespace KeyValueStorage.Testing
{
    [TestFixture]
    public class TextSerializerTests
    {
        private readonly JavaScriptTextSerializer _ser = new JavaScriptTextSerializer();

        [Test]
        public void ShouldSerializeNullAsEmptyString()
        {
            var res = _ser.Serialize<string>(null);
            Assert.AreEqual("null", res);
        }

        [Test]
        public void ShouldDeserializeEmptyString()
        {
            var res = _ser.Deserialize<TestBO_A>(string.Empty);
            Assert.IsNull(res);
        }

        [Test]
        public void ShouldDeserializeNull()
        {
            var res = _ser.Deserialize<TestBO_A>(null);
            Assert.IsNull(res);
        }

        [Test]
        public void ShouldDeserializeStringRepresentedNull()
        {
            var res = _ser.Deserialize<TestBO_A>("null");
            Assert.IsNull(res);
        }

        [Test]
        public void ShouldSerializeObject()
        {
            var obj = new TestBO_A()
                      {
                          Id = 1,
                          Description = "someDescr"
                      };

            var res = _ser.Serialize(obj);
            Assert.AreEqual("{\"Id\":1,\"Description\":\"someDescr\"}", res);
        }

        [Test]
        public void ShouldDeserializeObject()
        {
            var val = "{\"Id\":1,\"Description\":\"someDescr\"}";

            var res = _ser.Deserialize<TestBO_A>(val);

            Assert.IsNotNull(res);
            Assert.AreEqual(1, res.Id);
            Assert.AreEqual("someDescr", res.Description);
        }
    }
}
