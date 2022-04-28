using UIComponents.Core;
using UnityEngine.UIElements;
using UnityGit.Core;

namespace UnityGit.GUI.Components
{
    [Layout("SettingsProviderView/SettingsProviderView.uxml")]
    [Stylesheet("SettingsProviderView/SettingsProviderView.style.uss")]
    [InjectDependency(typeof(IUnityGitStatus), provider: typeof(UnityGitStatus))]
    public class SettingsProviderView : UIComponent
    {
        private readonly IUnityGitStatus _unityGitStatus;

        private readonly VisualElement _infoContainer;
        
        public SettingsProviderView()
        {
            _unityGitStatus = Provide<UnityGitStatus>();
            
            var refreshRepositoriesButton = this.Q<Button>("settings-provider-refresh-repositories-button");

            refreshRepositoriesButton.clicked += RefreshRepositories;
            
            AddToClassList("container");

            _infoContainer = this.Q<VisualElement>("settings-provider-info-container");
            
            RefreshInfo();
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