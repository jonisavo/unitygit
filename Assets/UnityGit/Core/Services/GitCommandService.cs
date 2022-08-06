using System;
using System.Diagnostics;
using JetBrains.Annotations;
using UIComponents;
using UIComponents.Experimental;
using UnityGit.Core.Internal;
using UnityGit.UnityGit.Core.Data;
using Debug = UnityEngine.Debug;

namespace UnityGit.Core.Services
{
    [Dependency(typeof(ILogService), provide: typeof(UnityGitLogService))]
    public class GitCommandService : IGitCommandService
    {
        public bool IsRunning { get; private set; }

        public delegate void CommandStartedDelegate();
        
        public event CommandStartedDelegate CommandStarted;
        
        public delegate void CommandFinishedDelegate();
        
        public event CommandFinishedDelegate CommandFinished;

        [Provide]
        private readonly ILogService _logService;

        private Process _currentProcess;
        
        private int _progressId;
        
        [CanBeNull]
        public Process Run(GitCommandInfo info)
        {
            if (IsRunning)
                return null;

            IsRunning = true;
            
            _progressId = ProgressWrapper.Start(
                info.ProgressName, info.ProgressDescription
            );
            
            _currentProcess = CreateProcess(info);

            try
            {
                _currentProcess.Start();
                CommandStarted?.Invoke();
            }
            catch
            {
                _currentProcess.Exited -= OnProcessExited;
                ProgressWrapper.FinishWithError(_progressId, "Could not start process");
                IsRunning = false;
            }
            
            return _currentProcess;
        }

        private Process CreateProcess(GitCommandInfo info)
        {
            var process = new Process();
            process.StartInfo.FileName = "git";
            process.StartInfo.Arguments = info.Arguments;
            process.StartInfo.WorkingDirectory = info.Repository.Info.WorkingDirectory;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.EnableRaisingEvents = true;
            process.OutputDataReceived += OnOutputDataReceived;
            process.ErrorDataReceived += OnErrorDataReceived;
            process.Exited += OnProcessExited;
            return process;
        }
        
        private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logService.LogMessage(e.Data);
        }
        
        private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logService.LogError(e.Data);
        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            _currentProcess.Exited -= OnProcessExited;
            IsRunning = false;
            
            _logService.LogMessage($"Process exited with exit code {_currentProcess.ExitCode}");
            
            var isSuccessful = _currentProcess.ExitCode == 0;
            
            CommandFinished?.Invoke();

            if (isSuccessful)
                ProgressWrapper.FinishWithSuccess(_progressId);
            else
                ProgressWrapper.FinishWithError(_progressId, $"Failed with exit code {_currentProcess.ExitCode}");
        }
    }
}