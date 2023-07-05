using System.IO;
using System.Text;
using LibGit2Sharp;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityGit.Core.Services;

namespace UnityGit.Tests.Core.Services
{
    [TestFixture]
    public class DiffServiceTests
    {
        private const string FilePath = "test.txt";
        
        private DiffService _diffService;
        private IRepository _repository;
        private DiffService.InvokeDiffToolDelegate _invokeDiffDelegate;
        private TreeEntry _treeEntry;

        [SetUp]
        public void Setup()
        {
            _repository = Substitute.For<IRepository>();

            var head = Substitute.For<Branch>();
            _repository.Head.Returns(head);
            var tip = Substitute.For<Commit>();
            head.Tip.Returns(tip);
            _treeEntry = Substitute.For<TreeEntry>();
            tip[FilePath].Returns(_treeEntry);

            _invokeDiffDelegate = Substitute.For<DiffService.InvokeDiffToolDelegate>();
            _diffService = new DiffService(_invokeDiffDelegate);
        }

        [Test]
        public void DiffFile_WhenBlobIsNull_ShouldNotInvokeDiffTool()
        {
            _treeEntry.Target.Returns((Blob) null);
            
            _diffService.DiffFile(_repository, FilePath);
            
            _invokeDiffDelegate.DidNotReceive().Invoke(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>()
            );
        }

        [Test]
        public void DiffFile_WhenBlobExists_ShouldInvokeDiffTool()
        {
            var repositoryInfo = Substitute.For<RepositoryInformation>();
            repositoryInfo.WorkingDirectory.Returns("/repo");
            _repository.Info.Returns(repositoryInfo);
            
            var blob = Substitute.For<Blob>();
            _treeEntry.Target.Returns(blob);
            
            var contentStream = new MemoryStream(Encoding.UTF8.GetBytes("Test Content"));
            blob.GetContentStream().Returns(contentStream);
            
            _diffService.DiffFile(_repository, FilePath);
            
            var headFilePath = Path.Combine(Application.temporaryCachePath, "HEAD");
            var currentFilePath = Path.Combine("/repo", FilePath);

            _invokeDiffDelegate.Received(1).Invoke(
                "HEAD",
                headFilePath,
                "Current",
                currentFilePath,
                null,
                null
            );
        }

        [Test]
        public void DiffFile_ShouldWriteBlobContentToFile()
        {
            var blob = Substitute.For<Blob>();
            _treeEntry.Target.Returns(blob);
            
            var contentStream = new MemoryStream(Encoding.UTF8.GetBytes("Test Content"));
            blob.GetContentStream().Returns(contentStream);

            _diffService.DiffFile(_repository, FilePath);
            
            var headFilePath = Path.Combine(Application.temporaryCachePath, "HEAD");
            Assert.IsTrue(File.Exists(headFilePath));
            var fileContent = File.ReadAllText(headFilePath);
            Assert.AreEqual("Test Content", fileContent);
        }
    }
}
