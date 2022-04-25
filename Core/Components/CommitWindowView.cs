using UIComponents.Core;
using UnityEngine;
using UnityEngine.UIElements;
using UnityGit.Core;
using UnityGit.Core.Utilities;

namespace UnityGit.GUI.Components
{
    [Layout("CommitWindowView/CommitWindowView.uxml", RelativeTo = AssetPaths.Components)]
    [Stylesheet("CommitWindowView/CommitWindowView.style.uss", RelativeTo = AssetPaths.Components)]
    [Stylesheet("Dimensions.uss", RelativeTo = AssetPaths.Styles)]
    public class CommitWindowView : UIComponent
    {
        private readonly UnityGitStatus _status;
        private readonly ScrollView _statusContainer;
        
        public CommitWindowView(UnityGitStatus status)
        {
            _status = status;
            _statusContainer = this.Q<ScrollView>("commit-window-view-status-container");
            AddToClassList("full-height");
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