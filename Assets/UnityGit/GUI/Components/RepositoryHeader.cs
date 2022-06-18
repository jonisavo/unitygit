using LibGit2Sharp;
using UIComponents;
using UIComponents.Experimental;
using UnityEngine.UIElements;

namespace UnityGit.GUI.Components
{
    [Layout("RepositoryHeader/RepositoryHeader")]
    [Stylesheet("RepositoryHeader/RepositoryHeader.style")]
    public class RepositoryHeader : UnityGitUIComponent
    {
        public new class UxmlFactory : UxmlFactory<RepositoryHeader> {}
        
        [Query("repository-header-name-label")]
        private readonly Label _repositoryNameLabel;
        [Query("repository-header-path-label")]
        private readonly Label _repositoryPathLabel;
        [Query("repository-header-refresh-button")]
        private readonly Button _refreshButton;
        
        public delegate void RefreshButtonClickedDelegate();
        
        public event RefreshButtonClickedDelegate RefreshButtonClicked;

        public RepositoryHeader()
        {
            _refreshButton.clicked += NotifyRefreshButtonClicked;
        }
        
        ~RepositoryHeader()
        {
            _refreshButton.clicked -= NotifyRefreshButtonClicked;
        }

        private void NotifyRefreshButtonClicked()
        {
            RefreshButtonClicked?.Invoke();
        }
        
        public void SetRepositoryAndName(IRepository repository, string repoName)
        {
            _repositoryNameLabel.text = repoName;
            _repositoryPathLabel.text = repository.Info.Path;
        }
    }
}