using System.Linq;
using LibGit2Sharp;
using UIComponents;
using UnityGit.Core.Services;

namespace UnityGit.GUI.Components
{
    [Layout("Components/RepositoryStatusView/RepositoryStatusView.uxml")]
    [Stylesheet("Components/RepositoryStatusView/RepositoryStatusView.style.uss")]
    [Dependency(typeof(IRestoreService), provide: typeof(RestoreService))]
    public partial class RepositoryStatusView : UnityGitUIComponent
    {
        private readonly IRepository _repository;

        [Provide]
        private IRestoreService _restoreService;

        [Query("repository-status-header")]
        private RepositoryHeader _header;

        private FileStatusList _trackedList;
        private FileStatusList _untrackedList;
        
        public RepositoryStatusView(IRepository repository)
        {
            _repository = repository;
        }
        
        public override void OnInit()
        {
            _restoreService.FileRestored += OnFileRestored;

            _header.SetRepository(_repository);
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
