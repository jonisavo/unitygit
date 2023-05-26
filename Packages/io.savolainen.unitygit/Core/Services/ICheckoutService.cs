using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface ICheckoutService
    {
        Branch CheckoutBranch(IRepository repository, Branch branch);
        
        void ForceCheckoutPaths(IRepository repository, params string[] filePaths);
    }
}