using System;
using System.Diagnostics;
using LibGit2Sharp;
using UIComponents;
using UnityGit.Core.Internal;
using Debug = UnityEngine.Debug;

namespace UnityGit.Core.Services
{
    [Dependency(typeof(IDialogService), provide: typeof(DialogService))]
    public class PushService : Service, IPushService
    {
        public bool IsPushing { get; private set; }

        private Process _pushProcess;
        private int _progressId;
        
        private readonly IDialogService _dialogService;

        public delegate void PushStartedDelegate();
        public event PushStartedDelegate PushStarted;
        
        public delegate void PushFinishedDelegate(bool success);
        public event PushFinishedDelegate PushFinished;

        private static readonly ProcessStartInfo PushProcessStartInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = "push",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        public PushService()
        {
            _dialogService = Provide<IDialogService>();
        }
        
        public void Push(IRepository repository, Branch branch)
        {
            if (_dialogService.Confirm($"Push branch {branch.FriendlyName} to remote {branch.RemoteName}?"))
                DoPush(repository, branch);
        }
        
        private void DoPush(IRepository repository, Branch branch)
        {
            IsPushing = true;
            
            PushStarted?.Invoke();
            
            PushProcessStartInfo.Arguments = $"push {branch.RemoteName} {branch.FriendlyName}";

            _progressId = ProgressWrapper.Start(
                "Pushing...",
                $"Pushing {branch.FriendlyName} to {branch.RemoteName}"
            );
            
            _pushProcess = new Process();
            _pushProcess.StartInfo = PushProcessStartInfo;
            _pushProcess.StartInfo.WorkingDirectory = repository.Info.WorkingDirectory;
            _pushProcess.EnableRaisingEvents = true;
            _pushProcess.Exited += OnPushFinished;

            try
            {
                _pushProcess.Start();
            } 
            catch (Exception e)
            {
                HandleError($"Failed to start git push: {e.Message}");
            }
        }

        private void HandleError(string message)
        {
            _pushProcess.Exited -= OnPushFinished;
            ProgressWrapper.FinishWithError(_progressId, message);
            Debug.LogError(message);
            IsPushing = false;
            PushFinished?.Invoke(false);
        }
        
        private void OnPushFinished(object sender, EventArgs e)
        {
            _pushProcess.Exited -= OnPushFinished;
            IsPushing = false;

            var isSuccessful = _pushProcess.ExitCode == 0;

            if (isSuccessful)
                ProgressWrapper.FinishWithSuccess(_progressId);
            else
                ProgressWrapper.FinishWithError(_progressId, $"Failed to push, received exit code {_pushProcess.ExitCode}");

            PushFinished?.Invoke(isSuccessful);
        }
    }
}