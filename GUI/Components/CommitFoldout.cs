using System;
using LibGit2Sharp;
using UIComponents;
using UnityEngine.UIElements;
using UnityGit.Core.Services;

namespace UnityGit.GUI.Components
{
    [Layout("Components/CommitFoldout/CommitFoldout.uxml")]
    [Stylesheet("Components/CommitFoldout/CommitFoldout.style.uss")]
    [Dependency(typeof(ISignatureService), provide: typeof(SignatureService))]
    [Dependency(typeof(ICommitService), provide: typeof(CommitService))]
    public partial class CommitFoldout : UnityGitUIComponent
    {
        [Provide]
        private ICommitService _commitService;

        [Query("commit-foldout-foldout")]
        private Foldout _foldout;
        [Query("commit-foldout-author-name-textfield")]
        private TextField _commitAuthorNameTextField;
        [Query("commit-foldout-author-email-textfield")]
        private TextField _commitAuthorEmailField;
        [Query("commit-foldout-message-textfield")]
        private TextField _commitMessageTextField;
        [Query("commit-foldout-commit-button")]
        private Button _commitButton;
        
        public new class UxmlFactory : UxmlFactory<CommitFoldout> {}

        public override void OnInit()
        {
            _commitService.CommitCreated += OnCommitCreated;
            _commitService.FileSelectionChanged += OnFileSelectionChanged;

            var signature = Provide<ISignatureService>().GetSignature();

            if (signature != null && !string.IsNullOrEmpty(signature.Name))
                _commitAuthorNameTextField.value = signature.Name;

            if (signature != null && !string.IsNullOrEmpty(signature.Email))
                _commitAuthorEmailField.value = signature.Email;

            _commitMessageTextField.RegisterCallback(new EventCallback<InputEvent>(OnMessageInputChange));
            _commitButton.clicked += CommitSelectedFiles;
            
            RefreshFoldoutText();
            RefreshCommitButton(_commitMessageTextField.value);
        }

        ~CommitFoldout()
        {
            _commitService.CommitCreated -= OnCommitCreated;
            _commitService.FileSelectionChanged -= OnFileSelectionChanged;
            _commitButton.clicked -= CommitSelectedFiles;
        }

        private void RefreshFoldoutText()
        {
            var selectedFileCountForCommit = _commitService.GetSelectedCount();

            if (selectedFileCountForCommit == 0)
                _foldout.text = "Commit...";
            else
                _foldout.text = $"Commit {selectedFileCountForCommit.ToString()} files...";
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
            RefreshFoldoutText();
            RefreshCommitButton(_commitMessageTextField.value);
        }
        
        private void OnCommitCreated(Commit commit)
        {
            RefreshFoldoutText();
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
