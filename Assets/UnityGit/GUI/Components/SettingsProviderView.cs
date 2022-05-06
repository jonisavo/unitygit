using UIComponents;
using UnityEngine.UIElements;
using UnityGit.Core.Services;

namespace UnityGit.GUI.Components
{
    [Layout("SettingsProviderView/SettingsProviderView")]
    [Stylesheet("SettingsProviderView/SettingsProviderView.style")]
    [Dependency(typeof(IStatusService), provide: typeof(StatusService))]
    public class SettingsProviderView : UnityGitUIComponent
    {
        private readonly IStatusService _statusService;

        private readonly VisualElement _infoContainer;
        private readonly Button _refreshRepositoriesButton;
        
        public SettingsProviderView()
        {
            _statusService = Provide<IStatusService>();
            
            _refreshRepositoriesButton = this.Q<Button>("settings-provider-refresh-repositories-button");

            _refreshRepositoriesButton.clicked += RefreshRepositories;
            
            AddToClassList("container");

            _infoContainer = this.Q<VisualElement>("settings-provider-info-container");
            
            RefreshInfo();
        }

        ~SettingsProviderView()
        {
            _refreshRepositoriesButton.clicked -= RefreshRepositories;
        }

        private void RefreshRepositories()
        {
            _statusService.PopulateRepositories();
            RefreshInfo();
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
    }
}