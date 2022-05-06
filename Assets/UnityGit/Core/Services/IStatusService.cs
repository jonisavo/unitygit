using System.Collections.Generic;
using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface IStatusService
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