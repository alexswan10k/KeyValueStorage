using System;
using LibGit2Sharp;

namespace KeyValueStorage.Git.Tests
{
    public class FileHistoryResult
    {
        private readonly Blob _content;
        private readonly string _commitSha;
        private readonly DateTimeOffset _date;
        public static FileHistoryResult NoResult = new FileHistoryResult(null, null, new DateTimeOffset());

        public FileHistoryResult(Blob target, string commitSha, DateTimeOffset date)
        {
            _content = target;
            _commitSha = commitSha;
            _date = date;
        }

        public string Content
        {
            get
            {
                return _content != null? _content.GetContentText()
                    : string.Empty;
            }
        }

        public string CommitSha
        {
            get { return _commitSha; }
        }

        public DateTimeOffset Date
        {
            get { return _date; }
        }
    }
}