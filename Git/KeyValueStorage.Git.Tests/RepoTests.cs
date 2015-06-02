using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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

        private static void DeleteFilesRec(DirectoryInfo di)
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
            string path = @"GitStore\G1";
            CleanPath(path);

            _repo = new Repo(path, new RepoOptions(){Branch = "ggg"});
        }

        public static void CleanPath(string path)
        {
            var di = new System.IO.DirectoryInfo(path);
            if (di.Exists)
            {
                DeleteFilesRec(di);
                di.Delete(true);
            }
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

    [TestFixture]
    public class RepoNetworkTests
    {
        [Test]
        public void RemotePushPull()
        {
            string n1 = @"GitStore\N1";
            RepoTests.CleanPath(n1);
            string n2 = @"GitStore\N2";
            RepoTests.CleanPath(n2);

            using(var r1 = new Repo(n1, new RepoOptions() {RemoteName = "origin", RemoteUrl = n2 }))
            using (var r2 = new Repo(n2, new RepoOptions(){RemoteName = "origin", RemoteUrl = n1}))
            {
                r1.Save("A", "Rabbit");
                r1.Push();
                r2.Pull();
                var r = r2.Get("A").Content;
                Assert.That(r, Is.EqualTo("Rabbit"));

                //p2
                r2.Save("B", "Rat");
                r2.Push();
                r1.Pull();
                Assert.That(r1.Get("B").Content, Is.EqualTo("Rat"));
            }
        }

        [Test]
        public void RemoteMergeConflictResolve()
        {
            string n1 = @"GitStore\N1";
            RepoTests.CleanPath(n1);
            string n2 = @"GitStore\N2";
            RepoTests.CleanPath(n2);

            using(var r1 = new Repo(n1, new RepoOptions() {RemoteName = "origin", RemoteUrl = n2 }))
            using (var r2 = new Repo(n2, new RepoOptions() {RemoteName = "origin", RemoteUrl = n1}))
            {
                r1.Save("A", @"
                {
                    a:1
                }");
                r1.Push();
                r2.Pull();
                r1.Save("A", @"
                {
                    a:1,
                    b:1
                }");
                r2.Save("A", @"
                {
                    a:1,
                    b:2
                }");
                r2.Push();
                r1.Pull();
                r1.Push();
                Assert.AreEqual(r1.Get("A").Content, r2.Get("A").Content);
                Assert.That(r1.Get("A").Content, Is.EqualTo(@"
                {
                    a:1,
                    b:2
                }"));
            }
        }
    }
}
