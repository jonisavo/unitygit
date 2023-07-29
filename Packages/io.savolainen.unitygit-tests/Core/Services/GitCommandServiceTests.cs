using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using UIComponents.Testing;
using UnityEngine.TestTools;
using UnityGit.Core.Data;
using UnityGit.Core.Internal;
using UnityGit.Core.Services;
using LibGit2Sharp;

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

        private static readonly GitCommandInfo CommandInfo =
            new GitCommandInfo("status", "Status", "Running status", null);

        [SetUp]
        public void Setup()
        {
            _logService = Substitute.For<ILogService>();
            _progressService = Substitute.For<IProgressService>();
            _progressService.Start(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ProgressOptions>()).Returns(0);
            _process = new Process();
            _process.StartInfo.FileName = "git";
            _process.StartInfo.Arguments = "status";
            _gitProcess = Substitute.For<GitProcess>(_process);

            GitCommandService.CreateProcessDelegate gitProcessDelegate = (info) =>
            {
                return _gitProcess;
            };

            _gitCommandService = new TestBed<GitCommandService>()
                .WithSingleton(_logService)
                .WithSingleton(_progressService)
                .Instantiate(gitProcessDelegate);
        }

        [Test]
        public void CreateGitProcess_CreatesAProcessForGitCommand()
        {
            var repository = Substitute.For<IRepository>();
            var repoInfo = Substitute.For<RepositoryInformation>();
            repoInfo.WorkingDirectory.Returns("/path/to/repo");
            repository.Info.Returns(repoInfo);

            var info = new GitCommandInfo("status", "Status", "Running status", repository, 1000);

            var gitProcess = GitCommandService.CreateGitProcess(info);

            Assert.AreEqual(gitProcess.GetTimeout(), 1000);
            Assert.AreEqual(gitProcess.GetWorkingDirectory(), "/path/to/repo");
        }

        [UnityTest]
        public IEnumerator Run_WhenNotRunning_ShouldRunGitProcess()
        {
            var result = new GitProcessResult() { ExitCode = 0, Started = true };
            _gitProcess.Run().Returns(Task.FromResult(result));

            var task = _gitCommandService.Run(CommandInfo);
            
            while (!task.IsCompleted)
                yield return null;

            _logService.Received(1).LogMessage("Running 'git status'");
            _logService.DidNotReceive().LogException(Arg.Any<Exception>());
            _progressService.Received(1).FinishWithSuccess(0);
            Assert.IsFalse(_gitCommandService.IsRunning);
            Assert.IsTrue(task.Result.Started);
            Assert.AreEqual(task.Result.ExitCode, 0);
        }

        [UnityTest]
        public IEnumerator Run_WhenSuccessful_ShouldLogMessages()
        {
            var result = new GitProcessResult() { ExitCode = 0, Started = true };
            var normalOutputLine = new OutputLine("Normal message", false);
            var errorOutputLine = new OutputLine("Error message", true);
            result.Output.Add(normalOutputLine);
            result.Output.Add(errorOutputLine);
            _gitProcess.Run().Returns(Task.FromResult(result));

            var task = _gitCommandService.Run(CommandInfo);

            while (!task.IsCompleted)
                yield return null;

            _logService.Received(1).LogMessage("Running 'git status'");
            _logService.DidNotReceive().LogException(Arg.Any<Exception>());
            _logService.Received(1).LogOutputLine(normalOutputLine);
            _logService.Received(1).LogOutputLine(errorOutputLine);
        }

        [UnityTest]
        public IEnumerator Run_WhenRunning_ShouldNotRunGitProcess()
        {
            _gitCommandService.SetIsRunning(true);

            var task = _gitCommandService.Run(CommandInfo);
            
            while (!task.IsCompleted)
                yield return null;

            _gitProcess.DidNotReceive().Run();
            
            Assert.IsFalse(task.Result.Started);
        }

        [UnityTest]
        public IEnumerator Run_WhenExceptionThrown_ShouldLogExceptionAndFinishWithErrorMessage()
        {
            var exception = new Exception("Test Exception");
            _gitProcess.Run().Returns(Task.FromException<GitProcessResult>(exception));

            var task = _gitCommandService.Run(CommandInfo);
            
            while (!task.IsCompleted)
                yield return null;

            _logService.Received(1).LogException(exception);
            _progressService.Received(1).FinishWithError(0, "Could not start process");
            Assert.IsFalse(_gitCommandService.IsRunning);
            Assert.IsNull(task.Result.ExitCode);
        }

        [UnityTest]
        public IEnumerator Run_WhenCommandTimesOut_ShouldFinishWithErrorMessage()
        {
            var result = new GitProcessResult() { ExitCode = null, Started = true, Timeout = true };
            _gitProcess.Run().Returns(Task.FromResult(result));

            var task = _gitCommandService.Run(CommandInfo);

            while (!task.IsCompleted)
                yield return null;

            _progressService.Received(1).FinishWithError(0, "Process timed out");
            Assert.IsTrue(task.Result.Timeout);
        }

        [UnityTest]
        public IEnumerator Run_WhenCommandCanNotBeStarted_ShouldFinishWithErrorMessage()
        {
            var result = new GitProcessResult() { ExitCode = null, Started = false };
            _gitProcess.Run().Returns(Task.FromResult(result));

            var task = _gitCommandService.Run(CommandInfo);

            while (!task.IsCompleted)
                yield return null;

            _progressService.Received(1).FinishWithError(0, "Process could not be started");
            Assert.IsFalse(task.Result.Started);
        }

        [UnityTest]
        public IEnumerator Run_WhenCommandExitsWithNonZeroExitCode_ShouldFinishWithErrorMessage()
        {
            var result = new GitProcessResult() { ExitCode = 1, Started = true };
            _gitProcess.Run().Returns(Task.FromResult(result));

            var task = _gitCommandService.Run(CommandInfo);

            while (!task.IsCompleted)
                yield return null;

            _progressService.Received(1).FinishWithError(0, "Process ended with exit code 1");
            Assert.AreEqual(task.Result.ExitCode, 1);
        }
    }
}
