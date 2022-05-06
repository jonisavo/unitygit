using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LibGit2Sharp;
using UnityGit.Core.Utilities;

namespace UnityGit.Core
{
    public class UnityGitStatus : IUnityGitStatus
    {
        public Repository ProjectRepository { get; private set; }

        public IReadOnlyList<Repository> PackageRepositories => _packageRepositories;

        private readonly List<Repository> _packageRepositories = new List<Repository>();

        public UnityGitStatus()
        {
           PopulateRepositories();
#if UNITY_EDITOR
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += Clear;
#endif
        }

        ~UnityGitStatus()
        {
#if UNITY_EDITOR
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload -= Clear;
#endif
            Clear();
        }

        public void Clear()
        {
            ProjectRepository?.Dispose();
            ProjectRepository = null;
            
            foreach (var packageRepository in PackageRepositories)
                packageRepository?.Dispose();
            
            _packageRepositories.Clear();
        }
        
        public bool HasProjectRepository()
        {
            return ProjectRepository != null;
        }
        
        public bool HasPackageRepositories()
        {
            return PackageRepositories.Count > 0;
        }

        public void PopulateRepositories()
        {
            if (HasProjectRepository())
                ProjectRepository.Dispose();
            
            ProjectRepository = RepositoryUtilities.GetProjectRepository();

            RemoveInvalidRepositories(_packageRepositories);

            var packageRepositories = RepositoryUtilities.GetPackageRepositories();
            
            foreach (var packageRepository in packageRepositories)
            {
                var alreadyExists = _packageRepositories.Any(storedRepository =>
                    RepositoryUtilities.AreRepositoriesEqual(storedRepository, packageRepository));

                if (!alreadyExists)
                    _packageRepositories.Add(packageRepository);
            }
        }

        private void RemoveInvalidRepositories(IList<Repository> repositories)
        {
            var indexesToRemove = new List<int>(repositories.Count);
            
            for (var i = 0; i < repositories.Count; i++)
            {
                var repository = repositories[i];

                if (repository == null || !Repository.IsValid(repository.Info.Path))
                    indexesToRemove.Add(i);
            }
            
            foreach (var index in indexesToRemove)
                repositories.RemoveAt(index);
        }

        [CanBeNull]
        public Signature GetSignature()
        {
            var hasProjectRepository = HasProjectRepository();

            if (!hasProjectRepository && !HasPackageRepositories())
                return null;
            
            if (hasProjectRepository)
            {
                var projectSignature = ProjectRepository.Config.BuildSignature(DateTimeOffset.Now);

                if (projectSignature != null)
                    return projectSignature;
            }

            foreach (var repository in PackageRepositories)
            {
                var packageRepoSignature = repository.Config.BuildSignature(DateTimeOffset.Now);

                if (packageRepoSignature != null)
                    return packageRepoSignature;
            }

            return null;
        }
    }
}