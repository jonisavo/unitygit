﻿using System;
using LibGit2Sharp;
using UIComponents.Core;
using UnityEngine;
using UnityEngine.UIElements;
using UnityGit.Core;
using UnityGit.Core.Utilities;
using UnityGit.GUI.Services;

namespace UnityGit.GUI.Components
{
    [Layout("CommitWindowView/CommitWindowView.uxml", RelativeTo = AssetPaths.Components)]
    [Stylesheet("CommitWindowView/CommitWindowView.style.uss", RelativeTo = AssetPaths.Components)]
    [Stylesheet("Dimensions.uss", RelativeTo = AssetPaths.Styles)]
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
            _commitAuthorNameTextField = this.Q<TextField>("commit-window-view-author-name-textfield");
            _commitAuthorEmailField = this.Q<TextField>("commit-window-view-author-email-textfield");
            _commitMessageTextField = this.Q<TextField>("commit-window-view-message-textfield");
            _commitMessageTextField.RegisterCallback(new EventCallback<InputEvent>(OnMessageInputChange));            
            _commitButton = this.Q<Button>("commit-window-view-commit-button");
            RefreshCommitButton(_commitMessageTextField.value);
            _commitButton.clicked += CommitSelectedFiles;

            _refreshButton = this.Q<Button>("commit-window-view-refresh-button");
            
            _refreshButton.clicked += () =>
            {
                _status.PopulateRepositories();
                Refresh();
            };
            
            AddToClassList("full-height");
            DrawRepositoryViews();
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