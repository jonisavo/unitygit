using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;
using UIComponents;
using UIComponents.DependencyInjection;

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
    }
    
    [Dependency(typeof(IRepositoryService), provide: typeof(RepositoryService))]
    public sealed partial class StatusService : Service, IStatusService
    {
        public IRepository ProjectRepository { get; private set; }

        public IReadOnlyList<IRepository> PackageRepositories => _packageRepositories;

        private readonly List<IRepository> _packageRepositories = new List<IRepository>();

        [Provide]
        private IRepositoryService _repositoryService;

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

                if (repository == null || !_repositoryService.IsValid(repository))
                    indexesToRemove.Add(i);
            }
            
            foreach (var index in indexesToRemove)
                repositories.RemoveAt(index);
        }
    }

    /// <summary>
    /// Contains the user's repositories.
    /// </summary>
    [UnityEditor.InitializeOnLoad]
    public static partial class StaticStatusContainer
    {
        [Dependency(typeof(IStatusService), provide: typeof(StatusService))]
        private partial class Container : Service
        {
            [Provide]
            public IStatusService StatusService;
        }

        private static readonly Container StaticContainer;

        static StaticStatusContainer()
        {
            StaticContainer = new Container();
            StaticContainer.StatusService.PopulateRepositories();
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += Clear;
        }

        private static void Clear()
        {
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload -= Clear;
            StaticContainer.StatusService.Clear();
        }
    }
}
