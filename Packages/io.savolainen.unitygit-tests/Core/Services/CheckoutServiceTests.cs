using System;
using LibGit2Sharp;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using UnityGit.Core.Services;

namespace UnityGit.Tests.Core.Services
{
    [TestFixture]
    public class CheckoutServiceTests
    {
        private ILogService _logService;
        private ICommandsService _commandsService;
        private CheckoutService _checkoutService;
        private IRepository _repository;
        private Branch _branch;

        [SetUp]
        public void SetUp()
        {
            _logService = Substitute.For<ILogService>();
            _commandsService = Substitute.For<ICommandsService>();
            _repository = Substitute.For<IRepository>();
            _branch = Substitute.For<Branch>();
            var tip = Substitute.For<Commit>();
            _branch.Tip.Returns(tip);
            _checkoutService = new ServiceTestBed<CheckoutService>()
                .WithSingleton(_logService)
                .WithSingleton(_commandsService)
                .Instantiate();
        }

        [Test]
        public void CheckoutBranch_LogsCheckoutMessage()
        {
            _branch.FriendlyName.Returns("feature/branch");

            _checkoutService.CheckoutBranch(_repository, _branch);

            _logService.Received(1).LogMessage("Checking out branch feature/branch...");
        }

        [Test]
        public void CheckoutBranch_ChecksOutBranch()
        {
            _commandsService.Checkout(_repository, _branch).Returns(_branch);

            _checkoutService.CheckoutBranch(_repository, _branch);

            _commandsService.Received(1).Checkout(_repository, _branch);
        }

        [Test]
        public void CheckoutBranch_LogsCheckedOutBranchMessage()
        {
            _branch.FriendlyName.Returns("feature/branch");

            _commandsService.Checkout(_repository, _branch).Returns(_branch);

            _checkoutService.CheckoutBranch(_repository, _branch);

            _logService.Received(1).LogMessage("Checked out branch feature/branch.");
        }

        [Test]
        public void CheckoutBranch_LogsExceptionIfCheckoutFails()
        {
            var exception = new Exception("Checkout failed");

            _commandsService.Checkout(_repository, _branch).Throws(exception);

            _checkoutService.CheckoutBranch(_repository, _branch);

            _logService.Received(1).LogException(exception);
        }

        [Test]
        public void ForceCheckoutPaths_CallsCheckoutPathsOnRepository()
        {
            var filePaths = new[] {"path1", "path2"};
            var branch = Substitute.For<Branch>();
            branch.FriendlyName.Returns("feature/branch");
            _repository.Head.Returns(branch);

            _checkoutService.ForceCheckoutPaths(_repository, filePaths);

            _repository
                .Received(1)
                .CheckoutPaths("feature/branch", filePaths, Arg.Do<CheckoutOptions>(options =>
                {
                    Assert.AreEqual(CheckoutModifiers.Force, options.CheckoutModifiers);
                }));
        }
    }
}
