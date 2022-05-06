using System.Collections.Generic;
using LibGit2Sharp;

namespace UnityGit.Core
{
    public interface IUnityGitStatus
    {
        public Repository ProjectRepository { get; }

        public IReadOnlyList<Repository> PackageRepositories { get; }

        public void Clear();

        public bool HasProjectRepository();

        public bool HasPackageRepositories();

        public void PopulateRepositories();

        public Signature GetSignature();
    }
}