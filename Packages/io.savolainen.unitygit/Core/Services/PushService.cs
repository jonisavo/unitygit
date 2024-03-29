﻿using LibGit2Sharp;
using UIComponents;
using UnityGit.Core.Data;

namespace UnityGit.Core.Services
{
    public interface IPushService
    {
        void Push(IRepository repository, Branch branch);
    }
    
    [Dependency(typeof(IDialogService), provide: typeof(DialogService))]
    [Dependency(typeof(IGitCommandService), provide: typeof(GitCommandService))]
    public sealed partial class PushService : Service, IPushService
    {
        [Provide]
        private IDialogService _dialogService;
        [Provide]
        private IGitCommandService _gitCommandService;

        public void Push(IRepository repository, Branch branch)
        {
            if (_dialogService.Confirm($"Push branch {branch.FriendlyName} to remote {branch.RemoteName}?"))
                DoPush(repository, branch);
        }
        
        private async void DoPush(IRepository repository, Branch branch)
        {
            var pushCommandInfo = new GitCommandInfo(
                $"push {branch.RemoteName} {branch.FriendlyName}",
                "Pushing...",
                $"Pushing {branch.FriendlyName} to {branch.RemoteName}",
                repository
            );

            var result = await _gitCommandService.Run(pushCommandInfo);

            if (result.ExitCode != 0)
                _dialogService.Error("Pushing failed. Refer to the UnityGit log for more information.");
        }
    }
}
