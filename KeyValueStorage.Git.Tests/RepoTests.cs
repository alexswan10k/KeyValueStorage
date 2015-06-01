using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace KeyValueStorage.Git.Tests
{
    [TestFixture]
    class RepoTests
    {
        private Repo _repo;

        private void DeleteFilesRec(DirectoryInfo di)
        {
            foreach (var file in di.EnumerateFiles())
            {
                file.Attributes = FileAttributes.Normal;
                file.Delete();
            }

            foreach (var dir in di.EnumerateDirectories())
            {
                DeleteFilesRec(dir);
            }
        }

        [SetUp]
        public void SetUp()
        {
            var di = new System.IO.DirectoryInfo(@"GitStore\G1");
            if (di.Exists){
                DeleteFilesRec(di);
                di.Delete(true);
            }
                
            _repo = new Repo(@"GitStore\G1");
        }

        [TearDown]
        public void TearDown()
        {
            _repo.Dispose();
        }

        [Test]
        public void SaveAndGet()
        {
            _repo.Save("A", @"
                {
                    timmy:4,
                    bobby:3
                }
            ");
            _repo.Save("A", @"
                {
                    timmy:5,
                    bobby:3
                }
            ");
            _repo.Save("A", @"
                {
                    timmy:4,
                    bobby:2
                }
            ");
            _repo.Save("A", @"
                {
                    timmy:4,
                    bobby:1
                }
            ");
            string aValFinal = @"
                {
                    timmy:4,
                    bobby:1,
                    jimmy:177
                }
            ";
            _repo.Save("A", aValFinal);
            string bValFinal = @"
                {
                    timmy:1,
                    bobby:1
                }
            ";
            _repo.Save("B", bValFinal);

            var resA = _repo.Get("A");
            var resB = _repo.Get("B");
            
            Assert.AreEqual(aValFinal, resA.Content);
            Assert.AreEqual(bValFinal, resB.Content);
        }

        [Test]
        public void GetFileHistoryTest()
        {
            _repo.Save("A", "1");
            Thread.Sleep(1000);
            _repo.Save("A", "2");
            var res = _repo.GetFileHistory("A").ToArray();
            Assert.IsNotNull(res);
            Assert.That(res.Count(), Is.GreaterThan(1));
            Assert.That(res[0].Date, Is.GreaterThan(res[1].Date));
        }

        [Test]
        public void DeleteTest()
        {
            string doc = "I am a doc";
            _repo.Save("A", doc);
            _repo.Delete("A");
            var res = _repo.Get("A");
            Assert.AreEqual(String.Empty, res.Content);
        }
    }
}
