using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;
using UIComponents;

namespace UnityGit.Core.Services
{
    [Dependency(typeof(IRepositoryService), provide: typeof(RepositoryService))]
    public partial class StatusService : Service, IStatusService
    {
        public IRepository ProjectRepository { get; private set; }

        public IReadOnlyList<IRepository> PackageRepositories => _packageRepositories;

        private readonly List<IRepository> _packageRepositories = new List<IRepository>();

        [Provide]
        private IRepositoryService _repositoryService;

        public StatusService()
        {
            PopulateRepositories();
#if UNITY_EDITOR
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += Clear;
#endif
        }

        ~StatusService()
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
            
            ProjectRepository = _repositoryService.GetProjectRepository();

            RemoveInvalidRepositories(_packageRepositories);

            var packageRepositories = _repositoryService.GetPackageRepositories();
            
            foreach (var packageRepository in packageRepositories)
            {
                var alreadyExists = _packageRepositories.Any(storedRepository =>
                    _repositoryService.AreRepositoriesEqual(storedRepository, packageRepository));

                if (!alreadyExists)
                    _packageRepositories.Add(packageRepository);
            }
        }

        private void RemoveInvalidRepositories(IList<IRepository> repositories)
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
    }
}
