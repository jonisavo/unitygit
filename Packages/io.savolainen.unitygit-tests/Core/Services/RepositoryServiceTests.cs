using System.IO;
using LibGit2Sharp;
using NSubstitute;
using NUnit.Framework;
using UnityGit.Core.Services;

namespace UnityGit.Tests.Core.Services
{
    [TestFixture]
    public class RepositoryServiceTests
    {
        [Test]
        public void GetProjectRepository_ReturnsTheProjectRepository()
        {
            var repositoryService = new RepositoryService();
            
            var repository = repositoryService.GetProjectRepository();
            var currentDirectory = Directory.GetCurrentDirectory();
            
            Assert.NotNull(repository);
            Assert.That(repository.Info.WorkingDirectory, Contains.Substring(currentDirectory));
        }
        
        [Test]
        public void GetRepositoryOfDirectory_ReturnsNull_IfTheDirectoryDoesNotExist()
        {
            var repositoryService = new RepositoryService();
            
            var repository = repositoryService.GetRepository("invalid");
            
            Assert.Null(repository);
        }
        
        [Test]
        public void GetRepositoryOfDirectory_ReturnsNull_IfTheDirectoryIsNotARepository()
        {
            var repositoryService = new RepositoryService();
            
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
            var repository = repositoryService.GetRepository(directory);
            
            Assert.Null(repository);
        }

        [Test]
        public void IsProjectRepository_ReturnsWhetherTheRepositoryIsTheProjectRepository()
        {
            var repositoryService = new RepositoryService();
            
            var projectRepository = repositoryService.GetProjectRepository();
            var otherRepository = new Repository();
            
            Assert.True(repositoryService.IsProjectRepository(projectRepository));
            Assert.False(repositoryService.IsProjectRepository(otherRepository));
        }
        
        [Test]
        public void GetProjectRepositoryName_ReturnsTheProjectRepositoryName()
        {
            var repositoryService = new RepositoryService();

            Assert.That(repositoryService.GetProjectRepositoryName(), Is.EqualTo("unitygit"));
        }
        
        [Test]
        public void AreRepositoriesEqual_ReturnsWhetherTheRepositoriesAreEqual()
        {
            var repositoryService = new RepositoryService();
            
            var projectRepository = repositoryService.GetProjectRepository();
            var otherRepository = new Repository();
            
            Assert.True(repositoryService.AreRepositoriesEqual(projectRepository, projectRepository));
            Assert.False(repositoryService.AreRepositoriesEqual(projectRepository, otherRepository));
        }
        
        [Test]
        public void AreRepositoriesEqual_ReturnsFalse_IfEitherRepositoryIsNull()
        {
            var repositoryService = new RepositoryService();
            
            var projectRepository = repositoryService.GetProjectRepository();
            
            Assert.False(repositoryService.AreRepositoriesEqual(projectRepository, null));
            Assert.False(repositoryService.AreRepositoriesEqual(null, projectRepository));
            Assert.False(repositoryService.AreRepositoriesEqual(null, null));
        }
        
        [Test]
        public void GetRepositoryName_ReturnsAFriendlyNameForTheRepository()
        {
            var repositoryService = new RepositoryService();
            
            var repositoryOne = Substitute.For<IRepository>();
            var repositoryTwo = Substitute.For<IRepository>();
            
            var repositoryInfo = Substitute.For<RepositoryInformation>();
            var repositoryOnePath = Path.Combine("test", "dir", "io.savolainen.test", ".git");
            repositoryInfo.Path.Returns(repositoryOnePath);
            repositoryOne.Info.Returns(repositoryInfo);
            
            var repositoryInfoTwo = Substitute.For<RepositoryInformation>();
            var repositoryTwoPath = Path.Combine("test", "dir", "io.savolainen.test", ".git") + Path.DirectorySeparatorChar;
            repositoryInfoTwo.Path.Returns(repositoryTwoPath);
            repositoryTwo.Info.Returns(repositoryInfoTwo);
            
            Assert.That(repositoryService.GetRepositoryName(repositoryOne), Is.EqualTo("io.savolainen.test"));
            Assert.That(repositoryService.GetRepositoryName(repositoryTwo), Is.EqualTo("io.savolainen.test"));
        }
        
        [Test]
        public void IsValid_ReturnsWhetherTheRepositoryIsValid()
        {
            var repositoryService = new RepositoryService();
            
            var projectRepository = repositoryService.GetProjectRepository();
            var otherRepository = Substitute.For<IRepository>();
            var repositoryInfo = Substitute.For<RepositoryInformation>();
            repositoryInfo.Path.Returns("invalid");
            otherRepository.Info.Returns(repositoryInfo);
            
            Assert.True(repositoryService.IsValid(projectRepository));
            Assert.False(repositoryService.IsValid(otherRepository));
        }
    }
}
