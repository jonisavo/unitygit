using System.Linq;
using LibGit2Sharp;
using UIComponents;
using UIComponents.Experimental;
using UnityGit.Core.Services;

namespace UnityGit.GUI.Components
{
    [Layout("RepositoryStatusView/RepositoryStatusView")]
    [Stylesheet("RepositoryStatusView/RepositoryStatusView.style")]
    [Dependency(typeof(IRestoreService), provide: typeof(RestoreService))]
    public class RepositoryStatusView : UnityGitUIComponent
    {
        private readonly IRepository _repository;

        [Provide]
        private readonly IRestoreService _restoreService;

        [Query("repository-status-header")]
        private readonly RepositoryHeader _header;

        private FileStatusList _trackedList;
        private FileStatusList _untrackedList;
        
        public RepositoryStatusView(IRepository repository)
        {
            _repository = repository;
            _restoreService.FileRestored += OnFileRestored;
            
            _header.SetRepository(repository);
            _header.RefreshButtonClicked += RefreshLists;

            RefreshLists();
        }

        ~RepositoryStatusView()
        {
            _restoreService.FileRestored -= OnFileRestored;
            _header.RefreshButtonClicked -= RefreshLists;
        }

        private void OnFileRestored(IRepository repository, string filePath)
        {
            if (_repository.Info.Path == repository.Info.Path)
                RefreshLists();
        }

        private void RefreshLists()
        {
            var statusOptions = new StatusOptions
            {
                ExcludeSubmodules = true,
                IncludeIgnored = false,
                IncludeUnaltered = false
            };
            var items = _repository.RetrieveStatus(statusOptions);
            var untrackedItems =
                items.Untracked.Where(item => !item.State.HasFlag(FileStatus.Nonexistent))
                    .ToList();

            var trackedItems =
                items.Where(item => !item.State.HasFlag(FileStatus.NewInWorkdir))
                    .ToList();
            
            if (_trackedList != null)
                Remove(_trackedList);
            if (_untrackedList != null)
                Remove(_untrackedList);

            _trackedList = new FileStatusList(_repository, trackedItems, "Tracked");
            _untrackedList = new FileStatusList(_repository, untrackedItems, "Untracked");
            
            Add(_trackedList);
            Add(_untrackedList);
        }
    }
}