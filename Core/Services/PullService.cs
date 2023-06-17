using LibGit2Sharp;
using UIComponents;

namespace UnityGit.Core.Services
{
    public interface IPullService
    {
        MergeResult Pull(Repository repository);
    }
    
    [Dependency(typeof(ICredentialsService), provide: typeof(CredentialsService))]
    [Dependency(typeof(ISignatureService), provide: typeof(SignatureService))]
    public partial class PullService : Service, IPullService
    {
        [Provide]
        private ICredentialsService _credentialsService;
        
        [Provide]
        private ISignatureService _signatureService;

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
