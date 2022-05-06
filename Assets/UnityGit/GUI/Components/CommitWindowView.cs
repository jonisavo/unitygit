﻿using LibGit2Sharp;
using UIComponents;
using UnityEngine;
using UnityEngine.UIElements;
using UnityGit.Core.Utilities;
using UnityGit.Core.Services;

namespace UnityGit.GUI.Components
{
    [Layout("CommitWindowView/CommitWindowView")]
    [Stylesheet("CommitWindowView/CommitWindowView.style")]
    [Stylesheet("Dimensions")]
    [Dependency(typeof(IStatusService), provide: typeof(StatusService))]
    [Dependency(typeof(ICommitService), provide: typeof(CommitService))]
    public class CommitWindowView : UnityGitUIComponent
    {
        private readonly IStatusService _statusService;
        private readonly ICommitService _commitService;
        
        private readonly ScrollView _statusContainer;

        public CommitWindowView()
        {
            _statusService = Provide<IStatusService>();
            _commitService = Provide<ICommitService>();
            _commitService.CommitCreated += OnCommitCreated;
            _statusContainer = this.Q<ScrollView>("commit-window-view-status-container");

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
        }

        private void Refresh()
        {
            _statusContainer.Clear();
            DrawRepositoryViews();
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
            if (!_statusService.HasProjectRepository())
                _statusContainer.Add(new Label("Project repository is not valid."));
            else
                _statusContainer.Add(new RepositoryStatusView(_statusService.ProjectRepository, Application.productName));
        }

        private void AddPackageRepositoryViews()
        {
            if (!_statusService.HasPackageRepositories())
            {
                _statusContainer.Add(new Label("No package repositories found."));
                return;
            }
            
            foreach (var packageRepo in _statusService.PackageRepositories)
            {
                var packageName = RepositoryUtilities.GetRepositoryName(packageRepo);
                _statusContainer.Add(new RepositoryStatusView(packageRepo, packageName));
            }
        }
    }
}