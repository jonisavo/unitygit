using UIComponents;
using UnityEngine.UIElements;
using UnityGit.Core;

namespace UnityGit.GUI.Components
{
    [Layout("SettingsProviderView/SettingsProviderView.uxml")]
    [Stylesheet("SettingsProviderView/SettingsProviderView.style.uss")]
    [Dependency(typeof(IUnityGitStatus), provide: typeof(UnityGitStatus))]
    public class SettingsProviderView : UnityGitUIComponent
    {
        private readonly IUnityGitStatus _unityGitStatus;

        private readonly VisualElement _infoContainer;
        private readonly Button _refreshRepositoriesButton;
        
        public SettingsProviderView()
        {
            _unityGitStatus = Provide<IUnityGitStatus>();
            
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
            _unityGitStatus.PopulateRepositories();
            RefreshInfo();
        }

        private void RefreshInfo()
        {
            _infoContainer.Clear();
            
            if (_unityGitStatus.HasProjectRepository())
                _infoContainer.Add(new Label("Project repository found."));
            else
                _infoContainer.Add(new Label("Project repository not found."));

            if (_unityGitStatus.HasPackageRepositories())
                _infoContainer.Add(new Label($"{_unityGitStatus.PackageRepositories.Count} package repositories found."));
            else
                _infoContainer.Add(new Label("No package repositories found."));
        }
    }
}