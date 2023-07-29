using NUnit.Framework;
using NSubstitute;
using LibGit2Sharp;
using UnityGit.Core.Services;
using UIComponents.Testing;
using UnityGit.Core.Data;

namespace UnityGit.Tests.Core.Services
{
    [TestFixture]
    public class PushServiceTests
    {
        private IDialogService _dialogService;
        private IGitCommandService _gitCommandService;
        private PushService _pushService;

        [SetUp]
        public void SetUp()
        {
            _dialogService = Substitute.For<IDialogService>();
            _gitCommandService = Substitute.For<IGitCommandService>();
            _pushService = new TestBed<PushService>()
                .WithSingleton(_dialogService)
                .WithSingleton(_gitCommandService)
                .Instantiate();
        }

        [Test]
        public void Push_Pushes_WhenConfirmed()
        {
            var repository = Substitute.For<IRepository>();
            var branch = Substitute.For<Branch>();
            branch.RemoteName.Returns("origin");
            branch.FriendlyName.Returns("feature/stuff");
            _dialogService.Confirm(Arg.Any<string>()).Returns(true);

            _pushService.Push(repository, branch);

            _gitCommandService.Received(1).Run(Arg.Do<GitCommandInfo>((info) =>
            {
                Assert.That(info.Arguments, Is.EqualTo("push origin feature/stuff"));
                Assert.That(info.ProgressName, Is.EqualTo("Pushing..."));
                Assert.That(info.ProgressDescription, Is.EqualTo("Pushing feature/stuff to origin"));
                Assert.That(info.Repository, Is.SameAs(repository));
            }));
        }

        [Test]
        public void Push_DoesNotPush_WhenNotConfirmed()
        {
            var repository = Substitute.For<IRepository>();
            var branch = Substitute.For<Branch>();
            _dialogService.Confirm(Arg.Any<string>()).Returns(false);

            _pushService.Push(repository, branch);

            _gitCommandService.DidNotReceive().Run(Arg.Any<GitCommandInfo>());
        }
    }
}
