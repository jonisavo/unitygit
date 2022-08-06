using System;
using LibGit2Sharp;
using UIComponents;
using UIComponents.Experimental;

namespace UnityGit.Core.Services
{
    [Dependency(typeof(ILogService), provide: typeof(UnityGitLogService))]
    public class CheckoutService : Service, ICheckoutService
    {
        [Provide]
        private readonly ILogService _logService;
        
        public Branch CheckoutBranch(IRepository repository, Branch branch)
        {
            Branch checkedOutBranch = null;
            
            _logService.LogMessage($"Checking out branch {branch.FriendlyName}...");

            try
            {
                checkedOutBranch = Commands.Checkout(repository, branch);
                _logService.LogMessage($"Checked out branch {checkedOutBranch.FriendlyName}.");
            }
            catch (Exception exception)
            {
                _logService.LogException(exception);
            }

            return checkedOutBranch;
        }
        
        public void ForceCheckoutPaths(IRepository repository, params string[] filePaths)
        {
            var options = new CheckoutOptions
            {
                CheckoutModifiers = CheckoutModifiers.Force
            };

            repository.CheckoutPaths(
                repository.Head.FriendlyName,
                filePaths,
                options
            );
        }
    }
}