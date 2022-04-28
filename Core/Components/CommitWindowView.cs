using System;
using LibGit2Sharp;
using UIComponents.Core;
using UnityEngine;
using UnityEngine.UIElements;
using UnityGit.Core;
using UnityGit.Core.Utilities;
using UnityGit.GUI.Services;

namespace UnityGit.GUI.Components
{
    [Layout("CommitWindowView/CommitWindowView.uxml")]
    [Stylesheet("CommitWindowView/CommitWindowView.style.uss")]
    [Stylesheet("Dimensions.uss")]
    [InjectDependency(typeof(IUnityGitStatus), provider: typeof(UnityGitStatus))]
    [InjectDependency(typeof(ICommitService), provider: typeof(CommitService))]
    public class CommitWindowView : UIComponent
    {
        private readonly IUnityGitStatus _status;
        private readonly ICommitService _commitService;

        private readonly Button _refreshButton;
        private readonly ScrollView _statusContainer;
        private readonly TextField _commitAuthorNameTextField;
        private readonly TextField _commitAuthorEmailField;
        private readonly TextField _commitMessageTextField;
        private readonly Button _commitButton;
        
        public CommitWindowView()
        {
            _status = Provide<IUnityGitStatus>();
            _commitService = Provide<ICommitService>();
            _commitService.CommitCreated += OnCommitCreated;
            _commitService.FileSelectionChanged += OnFileSelectionChanged;
            _statusContainer = this.Q<ScrollView>("commit-window-view-status-container");

            var signature = _status.GetSignature();
            
            _commitAuthorNameTextField = this.Q<TextField>("commit-window-view-author-name-textfield");

            if (signature != null && !string.IsNullOrEmpty(signature.Name))
                _commitAuthorNameTextField.value = signature.Name;
            
            _commitAuthorEmailField = this.Q<TextField>("commit-window-view-author-email-textfield");

            if (signature != null && !string.IsNullOrEmpty(signature.Email))
                _commitAuthorEmailField.value = signature.Email;
            
            _commitMessageTextField = this.Q<TextField>("commit-window-view-message-textfield");
            
            _commitMessageTextField.RegisterCallback(new EventCallback<InputEvent>(OnMessageInputChange));            
            _commitButton = this.Q<Button>("commit-window-view-commit-button");
            RefreshCommitButton(_commitMessageTextField.value);
            _commitButton.clicked += CommitSelectedFiles;

            AddToClassList("full-height");
            
            Add(new Button(() =>
            {
                if (_commitService is CommitService service)
                    service.LogSelectedFiles();
            }) { text = "Log Selected Files" });
            
            DrawRepositoryViews();
        }

        ~CommitWindowView()
        {
            _commitService.CommitCreated -= OnCommitCreated;
            _commitService.FileSelectionChanged -= OnFileSelectionChanged;
            _commitButton.clicked -= CommitSelectedFiles;
        }

        private void Refresh()
        {
            _statusContainer.Clear();
            DrawRepositoryViews();
        }

        private void OnMessageInputChange(InputEvent evt)
        {
            RefreshCommitButton(evt.newData);
        }

        private void RefreshCommitButton(string commitMessage)
        {
            if (_commitService.GetSelectedCount() == 0)
            {
                _commitButton.SetEnabled(false);
                return;
            }
            
            _commitButton.SetEnabled(!string.IsNullOrEmpty(commitMessage));
        }

        private void OnFileSelectionChanged(IRepository repository, string filePath, bool selected)
        {
            RefreshCommitButton(_commitMessageTextField.value);
        }

        private void CommitSelectedFiles()
        {
            var authorName = _commitAuthorNameTextField.value;
            var authorEmail = _commitAuthorEmailField.value;
            var commitMessage = _commitMessageTextField.value;
            
            var signature = new Signature(authorName, authorEmail, DateTimeOffset.Now);
            
            _commitService.CommitSelected(commitMessage, signature);
        }

        private void OnCommitCreated(Commit commit)
        {
            RefreshCommitButton(_commitMessageTextField.value);
            Refresh();
        }

        private void DrawRepositoryViews()
        {
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