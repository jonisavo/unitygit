using System;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UIComponents;
using UnityGit.Core.Data;
using UnityGit.Core.Internal;
using Debug = UnityEngine.Debug;

namespace UnityGit.Core.Services
{
    public interface IGitCommandService
    {
        Task<GitProcessResult> Run(GitCommandInfo info);
        
        bool IsRunning { get; }
    }
    
    [Dependency(typeof(IProgressService), provide: typeof(ProgressService))]
    [Dependency(typeof(ILogService), provide: typeof(UnityGitLogService))]
    public sealed partial class GitCommandService : Service, IGitCommandService
    {
        public bool IsRunning { get; private set; }

        public delegate void CommandStartedDelegate();
        
        public event CommandStartedDelegate CommandStarted;
        
        public delegate void CommandFinishedDelegate();
        
        public event CommandFinishedDelegate CommandFinished;

        [Provide]
        private ILogService _logService;

        [Provide]
        private IProgressService _progressService;

        private GitProcess _currentProcess;
        
        private int _progressId;
        
        [CanBeNull]
        public async Task<GitProcessResult> Run(GitCommandInfo info)
        {
            var result = new GitProcessResult();
            
            if (IsRunning)
                return result;

            IsRunning = true;
            
            _progressId = _progressService.Start(
                info.ProgressName,
                info.ProgressDescription,
                new ProgressOptions { Sticky = true }
            );

            _currentProcess = CreateGitProcess(info);

            try
            {
                _logService.LogMessage($"Running {_currentProcess}");
                CommandStarted?.Invoke();
                result = await _currentProcess.Run();
                OnProcessExited(result);
            }
            catch (Exception exception)
            {
                _progressService.FinishWithError(_progressId, "Could not start process");
                _logService.LogException(exception);
            } finally {
                IsRunning = false;
                CommandFinished?.Invoke();
            }
            
            return result;
        }
        
        /// <summary>Used for unit testing.</summary>
        internal void SetIsRunning(bool isRunning)
        {
            IsRunning = isRunning;
        }

        private static GitProcess CreateGitProcess(GitCommandInfo info)
        {
            var process = new Process();
            process.StartInfo.FileName = "git";
            process.StartInfo.Arguments = info.Arguments;
            process.StartInfo.WorkingDirectory = info.Repository.Info.WorkingDirectory;

            var gitProcess = new GitProcess(process);
            
            if (info.TimeoutMs > 0)
                gitProcess.SetTimeout(info.TimeoutMs);

            return gitProcess;
        }

        private void FinishProcessProgress(GitProcessResult result)
        {
            if (result.ExitCode == 0)
            {
                _progressService.FinishWithSuccess(_progressId);
                return;
            }

            string errorMessage;

            if (!result.Started)
                errorMessage = "Process could not be started";
            else if (result.Timeout)
                errorMessage = "Process timed out";
            else
                errorMessage = $"Failed with exit code {result.ExitCode}";
            
            _progressService.FinishWithError(_progressId, errorMessage);
        }

        private void OnProcessExited(GitProcessResult result)
        {
            FinishProcessProgress(result);
            
            foreach (var line in result.Output)
                _logService.LogOutputLine(line);
        }
    }
}
