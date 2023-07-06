using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityGit.Core.Data;
using UnityGit.Core.Internal;
using UnityGit.Core.Services;

namespace UnityGit.Tests.Core.Services
{
    [TestFixture]
    public class GitCommandServiceTests
    {
        private GitCommandService _gitCommandService;
        private ILogService _logService;
        private IProgressService _progressService;
        private GitProcess _gitProcess;
        private Process _process;

        [SetUp]
        public void Setup()
        {
            _logService = Substitute.For<ILogService>();
            _progressService = Substitute.For<IProgressService>();
            _process = Substitute.For<Process>();
            _gitProcess = Substitute.For<GitProcess>(_process);

            _gitCommandService = new ServiceTestBed<GitCommandService>()
                .WithSingleton(_logService)
                .WithSingleton(_progressService)
                .Instantiate();
        }

        [UnityTest]
        public IEnumerator Run_WhenNotRunning_ShouldRunGitProcess()
        {
            var info = new GitCommandInfo("status", "Status", "Running status", null);
            _gitProcess.Run().Returns(Task.FromResult(new GitProcessResult()));
            _gitProcess.ToString().Returns("git status");

            var task = _gitCommandService.Run(info);
            
            while (!task.IsCompleted)
                yield return null;

            _logService.Received(1).LogMessage("Running git status");
            _logService.DidNotReceive().LogException(Arg.Any<Exception>());
            _progressService.Received(1).FinishWithSuccess(Arg.Any<int>());
            Assert.IsFalse(_gitCommandService.IsRunning);
            Assert.AreSame(task.Result.Started, true);
            Assert.AreSame(task.Result.ExitCode, 0);
        }

        [UnityTest]
        public IEnumerator Run_WhenRunning_ShouldNotRunGitProcess()
        {
            var info = new GitCommandInfo("status", "Status", "Running status", null);
            _gitCommandService.SetIsRunning(true);

            var task = _gitCommandService.Run(info);
            
            while (!task.IsCompleted)
                yield return null;

            _gitProcess.DidNotReceive().Run();
            
            Assert.IsFalse(task.Result.Started);
        }

        [UnityTest]
        public IEnumerator Run_WhenExceptionThrown_ShouldLogExceptionAndFinishWithErrorMessage()
        {
            var exception = new Exception("Test Exception");
            var info = new GitCommandInfo("status", "Status", "Running status", null);
            _gitProcess.When(process => process.Run()).Throw(exception);

            var task = _gitCommandService.Run(info);
            
            while (!task.IsCompleted)
                yield return null;

            _logService.Received(1).LogException(exception);
            _progressService.Received(1).FinishWithError(Arg.Any<int>(), "Could not start process");
            Assert.IsFalse(_gitCommandService.IsRunning);
            Assert.AreEqual(0, task.Result.ExitCode);
        }

        [UnityTest]
        public IEnumerator Run_WhenProcessExitCodeIsZero_ShouldFinishWithSuccess()
        {
            var info = new GitCommandInfo("status", "Status", "Running status", null);
            var result = new GitProcessResult { ExitCode = 0 };
            _gitProcess.Run().Returns(Task.FromResult(result));
            
            var task = _gitCommandService.Run(info);
            
            while (!task.IsCompleted)
                yield return null;
            
            _progressService.Received(1).FinishWithSuccess(Arg.Any<int>());
        }
    }
}
