using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface ICheckoutService
    {
        public void ForceCheckoutPaths(IRepository repository, params string[] filePaths);
    }
}