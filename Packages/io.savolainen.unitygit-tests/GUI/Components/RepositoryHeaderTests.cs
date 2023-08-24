using LibGit2Sharp;
using NSubstitute;
using NUnit.Framework;
using UIComponents.Testing;
using UnityEditor;
using UnityEngine.UIElements;
using UnityGit.Core.Services;
using UnityGit.GUI.Components;
using UnityGit.Tests.Core;

namespace UnityGit.Tests.GUI.Components
{
    [TestFixture]
    public class RepositoryHeaderTests
    {
        private IRepositoryService _repositoryService;
        private RepositoryHeader _repositoryHeader;
        private TestWindow _testWindow;
        
        [SetUp]
        public void SetUp()
        {
            _repositoryService = Substitute.For<IRepositoryService>();

            _repositoryHeader = new TestBed<RepositoryHeader>()
                .WithSingleton(_repositoryService)
                .Instantiate();
            
            _testWindow = EditorWindow.GetWindow<TestWindow>();
            _testWindow.AddComponent(_repositoryHeader);
        }

        [TearDown]
        public void TearDown()
        {
            _testWindow.Close();
        }
        
        [Test]
        public void SetRepository_WhenRepositoryIsProjectRepository_SetsRepositoryNameToProjectRepositoryName()
        {
            const string projectRepositoryName = "Project Repository Name";
            
            _repositoryService.IsProjectRepository(Arg.Any<IRepository>()).Returns(true);
            _repositoryService.GetProjectRepositoryName().Returns(projectRepositoryName);
            
            var repository = TestUtilities.CreateMockRepository("/path/to/project/repository/.git");
            _repositoryHeader.SetRepository(repository);

            var repositoryNameLabel = _repositoryHeader.Q<Label>("repository-header-name-label");
            
            Assert.AreEqual(projectRepositoryName, repositoryNameLabel.text);
        }
        
        [Test]
        public void SetRepository_WhenRepositoryIsNotProjectRepository_SetsRepositoryNameToRepositoryName()
        {
            const string repositoryName = "Repository Name";
            
            _repositoryService.IsProjectRepository(Arg.Any<IRepository>()).Returns(false);
            _repositoryService.GetRepositoryName(Arg.Any<IRepository>()).Returns(repositoryName);
            
            var repository = TestUtilities.CreateMockRepository("/path/to/repository/.git");
            _repositoryHeader.SetRepository(repository);

            var repositoryNameLabel = _repositoryHeader.Q<Label>("repository-header-name-label");
            
            Assert.AreEqual(repositoryName, repositoryNameLabel.text);
        }
        
        [Test]
        public void SetRepository_SetsRepositoryPathToRepositoryPath()
        {
            var repository = TestUtilities.CreateMockRepository("/path/to/repository/.git");
            _repositoryHeader.SetRepository(repository);

            var repositoryPathLabel = _repositoryHeader.Q<Label>("repository-header-path-label");
            
            Assert.AreEqual("/path/to/repository", repositoryPathLabel.text);
        }
        
        [Test]
        public void RefreshButton_NotifiesWhenClicked()
        {
            var repository = TestUtilities.CreateMockRepository("/path/to/repository/.git");
            _repositoryHeader.SetRepository(repository);

            var refreshButton = _repositoryHeader.Q<Button>("repository-header-refresh-button");
            var refreshButtonClicked = false;
            _repositoryHeader.RefreshButtonClicked += () => refreshButtonClicked = true;

            using (var e = new NavigationSubmitEvent() { target = refreshButton })
                refreshButton.SendEvent(e);

            Assert.IsTrue(refreshButtonClicked);
        }
    }
}
