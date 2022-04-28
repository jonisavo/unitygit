using UIComponents.Core;
using UnityEngine;
using UnityEngine.UIElements;
using UnityGit.Core;
using UnityGit.Core.Utilities;
using UnityGit.GUI.Services;

namespace UnityGit.GUI.Components
{
    [Layout("CommitWindowView/CommitWindowView.uxml", RelativeTo = AssetPaths.Components)]
    [Stylesheet("CommitWindowView/CommitWindowView.style.uss", RelativeTo = AssetPaths.Components)]
    [Stylesheet("Dimensions.uss", RelativeTo = AssetPaths.Styles)]
    [InjectDependency(typeof(IUnityGitStatus), provider: typeof(UnityGitStatus))]
    [InjectDependency(typeof(ICommitService), provider: typeof(CommitService))]
    public class CommitWindowView : UIComponent
    {
        private readonly IUnityGitStatus _status;
        private readonly ICommitService _commitService;

        private readonly Button _refreshButton;
        private readonly ScrollView _statusContainer;
        private readonly TextField _commitMessageTextField;
        private readonly Button _commitButton;
        
        public CommitWindowView()
        {
            _status = Provide<IUnityGitStatus>();
            _commitService = Provide<ICommitService>();
            _statusContainer = this.Q<ScrollView>("commit-window-view-status-container");
            _commitMessageTextField = this.Q<TextField>("commit-window-view-commit-textfield");
            _commitButton = this.Q<Button>("commit-window-view-commit-button");

            _commitButton.clicked += () =>
            {
                _commitService.LogSelectedFiles();
            };

            _refreshButton = this.Q<Button>("commit-window-view-refresh-button");
            
            _refreshButton.clicked += () =>
            {
                _status.PopulateRepositories();
                Refresh();
            };
            
            AddToClassList("full-height");
            DrawRepositoryViews();
        }

        private void Refresh()
        {
            _statusContainer.Clear();
            DrawRepositoryViews();
        }

        private void DrawRepositoryViews()
        {
            AddProjectRepositoryView();
            AddPackageRepositoryViews();
        }

        private void AddProjectRepositoryView()
        {
            if (!_status.HasProjectRepository())
                _statusContainer.Add(new Label("Project repository is not valid."));
            else
                _statusContainer.Add(new RepositoryStatusView(_status.ProjectRepository, Application.productName));
        }

        private void AddPackageRepositoryViews()
        {
            if (!_status.HasPackageRepositories())
            {
                _statusContainer.Add(new Label("No package repositories found."));
                return;
            }
            
            foreach (var packageRepo in _status.PackageRepositories)
            {
                var packageName = RepositoryUtilities.GetRepositoryName(packageRepo);
                _statusContainer.Add(new RepositoryStatusView(packageRepo, packageName));
            }
        }
    }
}