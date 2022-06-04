using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface ICheckoutService
    {
        void ForceCheckoutPaths(IRepository repository, params string[] filePaths);
    }
}