using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KeyValueStorage.Utility;
using LibGit2Sharp;

namespace KeyValueStorage.Git.Tests
{
    public class RepoOptions
    {
        public string RemoteName { get; set; }
        public string RemoteUrl { get; set; }

        public string Branch { get; set; }
    }
    public class Repo : IDisposable
    {
        private readonly Repository _repo;
        private MergeOptions _mergeOptions = new MergeOptions()
        {
            MergeFileFavor = MergeFileFavor.Ours, 
            CommitOnSuccess = true,
            FastForwardStrategy = FastForwardStrategy.Default,
            FileConflictStrategy = CheckoutFileConflictStrategy.Merge
        };
        private FetchOptions _fetchOptions = new FetchOptions()
        {
            TagFetchMode = TagFetchMode.All
        };

        private PushOptions _pushOptions = new PushOptions(){};
        private string _branchName;

        public Repo(string path, RepoOptions options = null)
        {
            if(options == null)
                options = new RepoOptions();

            if (!Repository.IsValid(path))
                Repository.Init(path, true);
            var repositoryOptions = new RepositoryOptions() {};
            _repo = new Repository(path, repositoryOptions);

            if (!string.IsNullOrEmpty(options.RemoteName))
                _repo.Network.Remotes.Add(options.RemoteName, options.RemoteUrl);
            _branchName = options.Branch ?? "master";
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

            if (_repo.Branches[_branchName] == null)
            {
                _repo.CreateBranch(_branchName, commit);
                //_repo.Branches[_branchName].
            }

            UpdateRefs(commit);
            
            return commit.Id.Sha;
        }

        public void Delete(string fileName)
        {
            var committer = CreateSignature();
            Branch branch = _repo.Branches[_branchName];
            if (branch == null)
                return;

            var commitLog = branch.Commits.Take(1).ToArray();

            if (!commitLog.Any())
                return;

            var td = TreeDefinition.From(commitLog.First());

            td.Remove(fileName);
            var tree = _repo.ObjectDatabase.CreateTree(td);

            var commit = _repo.ObjectDatabase.CreateCommit(committer, committer, "Deleted "+ fileName, tree, commitLog, false);

            UpdateRefs(commit);
        }

        private void UpdateRefs(Commit commit)
        {
            Reference branchRef = _repo.Refs["refs/heads/" + _branchName];
            _repo.Refs.UpdateTarget(branchRef, commit.Id);
            _repo.Refs.UpdateTarget(_repo.Refs.Head, branchRef);
        }

        private static Signature CreateSignature()
        {
            return new Signature("Computer", "Computer@auto.com", DateTimeOffset.UtcNow);
        }

        public IEnumerable<FileHistoryResult> GetFileHistory(string fileName)
        {
            return _GetFileHistoryWithEmptyCommits(fileName)
                .Where(q => q.Content != null);
        }

        private IEnumerable<FileHistoryResult> _GetFileHistoryWithEmptyCommits(string fileName)
        {
            Branch branch = _repo.Branches[_branchName];
            if (branch == null)
                return Enumerable.Empty<FileHistoryResult>();

            return branch.Commits.Select(c =>
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
                .FirstOrDefault();
        }

        public void Pull()
        {
            _repo.Network.Fetch(_repo.Network.Remotes.First(), _fetchOptions);
            
            //merge??

            //_repo.Network.Pull(CreateSignature(),
            //    new PullOptions()
            //    {
            //        FetchOptions = _fetchOptions,
            //        MergeOptions = _mergeOptions
            //    });
        }

        public void Push()
        {
            //This should hopefully be able to be cut down once #514 is resolved
            Remote remote = _repo.Network.Remotes.First();
            Branch branch = _repo.Branches[_branchName];
            string refspec = string.Format("{0}:{1}",
                branch.CanonicalName, branch.CanonicalName);

            _repo.Network.Push(remote, refspec, _pushOptions);

            _repo.Branches.Update(_repo.Head, delegate(BranchUpdater updater)
            {
                updater.Remote = remote.Name;
                updater.UpstreamBranch = _repo.Head.CanonicalName;
            });
        }

        public void Dispose()
        {
            _repo.Dispose();
        }
    }
}