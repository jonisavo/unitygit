using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using LibGit2Sharp;

namespace UnityGit.Core.Utilities
{
    public static class RepositoryUtilities
    {
        [CanBeNull]
        public static Repository GetProjectRepository()
        {
            var projectFullPath = Directory.GetCurrentDirectory();
            var gitRepositoryPath = Path.Combine(projectFullPath, ".git");

            if (!Directory.Exists(gitRepositoryPath))
                return null;

            if (!Repository.IsValid(gitRepositoryPath))
                return null;

            return new Repository(gitRepositoryPath);
        }
        
        public static List<Repository> GetPackageRepositories()
        {
            var repositories = new List<Repository>();
            var packagesFullPath = Path.GetFullPath("Packages");
            var packagesDirectoryInfo = new DirectoryInfo(packagesFullPath);

            AddRepositoriesInDirectory(packagesDirectoryInfo, repositories);

            return repositories;
        }

        public static void AddRepositoriesInDirectory(DirectoryInfo directoryInfo,
            List<Repository> repositories)
        {
            var gitDirectories =
                directoryInfo.GetDirectories(".git", SearchOption.AllDirectories);
            
            foreach (var directory in gitDirectories)
            {
                var alreadyExists =
                    repositories.Any(repository => repository.Info.Path.Equals(directory.FullName));

                if (alreadyExists)
                    continue;

                if (Repository.IsValid(directory.FullName))
                    repositories.Add(new Repository(directory.FullName));
            }
        }

        public static string GetRepositoryName(Repository repository)
        {
            var packagePath = repository.Info.Path;
            
            var packagePathParts = packagePath.Split(Path.DirectorySeparatorChar);

            int packageNameIndex;

            if (packagePath[packagePath.Length - 1] == Path.DirectorySeparatorChar)
                packageNameIndex = packagePathParts.Length - 3;
            else
                packageNameIndex = packagePathParts.Length - 2;
            
            return packagePathParts[packageNameIndex];
        }

        public static bool AreRepositoriesEqual(Repository repositoryOne, Repository repositoryTwo)
        {
            if (repositoryOne == null || repositoryTwo == null)
                return false;
            
            return repositoryOne.Info.Path.Equals(repositoryTwo.Info.Path);
        }
    }
}