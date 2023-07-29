using System;
using System.IO;
using LibGit2Sharp;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using UIComponents.Testing;
using UnityGit.Core.Services;

namespace UnityGit.Tests.Core.Services
{
    [TestFixture]
    public class RestoreServiceTests
    {
        private IDialogService _dialogService;
        private ILogService _logService;
        private ICheckoutService _checkoutService;
        private IFileService _fileService;
        private RestoreService _restoreService;

        [SetUp]
        public void SetUp()
        {
            _dialogService = Substitute.For<IDialogService>();
            _logService = Substitute.For<ILogService>();
            _checkoutService = Substitute.For<ICheckoutService>();
            _fileService = Substitute.For<IFileService>();

            _restoreService = new TestBed<RestoreService>()
                .WithSingleton(_logService)
                .WithSingleton(_checkoutService)
                .WithSingleton(_dialogService)
                .WithSingleton(_fileService)
                .Instantiate();
        }
        
        [Test]
        public void RestoreFile_IfFileDoesNotExist_ReturnsFalse()
        {
            var repository = Substitute.For<IRepository>();
            repository.RetrieveStatus("test").Returns(FileStatus.Nonexistent);

            var result = _restoreService.RestoreFile(repository, "test");

            Assert.IsFalse(result);
        }

        [Test]
        public void RestoreFile_IfNotConfirmed_ReturnsFalse()
        {
            var repository = Substitute.For<IRepository>();
            repository.RetrieveStatus("new").Returns(FileStatus.NewInWorkdir);
            repository.RetrieveStatus("notnew").Returns(FileStatus.ModifiedInWorkdir);
            _dialogService.Confirm(Arg.Any<string>()).Returns(false);
            
            var resultNew = _restoreService.RestoreFile(repository, "new");
            var resultNotNew = _restoreService.RestoreFile(repository, "notnew");

            Assert.IsFalse(resultNew);
            Assert.IsFalse(resultNotNew);
        }

        [Test]
        public void RestoreFile_IfRestored_EmitsEvent()
        {
            _dialogService.Confirm(Arg.Any<string>()).Returns(true);
            var repository = TestUtilities.CreateMockRepository("path");
            repository.RetrieveStatus("file").Returns(FileStatus.ModifiedInWorkdir);
            
            var wasRaised = false;
            
            _restoreService.FileRestored += (repo, filePath) =>
            {
                wasRaised = true;
                Assert.AreEqual(repository, repo);
                Assert.AreEqual("file", filePath);
            };
            
            var result = _restoreService.RestoreFile(repository, "file");
            
            Assert.IsTrue(wasRaised);
            Assert.IsTrue(result);
        }
        
        [Test]
        public void RestoreFile_IfDeleted_EmitsEvent()
        {
            _dialogService.Confirm(Arg.Any<string>()).Returns(true);
            var repository = TestUtilities.CreateMockRepository("path");
            repository.RetrieveStatus("file").Returns(FileStatus.NewInWorkdir);
            
            var wasRaised = false;
            
            _restoreService.FileRestored += (repo, filePath) =>
            {
                wasRaised = true;
                Assert.AreEqual(repository, repo);
                Assert.AreEqual("file", filePath);
            };
            
            var result = _restoreService.RestoreFile(repository, "file");
            
            Assert.IsTrue(wasRaised);
            Assert.IsTrue(result);
        }
        
        [Test]
        public void TryDeleteFile_IfNotConfirmed_ReturnsFalse()
        {
            var repository = Substitute.For<IRepository>();
            _dialogService.Confirm(Arg.Any<string>()).Returns(false);

            var result = _restoreService.TryDeleteFile(repository, "test");

            Assert.IsFalse(result);
        }

        [Test]
        public void TryDeleteFile_IfConfirmed_DeletesFile()
        {
            var repository = TestUtilities.CreateMockRepository("path");
            
            _dialogService.Confirm(Arg.Any<string>()).Returns(true);
            
            var result = _restoreService.TryDeleteFile(repository, "test");
            
            _fileService.Received(1).Delete(Path.Combine("path", "test"));
            Assert.IsTrue(result);
        }

        [Test]
        public void TryDeleteFile_IfConfirmed_LogsMessages()
        {
            var repository = TestUtilities.CreateMockRepository("path");
            _dialogService.Confirm(Arg.Any<string>()).Returns(true);
            
            _restoreService.TryDeleteFile(repository, "test");

            var filePath = Path.Combine("path", "test");
            _logService.Received(1).LogMessage($"Deleting file {filePath}...");
            _logService.Received(1).LogMessage("File deleted.");
        }

        [Test]
        public void TryDeleteFile_IfFailed_LogsException()
        {
            var repository = TestUtilities.CreateMockRepository("path");
            _dialogService.Confirm(Arg.Any<string>()).Returns(true);
            
            var exception = new Exception("test");
            _fileService.When(x => x.Delete(Arg.Any<string>()))
                .Do(_ => throw exception);

            var result = _restoreService.TryDeleteFile(repository, "test");
            
            var filePath = Path.Combine("path", "test");
            Assert.IsFalse(result);
            _dialogService.Received(1).Error($"Failed to delete {filePath}.");
            _logService.Received(1).LogException(exception);
        }
        
        [Test]
        public void TryRestoreFile_IfNotConfirmed_ReturnsFalse()
        {
            var repository = Substitute.For<IRepository>();
            _dialogService.Confirm(Arg.Any<string>()).Returns(false);

            var result = _restoreService.TryRestoreFile(repository, "test");

            Assert.IsFalse(result);
        }

        [Test]
        public void TryRestoreFile_IfConfirmed_DeletesFile()
        {
            var repository = TestUtilities.CreateMockRepository("path");
            
            _dialogService.Confirm(Arg.Any<string>()).Returns(true);
            
            var result = _restoreService.TryRestoreFile(repository, "test");
            
            _checkoutService.Received(1).ForceCheckoutPaths(repository, Arg.Do<string[]>(ary =>
            {
                Assert.Equals(ary[0], "test");
            }));
            Assert.IsTrue(result);
        }

        [Test]
        public void TryRestoreFile_IfConfirmed_LogsMessages()
        {
            var repository = TestUtilities.CreateMockRepository("path");
            _dialogService.Confirm(Arg.Any<string>()).Returns(true);
            
            _restoreService.TryRestoreFile(repository, "test");

            var filePath = Path.Combine("path", "test");
            _logService.Received(1).LogMessage($"Restoring file {filePath}...");
            _logService.Received(1).LogMessage("File restored.");
        }

        [Test]
        public void TryRestoreFile_IfFailed_LogsException()
        {
            var repository = TestUtilities.CreateMockRepository("path");
            _dialogService.Confirm(Arg.Any<string>()).Returns(true);
            
            var exception = new Exception("test");
            _checkoutService.When(x => x.ForceCheckoutPaths(repository, Arg.Any<string[]>()))
                .Do(_ => throw exception);

            var result = _restoreService.TryRestoreFile(repository, "test");
            
            var filePath = Path.Combine("path", "test");
            Assert.IsFalse(result);
            _dialogService.Received(1).Error($"Failed to restore {filePath}.");
            _logService.Received(1).LogException(exception);
        }
    }
}
