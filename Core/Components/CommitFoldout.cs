using System;
using LibGit2Sharp;
using UIComponents;
using UnityEngine.UIElements;
using UnityGit.Core;
using UnityGit.GUI.Services;

namespace UnityGit.GUI.Components
{
    [Layout("CommitFoldout/CommitFoldout.uxml")]
    [Stylesheet("CommitFoldout/CommitFoldout.style.uss")]
    [Dependency(typeof(IUnityGitStatus), provide: typeof(UnityGitStatus))]
    [Dependency(typeof(ICommitService), provide: typeof(CommitService))]
    public class CommitFoldout : UnityGitUIComponent
    {
        private readonly ICommitService _commitService;
        
        private readonly TextField _commitAuthorNameTextField;
        private readonly TextField _commitAuthorEmailField;
        private readonly TextField _commitMessageTextField;
        private readonly Button _commitButton;
        
        public new class UxmlFactory : UxmlFactory<CommitFoldout> {}
        
        public CommitFoldout()
        {
            _commitService = Provide<ICommitService>();
            _commitService.CommitCreated += OnCommitCreated;
            _commitService.FileSelectionChanged += OnFileSelectionChanged;
            
            var signature = Provide<IUnityGitStatus>().GetSignature();
            
            _commitAuthorNameTextField = this.Q<TextField>("commit-foldout-author-name-textfield");

            if (signature != null && !string.IsNullOrEmpty(signature.Name))
                _commitAuthorNameTextField.value = signature.Name;
            
            _commitAuthorEmailField = this.Q<TextField>("commit-foldout-author-email-textfield");

            if (signature != null && !string.IsNullOrEmpty(signature.Email))
                _commitAuthorEmailField.value = signature.Email;
            
            _commitMessageTextField = this.Q<TextField>("commit-foldout-message-textfield");
            
            _commitMessageTextField.RegisterCallback(new EventCallback<InputEvent>(OnMessageInputChange));            
            _commitButton = this.Q<Button>("commit-foldout-commit-button");
            RefreshCommitButton(_commitMessageTextField.value);
            _commitButton.clicked += CommitSelectedFiles;
        }
        
        ~CommitFoldout()
        {
            _commitService.CommitCreated -= OnCommitCreated;
            _commitService.FileSelectionChanged -= OnFileSelectionChanged;
            _commitButton.clicked -= CommitSelectedFiles;
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
        
        private void OnMessageInputChange(InputEvent evt)
        {
            RefreshCommitButton(evt.newData);
        }

        private void OnFileSelectionChanged(IRepository repository, string filePath, bool selected)
        {
            RefreshCommitButton(_commitMessageTextField.value);
        }
        
        private void OnCommitCreated(Commit commit)
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
    }
}