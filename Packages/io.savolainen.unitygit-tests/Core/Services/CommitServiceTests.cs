using System;
using LibGit2Sharp;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using UnityGit.Core.Services;

namespace UnityGit.Tests.Core.Services
{
    [TestFixture]
    public class CommitServiceTests
    {
        private CommitService _commitService;
        private IRepository _repository;
        private ILogService _logService;
        private IProgressService _progressService;
        private ICommandsService _commandsService;
        
        private static readonly Signature CommitSignature =
            new Signature("Name", "email", DateTimeOffset.Now);

        [SetUp]
        public void Setup()
        {
            _logService = Substitute.For<ILogService>();
            _progressService = Substitute.For<IProgressService>();
            _progressService.Start(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<ProgressOptions>()
                ).Returns(0);
            _commandsService = Substitute.For<ICommandsService>();
            _repository = Substitute.For<IRepository>();
            _commandsService.Commit(
                Arg.Any<IRepository>(),
                Arg.Any<string>(),
                Arg.Any<Signature>()
                ).Returns(Substitute.For<Commit>());

            _commitService = new ServiceTestBed<CommitService>()
                .WithSingleton(_logService)
                .WithSingleton(_progressService)
                .WithSingleton(_commandsService)
                .Instantiate();
        }
        
        [Test]
        public void SelectFile_AddsFileToCommittedFilesDictionaryAndInvokesFileSelectionChangedEvent()
        {
            var wasRaised = false;
            
            _commitService.FileSelectionChanged += (repository, filePath, isSelected) =>
            {
                wasRaised = true;
                Assert.AreEqual(_repository, repository);
                Assert.AreEqual("path/to/file", filePath);
                Assert.IsTrue(isSelected);
            };
            
            _commitService.SelectFile(_repository, "path/to/file");

            Assert.IsTrue(_commitService.IsFileSelected(_repository, "path/to/file"));
            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void DeselectFile_RemovesFileFromCommittedFilesDictionaryAndInvokesFileSelectionChangedEvent()
        {
            var wasRaised = false;

            _commitService.SelectFile(_repository, "path/to/file");
            
            _commitService.FileSelectionChanged += (repository, filePath, isSelected) =>
            {
                wasRaised = true;
                Assert.AreEqual(_repository, repository);
                Assert.AreEqual("path/to/file", filePath);
                Assert.IsFalse(isSelected);
            };

            _commitService.DeselectFile(_repository, "path/to/file");

            Assert.IsFalse(_commitService.IsFileSelected(_repository, "path/to/file"));
            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void DeselectFile_DoesNotRemoveFileIfRepositoryDoesNotExistInCommittedFilesDictionary()
        {
            var wasRaised = false;
            
            _commitService.FileSelectionChanged += (_, _, _) =>
            {
                wasRaised = true;
            };
            
            _commitService.DeselectFile(_repository, "path/to/file");

            Assert.IsFalse(_commitService.IsFileSelected(_repository, "path/to/file"));
            Assert.IsFalse(wasRaised);
        }

        [Test]
        public void CommitSelected_Creates_Progress_Indicator()
        {
            _commitService.SelectFile(_repository, "path1");
            _commitService.SelectFile(_repository, "path2");
            
            _commitService.CommitSelected("Commit message", CommitSignature);

            _progressService.Received(1).Start("Committing 2 files", "Creating commit to 1 repositories",
                Arg.Do<ProgressOptions>(options =>
                {
                    Assert.IsTrue(options.Synchronous);
                    Assert.IsFalse(options.Sticky);
                }));
            _progressService.Received(1).FinishWithSuccess(0);
        }
        
        [Test]
        public void CommitSelected_CallsStageAndFileSelectionChangedForEachSelectedFileInRepositories()
        {
            _commitService.SelectFile(_repository, "path1");
            _commitService.SelectFile(_repository, "path2");
            
            var raisedCount = 0;
            
            _commitService.FileSelectionChanged += (repository, filePath, isSelected) =>
            {
                raisedCount++;
                Assert.AreEqual(_repository, repository);
                Assert.IsFalse(isSelected);

                if (filePath != "path1" && filePath != "path2")
                    Assert.Fail("Unexpected file path: " + filePath);
            };
            
            _commitService.CommitSelected("Commit message", CommitSignature);

            Assert.That(raisedCount, Is.EqualTo(2));
        }

        [Test]
        public void CommitSelected_Stages_EachSelectedFile()
        {
            _commitService.SelectFile(_repository, "path1");
            _commitService.SelectFile(_repository, "path2");

            _commitService.CommitSelected("Commit message", CommitSignature);
            
            _commandsService.Received(1).Stage(_repository, "path1");
            _commandsService.Received(1).Stage(_repository, "path2");
        }

        [Test]
        public void CommitSelected_ClearsCommittedFilesDictionary()
        {
            _commitService.SelectFile(_repository, "path1");
            _commitService.SelectFile(_repository, "path2");
            
            _commitService.CommitSelected("Commit message", CommitSignature);
            
            Assert.IsFalse(_commitService.IsFileSelected(_repository, "path1"));
            Assert.IsFalse(_commitService.IsFileSelected(_repository, "path2"));
        }

        [Test]
        public void CommitSelected_Creates_A_Commit()
        {
            _commitService.SelectFile(_repository, "path1");
            _commitService.SelectFile(_repository, "path2");
            
            _commitService.CommitSelected("Commit message", CommitSignature);

            _commandsService.Received(1).Commit(_repository, "Commit message", CommitSignature);
        }

        [Test]
        public void CommitSelected_Invokes_CommitCreated_Event()
        {
            var wasRaised = false;
            _commitService.CommitCreated += _ => wasRaised = true;
            
            _commitService.SelectFile(_repository, "path1");
            
            _commitService.CommitSelected("Commit message", CommitSignature);
            
            Assert.IsTrue(wasRaised);
        }
        
        [Test]
        public void Reports_Error_When_Committing_Empty_Commit()
        {
            _commandsService.Commit(
                Arg.Any<IRepository>(),
                Arg.Any<string>(),
                Arg.Any<Signature>()
            ).Throws(_ => new EmptyCommitException());
            
            _commitService.SelectFile(_repository, "path1");
            _commitService.CommitSelected("Commit message", CommitSignature);

            _progressService.Received(1).FinishWithError(0, "Can not create empty commit");
            _logService.Received(1).LogError("Can not create empty commit.");
        }
        
        [Test]
        public void Reports_Exceptions()
        {
            var exception = new Exception("Test exception");
            
            _commandsService.Commit(
                Arg.Any<IRepository>(),
                Arg.Any<string>(),
                Arg.Any<Signature>()
            ).Throws(_ => exception);
            
            _commitService.SelectFile(_repository, "path1");
            _commitService.CommitSelected("Commit message", CommitSignature);

            _progressService.Received(1).FinishWithError(0, "Commit failed with message 'Test exception'");
            _logService.Received(1).LogException(exception);
        }
        
        [Test]
        public void GetSelectedCount_ReturnsCorrectCount_WhenFilesAreSelected()
        {
            _commitService.SelectFile(_repository, "path1");
            _commitService.SelectFile(_repository, "path2");
            _commitService.SelectFile(_repository, "path3");
    
            var selectedCount = _commitService.GetSelectedCount();
    
            Assert.AreEqual(3, selectedCount);
        }

        [Test]
        public void GetSelectedCount_ReturnsZero_WhenNoFilesAreSelected()
        {
            var selectedCount = _commitService.GetSelectedCount();
    
            Assert.AreEqual(0, selectedCount);
        }

        [Test]
        public void GetSelectedCount_ReturnsZero_WhenFilesAreSelectedThenDeselected()
        {
            _commitService.SelectFile(_repository, "path1");
            _commitService.SelectFile(_repository, "path2");
            _commitService.DeselectFile(_repository, "path1");
            _commitService.DeselectFile(_repository, "path2");
    
            var selectedCount = _commitService.GetSelectedCount();
    
            Assert.AreEqual(0, selectedCount);
        }
    }
}
