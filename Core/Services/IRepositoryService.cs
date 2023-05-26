using System.Collections.Generic;
using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface IRepositoryService
    {
        IRepository GetProjectRepository();
        
        List<IRepository> GetPackageRepositories();
        
        bool IsProjectRepository(IRepository repository);
        
        string GetProjectRepositoryName();
        
        string GetRepositoryName(IRepository repository);
        
        bool AreRepositoriesEqual(IRepository repositoryOne, IRepository repositoryTwo);
    }
}