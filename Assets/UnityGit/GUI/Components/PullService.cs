using LibGit2Sharp;
using UIComponents;
using UnityGit.Core.Services;

namespace UnityGit.GUI.Components
{
    [Dependency(typeof(ICredentialsService), provide: typeof(CredentialsService))]
    [Dependency(typeof(ISignatureService), provide: typeof(SignatureService))]
    public class PullService : Service, IPullService
    {
        private readonly ICredentialsService _credentialsService;
        private readonly ISignatureService _signatureService;

        public PullService()
        {
            _credentialsService = Provide<ICredentialsService>();
            _signatureService = Provide<ISignatureService>();
        }
        
        public MergeResult Pull(Repository repository)
        {
            var fetchOptions = new FetchOptions();

            if (_credentialsService.HasCredentialsForRepository(repository))
            {
                var handler = _credentialsService.GetCredentialsHandlerForRepository(repository);
                fetchOptions.CredentialsProvider = handler;
            }

            var pullOptions = new PullOptions
            {
                FetchOptions = fetchOptions
            };

            return Commands.Pull(repository, _signatureService.GetSignature(), pullOptions);
        }
    }
}