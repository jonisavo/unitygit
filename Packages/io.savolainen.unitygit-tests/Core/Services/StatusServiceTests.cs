using System.Collections.Generic;
using LibGit2Sharp;
using NSubstitute;
using NUnit.Framework;
using UIComponents.Testing;
using UnityGit.Core.Services;

namespace UnityGit.Tests.Core.Services
{
    [TestFixture]
    public class StatusServiceTests
    {
        private IRepositoryService _repositoryService;
        private StatusService _statusService;

        [SetUp]
        public void SetUp()
        {
            _repositoryService = Substitute.For<IRepositoryService>();
            _repositoryService.GetPackageRepositories().Returns(new List<IRepository>());
            _repositoryService.IsValid(Arg.Any<IRepository>()).Returns(true);
            _statusService = new TestBed<StatusService>()
                .WithSingleton(_repositoryService)
                .Instantiate();
        }
        
        [Test]
        public void Clear_RemovesAndDisposesRepositories()
        {
            var projectRepository = Substitute.For<IRepository>();
            var packageRepository = Substitute.For<IRepository>();
            _statusService.ProjectRepository = projectRepository;
            _statusService._packageRepositories.Add(packageRepository);
            
            _statusService.Clear();
            
            Assert.IsNull(_statusService.ProjectRepository);
            Assert.AreEqual(0, _statusService._packageRepositories.Count);
            projectRepository.Received().Dispose();
            packageRepository.Received().Dispose();
        }
        
        [Test]
        public void HasProjectRepository_ReturnsWhetherProjectRepositoryExists()
        {
            _statusService.ProjectRepository = null;
            Assert.IsFalse(_statusService.HasProjectRepository());
            
            _statusService.ProjectRepository = Substitute.For<IRepository>();
            Assert.IsTrue(_statusService.HasProjectRepository());
        }
        
        [Test]
        public void HasPackageRepositories_ReturnsWhetherPackageRepositoriesExist()
        {
            _statusService._packageRepositories.Clear();
            Assert.IsFalse(_statusService.HasPackageRepositories());
            
            _statusService._packageRepositories.Add(Substitute.For<IRepository>());
            Assert.IsTrue(_statusService.HasPackageRepositories());
        }
        
        [Test]
        public void PopulateRepositories_PopulatesProjectRepository()
        {
            var projectRepository = Substitute.For<IRepository>();
            _repositoryService.GetProjectRepository().Returns(projectRepository);

            _statusService.PopulateRepositories();
            
            Assert.AreEqual(projectRepository, _statusService.ProjectRepository);
        }
        
        [Test]
        public void PopulateRepositories_PopulatesPackageRepositories()
        {
            var packageRepository = Substitute.For<IRepository>();
            _repositoryService.GetPackageRepositories().Returns(new List<IRepository>() {packageRepository});
            
            _statusService.PopulateRepositories();
            
            Assert.AreEqual(packageRepository, _statusService._packageRepositories[0]);
        }

        [Test]
        public void PopulateRepositories_DoesNotAddDuplicatePackageRepositories()
        {
            var packageRepository = Substitute.For<IRepository>();
            var newRepository = Substitute.For<IRepository>();
            _statusService._packageRepositories.Add(packageRepository);
            
            _repositoryService.AreRepositoriesEqual(packageRepository, newRepository).Returns(true);
            _repositoryService.GetPackageRepositories().Returns(new List<IRepository>() {newRepository});
            
            _statusService.PopulateRepositories();
            
            Assert.AreEqual(1, _statusService._packageRepositories.Count);
            Assert.AreSame(packageRepository, _statusService._packageRepositories[0]);
        }

        [Test]
        public void PopulateRepositories_RemovesInvalidPackageRepositories()
        {
            var packageRepository = Substitute.For<IRepository>();
            _repositoryService.IsValid(packageRepository).Returns(false);
            
            _statusService._packageRepositories.Add(packageRepository);
            _statusService._packageRepositories.Add(packageRepository);
            
            _statusService.PopulateRepositories();
            
            Assert.AreEqual(0, _statusService._packageRepositories.Count);
        }

        [Test]
        public void PopulateRepositories_DisposesCurrentProjectRepository()
        {
            var projectRepository = Substitute.For<IRepository>();
            
            _statusService.ProjectRepository = projectRepository;
            
            _statusService.PopulateRepositories();
            
            projectRepository.Received().Dispose();
        }
    }
}
