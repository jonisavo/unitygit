using NUnit.Framework;
using LibGit2Sharp;
using UnityGit.Core.Services;
using NSubstitute;
using Credentials = UnityGit.Core.Data.Credentials;

namespace UnityGit.Tests.Core.Services
{
    [TestFixture]
    public class CredentialsServiceTests
    {
        private CredentialsService _credentialsService;
        private IRepository _repository;

        [SetUp]
        public void Setup()
        {
            _credentialsService = new CredentialsService();
            _repository = Substitute.For<IRepository>();
        }

        [Test]
        public void GetCredentialsForRepository_WithExistingCredentials_ReturnsValidCredentials()
        {
            var expectedCredentials = new Credentials { Username = "testuser", Password = "testpassword" };
            _credentialsService.SetCredentialsForRepository(_repository, expectedCredentials);
            
            var credentials = _credentialsService.GetCredentialsForRepository(_repository);
            
            Assert.AreEqual(expectedCredentials, credentials);
        }

        [Test]
        public void GetCredentialsForRepository_WithNoCredentials_ReturnsInvalidCredentials()
        {
            var credentials = _credentialsService.GetCredentialsForRepository(_repository);
            
            Assert.AreEqual(Credentials.InvalidCredentials, credentials);
        }

        [Test]
        public void HasCredentialsForRepository_WithExistingCredentials_ReturnsTrue()
        {
            _credentialsService.SetCredentialsForRepository(_repository, Credentials.InvalidCredentials);
            
            var hasCredentials = _credentialsService.HasCredentialsForRepository(_repository);
            
            Assert.IsTrue(hasCredentials);
        }

        [Test]
        public void HasCredentialsForRepository_WithNoCredentials_ReturnsFalse()
        {
            var hasCredentials = _credentialsService.HasCredentialsForRepository(_repository);
            
            Assert.IsFalse(hasCredentials);
        }

        [Test]
        public void SetCredentialsForRepository_SetsCredentials()
        {
            var credentials = new Credentials { Username = "testuser", Password = "testpassword" };
            
            _credentialsService.SetCredentialsForRepository(_repository, credentials);
            
            Assert.AreEqual(credentials, _credentialsService.GetCredentialsForRepository(_repository));
        }

        [Test]
        public void GetCredentialsHandlerForRepository_ReturnsCredentialsHandlerWithCorrectCredentials()
        {
            var credentials = new Credentials { Username = "testuser", Password = "testpassword" };
            _credentialsService.SetCredentialsForRepository(_repository, credentials);
            
            var credentialsHandler = _credentialsService.GetCredentialsHandlerForRepository(_repository);
            var libGit2SharpCredentials = credentialsHandler("url", "username", SupportedCredentialTypes.Default) as UsernamePasswordCredentials;
            
            Assert.AreEqual(credentials.Username, libGit2SharpCredentials.Username);
            Assert.AreEqual(credentials.Password, libGit2SharpCredentials.Password);
        }
    }
}
