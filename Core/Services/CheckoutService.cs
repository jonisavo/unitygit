using System;
using LibGit2Sharp;
using UIComponents;

namespace UnityGit.Core.Services
{
    public interface ICheckoutService
    {
        Branch CheckoutBranch(IRepository repository, Branch branch);
        
        void ForceCheckoutPaths(IRepository repository, params string[] filePaths);
    }
    
    [Dependency(typeof(ILogService), provide: typeof(UnityGitLogService))]
    [Dependency(typeof(ICommandsService), provide: typeof(CommandsService))]
    public sealed partial class CheckoutService : Service, ICheckoutService
    {
        [Provide]
        private ILogService _logService;
        
        [Provide]
        private ICommandsService _commandsService;
        
        public Branch CheckoutBranch(IRepository repository, Branch branch)
        {
            Branch checkedOutBranch = null;
            
            _logService.LogMessage($"Checking out branch {branch.FriendlyName}...");

            try
            {
                checkedOutBranch = _commandsService.Checkout(repository, branch);
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
