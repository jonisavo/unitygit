using LibGit2Sharp;
using UIComponents;
using UnityEngine.UIElements;
using UnityGit.Core.Services;

namespace UnityGit.GUI.Components
{
    [Layout("Components/RepositoryHeader/RepositoryHeader")]
    [Stylesheet("Components/RepositoryHeader/RepositoryHeader.style")]
    [Dependency(typeof(IRepositoryService), provide: typeof(RepositoryService))]
    public partial class RepositoryHeader : UnityGitUIComponent
    {
        public new class UxmlFactory : UxmlFactory<RepositoryHeader> {}
        
        [Query("repository-header-name-label")]
        private Label _repositoryNameLabel;
        [Query("repository-header-path-label")]
        private Label _repositoryPathLabel;
        [Query("repository-header-refresh-button")]
        private Button _refreshButton;
        [Query("repository-header-refresh-button-image")]
        private Image _refreshButtonImage;
        
        [Provide]
        private IRepositoryService _repositoryService;
        
        public delegate void RefreshButtonClickedDelegate();
        
        public event RefreshButtonClickedDelegate RefreshButtonClicked;

        public override void OnInit()
        {
            _refreshButton.clicked += NotifyRefreshButtonClicked;
            _refreshButtonImage.image = Icons.GetIcon(Icons.Name.Refresh);
        }

        ~RepositoryHeader()
        {
            _refreshButton.clicked -= NotifyRefreshButtonClicked;
        }

        private void NotifyRefreshButtonClicked()
        {
            RefreshButtonClicked?.Invoke();
        }
        
        public void SetRepository(IRepository repository)
        {
            if (_repositoryService.IsProjectRepository(repository))
                _repositoryNameLabel.text = _repositoryService.GetProjectRepositoryName();
            else
                _repositoryNameLabel.text = _repositoryService.GetRepositoryName(repository);

            _repositoryPathLabel.text = repository.Info.Path;
        }
    }
}
