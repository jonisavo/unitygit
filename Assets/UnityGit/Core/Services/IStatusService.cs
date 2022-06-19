using System.Collections.Generic;
using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface IStatusService
    {
        IRepository ProjectRepository { get; }

        IReadOnlyList<IRepository> PackageRepositories { get; }

        void Clear();

        bool HasProjectRepository();

        bool HasPackageRepositories();
        
        void PopulateRepositories();

        Signature GetSignature();
    }
}