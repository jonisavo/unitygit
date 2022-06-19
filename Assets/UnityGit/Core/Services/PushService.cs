using LibGit2Sharp;
using UIComponents;

namespace UnityGit.Core.Services
{
    [Dependency(typeof(IDialogService), provide: typeof(DialogService))]
    [Dependency(typeof(ICredentialsService), provide: typeof(CredentialsService))]
    public class PushService : Service, IPushService
    {
        private readonly IDialogService _dialogService;
        private readonly ICredentialsService _credentialsService;

        public PushService()
        {
            _dialogService = Provide<IDialogService>();
            _credentialsService = Provide<ICredentialsService>();
        }
        
        public void Push(IRepository repository, Branch branch)
        {
            if (_dialogService.Confirm($"Push branch {branch.FriendlyName} to remote {branch.RemoteName}?"))
                DoPush(repository, branch);
        }
        
        private void DoPush(IRepository repository, Branch branch)
        {
            var pushOptions = new PushOptions();

            if (_credentialsService.HasCredentialsForRepository(repository))
            {
                var handler = _credentialsService.GetCredentialsHandlerForRepository(repository);
                pushOptions.CredentialsProvider = handler;
            }
            
            repository.Network.Push(branch, pushOptions);
        }
    }
}