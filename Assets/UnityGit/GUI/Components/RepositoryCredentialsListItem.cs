using LibGit2Sharp;
using UIComponents;
using UIComponents.Experimental;
using UnityEngine.UIElements;
using UnityGit.Core.Services;
using Credentials = UnityGit.UnityGit.Core.Data.Credentials;

namespace UnityGit.GUI.Components
{
    [Layout("RepositoryCredentialsListItem/RepositoryCredentialsListItem")]
    [Stylesheet("RepositoryCredentialsListItem/RepositoryCredentialsListItem.style")]
    [Dependency(typeof(IRepositoryService), provide: typeof(RepositoryService))]
    [Dependency(typeof(ICredentialsService), provide: typeof(CredentialsService))]
    public class RepositoryCredentialsListItem : UnityGitUIComponent
    {
        [Query("credentials-list-item-repository-name-label")]
        private readonly Label _repositoryNameLabel;
        [Query("credentials-list-item-username-input")]
        private readonly TextField _usernameInputField;
        [Query("credentials-list-item-password-input")]
        private readonly TextField _passwordInputField;

        [Provide]
        private readonly IRepositoryService _repositoryService;
        [Provide]
        private readonly ICredentialsService _credentialsService;

        private readonly IRepository _repository;
        private Credentials _credentials;

        public RepositoryCredentialsListItem(IRepository repository)
        {
            _repository = repository;

            if (_repositoryService.IsProjectRepository(repository))
                _repositoryNameLabel.text = _repositoryService.GetProjectRepositoryName();
            else
                _repositoryNameLabel.text = _repositoryService.GetRepositoryName(repository);
            
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