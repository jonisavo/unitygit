using LibGit2Sharp;
using UIComponents;
using UnityEngine.UIElements;
using UnityGit.Core.Services;
using Credentials = UnityGit.Core.Data.Credentials;

namespace UnityGit.GUI.Components
{
    [Layout("Components/RepositoryCredentialsListItem/RepositoryCredentialsListItem.uxml")]
    [Stylesheet("Components/RepositoryCredentialsListItem/RepositoryCredentialsListItem.style.uss")]
    [Dependency(typeof(IRepositoryService), provide: typeof(RepositoryService))]
    [Dependency(typeof(ICredentialsService), provide: typeof(CredentialsService))]
    public partial class RepositoryCredentialsListItem : UnityGitUIComponent
    {
        [Query("credentials-list-item-repository-name-label")]
        private Label _repositoryNameLabel;
        [Query("credentials-list-item-username-input")]
        private TextField _usernameInputField;
        [Query("credentials-list-item-password-input")]
        private TextField _passwordInputField;

        [Provide]
        private IRepositoryService _repositoryService;
        [Provide]
        private ICredentialsService _credentialsService;

        private readonly IRepository _repository;
        private Credentials _credentials;

        public RepositoryCredentialsListItem(IRepository repository)
        {
            _repository = repository;
        }

        public override void OnInit()
        {
            if (_repositoryService.IsProjectRepository(_repository))
                _repositoryNameLabel.text = _repositoryService.GetProjectRepositoryName();
            else
                _repositoryNameLabel.text = _repositoryService.GetRepositoryName(_repository);
            
            _credentials = _credentialsService.GetCredentialsForRepository(_repository);
            
            _usernameInputField.value = _credentials.Username;
            _passwordInputField.value = _credentials.Password;
            
            _usernameInputField.RegisterValueChangedCallback(OnUsernameInputChange);
            _passwordInputField.RegisterValueChangedCallback(OnPasswordInputChange);
        }

        private void OnUsernameInputChange(ChangeEvent<string> evt)
        {
            _credentials.Username = evt.newValue;
            _credentialsService.SetCredentialsForRepository(_repository, _credentials);
        }

        private void OnPasswordInputChange(ChangeEvent<string> evt)
        {
            _credentials.Password = evt.newValue;
            _credentialsService.SetCredentialsForRepository(_repository, _credentials);
        }
    }
}
