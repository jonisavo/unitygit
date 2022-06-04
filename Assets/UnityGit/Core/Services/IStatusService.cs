using System.Collections.Generic;
using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface IStatusService
    {
        Repository ProjectRepository { get; }

        IReadOnlyList<Repository> PackageRepositories { get; }

        void Clear();

        bool HasProjectRepository();

        bool HasPackageRepositories();

        void PopulateRepositories();

        Signature GetSignature();
    }
}