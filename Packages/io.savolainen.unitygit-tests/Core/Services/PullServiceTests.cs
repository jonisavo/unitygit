using NSubstitute;
using NUnit.Framework;
using UnityGit.Core.Services;
using LibGit2Sharp;
using System;
using UIComponents.Testing;
using LibGit2Sharp.Handlers;

namespace UnityGit.Tests.Core.Services
{
	[TestFixture]
	public class PullServiceTests
	{
		private ICredentialsService _credentialsService;
		private ISignatureService _signatureService;
		private ICommandsService _commandsService;
		private PullService _pullService;

		private static readonly Signature Signature =
            new Signature("Name", "example@example.org", DateTimeOffset.Now);

        [SetUp]
		public void SetUp()
		{
			_credentialsService = Substitute.For<ICredentialsService>();

			_signatureService = Substitute.For<ISignatureService>();
            _signatureService.GetSignature().Returns(Signature);

            _commandsService = Substitute.For<ICommandsService>();

			_pullService = new TestBed<PullService>()
				.WithSingleton(_credentialsService)
				.WithSingleton(_signatureService)
				.WithSingleton(_commandsService)
				.Instantiate();
		}

		[Test]
		public void Pull_CreatesAMergeResult()
		{
			var repository = new Repository();
			var mergeResult = Substitute.For<MergeResult>();
			_credentialsService.HasCredentialsForRepository(repository).Returns(false);
			_commandsService.Pull(repository, Signature, Arg.Any<PullOptions>()).Returns(mergeResult);

			var result = _pullService.Pull(repository);

			Assert.That(result, Is.SameAs(mergeResult));
			_commandsService.Received(1).Pull(repository, Signature, Arg.Any<PullOptions>());
		}

		[Test]
		public void Pull_ProvidesCredentials()
		{
            var repository = new Repository();
            _credentialsService.HasCredentialsForRepository(repository).Returns(true);
			CredentialsHandler handler = (_, __, ___) => { return null; };
			_credentialsService.GetCredentialsHandlerForRepository(repository).Returns(handler);

            _pullService.Pull(repository);

			_commandsService.Received(1).Pull(repository, Signature, Arg.Do<PullOptions>((options) => {
				Assert.That(options.FetchOptions.CredentialsProvider, Is.EqualTo(handler));
			}));
        }
	}
}
