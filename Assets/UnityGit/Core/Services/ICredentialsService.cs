using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Credentials = UnityGit.UnityGit.Core.Data.Credentials;

namespace UnityGit.Core.Services
{
    public interface ICredentialsService
    {
        Credentials GetCredentialsForRepository(IRepository repository);

        bool HasCredentialsForRepository(IRepository repository);

        void SetCredentialsForRepository(IRepository repository, Credentials credentials);
        
        CredentialsHandler GetCredentialsHandlerForRepository(IRepository repository);
    }
}