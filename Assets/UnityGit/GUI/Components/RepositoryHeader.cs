using LibGit2Sharp;
using UIComponents;
using UIComponents.Experimental;
using UnityEngine.UIElements;
using UnityGit.Core.Services;

namespace UnityGit.GUI.Components
{
    [Layout("RepositoryHeader/RepositoryHeader")]
    [Stylesheet("RepositoryHeader/RepositoryHeader.style")]
    [Dependency(typeof(IRepositoryService), provide: typeof(RepositoryService))]
    public class RepositoryHeader : UnityGitUIComponent
    {
        public new class UxmlFactory : UxmlFactory<RepositoryHeader> {}
        
        [Query("repository-header-name-label")]
        private readonly Label _repositoryNameLabel;
        [Query("repository-header-path-label")]
        private readonly Label _repositoryPathLabel;
        [Query("repository-header-refresh-button")]
        private readonly Button _refreshButton;

        private readonly IRepositoryService _repositoryService;
        
        public delegate void RefreshButtonClickedDelegate();
        
        public event RefreshButtonClickedDelegate RefreshButtonClicked;

        public RepositoryHeader()
        {
            _repositoryService = Provide<IRepositoryService>();
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