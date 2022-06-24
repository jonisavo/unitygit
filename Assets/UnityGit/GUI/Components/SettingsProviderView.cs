using UIComponents;
using UIComponents.Experimental;
using UnityEngine.UIElements;
using UnityGit.Core.Services;

namespace UnityGit.GUI.Components
{
    [Layout("SettingsProviderView/SettingsProviderView")]
    [Stylesheet("SettingsProviderView/SettingsProviderView.style")]
    [RootClass("container")]
    [Dependency(typeof(IStatusService), provide: typeof(StatusService))]
    public class SettingsProviderView : UnityGitUIComponent
    {
        [Provide]
        private readonly IStatusService _statusService;

        [Query("settings-provider-info-container")]
        private readonly VisualElement _infoContainer;

        [Query("settings-provider-credentials-container")]
        private readonly VisualElement _credentialsContainer;
        
        [Query("settings-provider-refresh-repositories-button")]
        private readonly Button _refreshRepositoriesButton;
        
        public SettingsProviderView()
        {
            _refreshRepositoriesButton.clicked += RefreshRepositories;

            RefreshInfo();
            
            RefreshCredentialsContainer();
        }

        ~SettingsProviderView()
        {
            _refreshRepositoriesButton.clicked -= RefreshRepositories;
        }

        private void RefreshRepositories()
        {
            _statusService.PopulateRepositories();
            RefreshInfo();
            RefreshCredentialsContainer();
        }

        private void RefreshInfo()
        {
            _infoContainer.Clear();
            
            if (_statusService.HasProjectRepository())
                _infoContainer.Add(new Label("Project repository found."));
            else
                _infoContainer.Add(new Label("Project repository not found."));

            if (_statusService.HasPackageRepositories())
                _infoContainer.Add(new Label($"{_statusService.PackageRepositories.Count} package repositories found."));
            else
                _infoContainer.Add(new Label("No package repositories found."));
        }

        private void RefreshCredentialsContainer()
        {
            _credentialsContainer.Clear();
            
            _credentialsContainer.Add(new Label("Credentials") { name = "settings-provider-credentials-title" });
            
            if (_statusService.HasProjectRepository())
                _credentialsContainer.Add(new RepositoryCredentialsListItem(_statusService.ProjectRepository));
            
            foreach (var repository in _statusService.PackageRepositories)
                _credentialsContainer.Add(new RepositoryCredentialsListItem(repository));
        }
    }
}