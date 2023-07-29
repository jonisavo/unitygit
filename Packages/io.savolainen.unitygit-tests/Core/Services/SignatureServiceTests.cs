using System;
using LibGit2Sharp;
using NSubstitute;
using NUnit.Framework;
using UIComponents.Testing;
using UnityGit.Core.Services;

namespace UnityGit.Tests.Core.Services
{
    [TestFixture]
    public class SignatureServiceTests
    {
        private IStatusService _statusService;
        private SignatureService _signatureService;

        [SetUp]
        public void Setup()
        {
            _statusService = Substitute.For<IStatusService>();
            _signatureService = new TestBed<SignatureService>()
                .WithSingleton(_statusService)
                .Instantiate();
        }

        [Test]
        public void GetSignature_WhenNoRepositoriesExist_ReturnsNull()
        {
            _statusService.HasProjectRepository().Returns(false);
            _statusService.HasPackageRepositories().Returns(false);

            var signature = _signatureService.GetSignature();
            
            Assert.IsNull(signature);
        }
        
        [Test]
        public void GetSignature_WhenProjectRepositoryExists_ReturnsItsSignature()
        {
            _statusService.HasProjectRepository().Returns(true);
            
            var repository = Substitute.For<IRepository>();
            var configuration = Substitute.For<Configuration>();
            repository.Config.Returns(configuration);
            
            var signature = new Signature("name", "email", DateTimeOffset.Now);
            configuration.BuildSignature(Arg.Any<DateTimeOffset>()).Returns(signature);
            _statusService.ProjectRepository.Returns(repository);

            var result = _signatureService.GetSignature();

            Assert.AreSame(signature, result);
        }

        [Test]
        public void GetSignature_WhenPackageRepositoryExists_ReturnsItsSignature()
        {
            _statusService.HasProjectRepository().Returns(false);
            _statusService.HasPackageRepositories().Returns(true);
            
            var repository = Substitute.For<IRepository>();
            var configuration = Substitute.For<Configuration>();
            repository.Config.Returns(configuration);
            
            var signature = new Signature("name", "email", DateTimeOffset.Now);
            configuration.BuildSignature(Arg.Any<DateTimeOffset>()).Returns(signature);
            _statusService.PackageRepositories.Returns(new[] {repository});
            
            var result = _signatureService.GetSignature();
            
            Assert.AreSame(signature, result);
        }

        [Test]
        public void GetSignature_WhenNoRepositoryWithSignatureExists_ReturnsNull()
        {
            _statusService.HasProjectRepository().Returns(true);
            _statusService.HasPackageRepositories().Returns(true);
            
            var repository = Substitute.For<IRepository>();
            var configuration = Substitute.For<Configuration>();
            repository.Config.Returns(configuration);
            
            configuration.BuildSignature(Arg.Any<DateTimeOffset>()).Returns((Signature) null);
            
            _statusService.ProjectRepository.Returns(repository);
            _statusService.PackageRepositories.Returns(new[] {repository});
            
            var result = _signatureService.GetSignature();
            
            Assert.IsNull(result);
        }
    }
}
