using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface IPushService
    {
        void Push(IRepository repository, Branch branch);
    }
}