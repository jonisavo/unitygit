using LibGit2Sharp;
using UIComponents;
using UnityEngine.UIElements;
using UnityGit.Core.Services;

namespace UnityGit.GUI.Components
{
    [Layout("Components/CommitWindowView/CommitWindowView")]
    [Stylesheet("Components/CommitWindowView/CommitWindowView.style")]
    [RootClass("ugit-full-height")]
    [Dependency(typeof(IStatusService), provide: typeof(StatusService))]
    [Dependency(typeof(ICommitService), provide: typeof(CommitService))]
    public partial class CommitWindowView : UnityGitUIComponent
    {
        [Provide]
        private IStatusService _statusService;
        [Provide]
        private ICommitService _commitService;
        
        [Query("commit-window-view-status-container")]
        private ScrollView _statusContainer;

        public override void OnInit()
        {
            _commitService.CommitCreated += OnCommitCreated;
            DrawRepositoryViews();
        }

        ~CommitWindowView()
        {
            _commitService.CommitCreated -= OnCommitCreated;
        }

        private void Refresh()
        {
            _statusContainer.Clear();
            DrawRepositoryViews();
        }

        private void OnCommitCreated(Commit commit)
        {
            Refresh();
        }

        private void DrawRepositoryViews()
        {
            AddProjectRepositoryView();
            AddPackageRepositoryViews();
        }

        private void AddProjectRepositoryView()
        {
            if (!_statusService.HasProjectRepository())
                _statusContainer.Add(new Label("Project repository is not valid."));
            else
                _statusContainer.Add(new RepositoryStatusView(_statusService.ProjectRepository));
        }

        private void AddPackageRepositoryViews()
        {
            if (!_statusService.HasPackageRepositories())
            {
                _statusContainer.Add(new Label("No package repositories found."));
                return;
            }
            
            foreach (var packageRepo in _statusService.PackageRepositories)
                _statusContainer.Add(new RepositoryStatusView(packageRepo));
        }
    }
}
