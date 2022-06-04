using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public class CheckoutService : ICheckoutService
    {
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