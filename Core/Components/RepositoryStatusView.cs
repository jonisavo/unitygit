using System.Linq;
using LibGit2Sharp;
using UIComponents.Core;
using UnityEngine.UIElements;
using UnityGit.GUI.Services;

namespace UnityGit.GUI.Components
{
    [Layout("RepositoryStatusView/RepositoryStatusView.uxml")]
    [Stylesheet("RepositoryStatusView/RepositoryStatusView.style.uss")]
    [Dependency(typeof(IRestoreService), provide: typeof(RestoreService))]
    public class RepositoryStatusView : UnityGitUIComponent
    {
        private readonly IRepository _repository;

        private readonly IRestoreService _restoreService;

        private readonly Button _refreshButton;
        private FileStatusList _trackedList;
        private FileStatusList _untrackedList;
        
        public RepositoryStatusView(IRepository repository, string name)
        {
            _repository = repository;
            _restoreService = Provide<IRestoreService>();
            _restoreService.FileRestored += OnFileRestored;

            this.Q<Label>("repository-status-name-label").text = name;
            this.Q<Label>("repository-status-path-label").text = _repository.Info.Path;
            _refreshButton = this.Q<Button>("repository-status-refresh-button");
            _refreshButton.clicked += RefreshLists;

            RefreshLists();
        }

        ~RepositoryStatusView()
        {
            _restoreService.FileRestored -= OnFileRestored;
            _refreshButton.clicked -= RefreshLists;
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