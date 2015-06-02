using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KeyValueStorage.Utility;
using LibGit2Sharp;

namespace KeyValueStorage.Git.Tests
{
    public class Repo : IDisposable
    {
        private readonly Repository _repo;

        public Repo(string path)
        {
            if (!Repository.IsValid(path))
                Repository.Init(path, true);
            _repo = new Repository(path, new RepositoryOptions() {});
        }

        public string Save(string fileName, string value)
        {
            var contentBytes = System.Text.Encoding.UTF8.GetBytes(value);
            var ms = new MemoryStream(contentBytes);
            var blob = _repo.ObjectDatabase.CreateBlob(ms);



            var committer = new Signature("Computer", "Computer@auto.com", DateTimeOffset.UtcNow);
            var commitLog = _repo.Head.Commits.Take(1).ToArray();

            var commitPrefixString =
                commitLog.Any() && commitLog.First()[fileName] != null
                    ? "Updated "
                    : "Created ";

            var td = commitLog.Any()
                ? TreeDefinition.From(commitLog.First())
                : new TreeDefinition();

            td.Add(fileName, blob, Mode.NonExecutableFile);
            var tree = _repo.ObjectDatabase.CreateTree(td);

            var commit = _repo.ObjectDatabase.CreateCommit(committer, committer, commitPrefixString + fileName, tree, commitLog, false);

            _repo.Refs.UpdateTarget(_repo.Refs.Head, commit.Id);
            return commit.Id.Sha;
        }

        public void Delete(string fileName)
        {
            var committer = new Signature("Computer", "Computer@auto.com", DateTimeOffset.UtcNow);
            var commitLog = _repo.Head.Commits.Take(1).ToArray();

            if (!commitLog.Any())
                return;

            var td = TreeDefinition.From(commitLog.First());

            td.Remove(fileName);
            var tree = _repo.ObjectDatabase.CreateTree(td);

            var commit = _repo.ObjectDatabase.CreateCommit(committer, committer, "Deleted "+ fileName, tree, commitLog, false);

            _repo.Refs.UpdateTarget(_repo.Refs.Head, commit.Id);
        }

        public IEnumerable<FileHistoryResult> GetFileHistory(string fileName)
        {
            return _GetFileHistoryWithEmptyCommits(fileName)
                .Where(q => q.Content != null);
        }

        private IEnumerable<FileHistoryResult> _GetFileHistoryWithEmptyCommits(string fileName)
        {
            return _repo.Head.Commits.Select(c =>
            {
                TreeEntry treeEntry = c[fileName];
                if (treeEntry == null)
                    return FileHistoryResult.NoResult;

                return new FileHistoryResult((Blob) treeEntry.Target, c.Id.Sha, c.Committer.When);
            });
        }

        public FileHistoryResult Get(string key)
        {
            return _GetFileHistoryWithEmptyCommits(key)
                .First();
        }

        public void Dispose()
        {
            _repo.Dispose();
        }
    }
}