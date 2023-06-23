using LibGit2Sharp;
using NSubstitute;
using NUnit.Framework;
using UnityGit.Core.Services;

namespace UnityGit.Tests.Core.Services
{
    [TestFixture]
    public class BranchServiceTests
    {
        private BranchService _branchService;
        private Branch _branch;

        [SetUp]
        public void Setup()
        {
            _branchService = new BranchService();
            _branch = Substitute.For<Branch>();
        }

        [Test]
        public void GetBranchName_ReturnsBranchFriendlyName()
        {
            const string expectedBranchName = "master";
            _branch.FriendlyName.Returns(expectedBranchName);
            
            var result = _branchService.GetBranchName(_branch);
            
            Assert.AreEqual(expectedBranchName, result);
        }

        [Test]
        public void IsBehindRemote_ReturnsTrue_WhenBranchIsBehindRemote()
        {
            _branch.TrackingDetails.BehindBy.Returns(1);
            
            var result = _branchService.IsBehindRemote(_branch);
            
            Assert.IsTrue(result);
        }

        [Test]
        public void IsBehindRemote_ReturnsFalse_WhenBranchIsNotBehindRemote()
        {
            _branch.TrackingDetails.BehindBy.Returns(0);
            
            var result = _branchService.IsBehindRemote(_branch);
            
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsBehindRemote_ReturnsFalse_WhenBranchDoesNotExist()
        {
            _branch.TrackingDetails.BehindBy.Returns((int?)null);
            
            var result = _branchService.IsBehindRemote(_branch);
            
            Assert.IsFalse(result);
        }

        [Test]
        public void IsAheadOfRemote_ReturnsTrue_WhenBranchIsAheadOfRemote()
        {
            _branch.TrackingDetails.AheadBy.Returns(1);
            
            var result = _branchService.IsAheadOfRemote(_branch);
            
            Assert.IsTrue(result);
        }

        [Test]
        public void IsAheadOfRemote_ReturnsFalse_WhenBranchIsNotAheadOfRemote()
        {
            _branch.TrackingDetails.AheadBy.Returns(0);
            
            var result = _branchService.IsAheadOfRemote(_branch);
            
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsAheadOfRemote_ReturnsFalse_WhenBranchDoesNotExist()
        {
            _branch.TrackingDetails.AheadBy.Returns((int?)null);
            
            var result = _branchService.IsAheadOfRemote(_branch);
            
            Assert.IsFalse(result);
        }
    }
}
