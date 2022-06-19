using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface IPushService
    {
        bool IsPushing { get; }
        
        void Push(IRepository repository, Branch branch);
    }
}