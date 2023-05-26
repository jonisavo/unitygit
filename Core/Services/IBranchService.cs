using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface IBranchService
    {
        string GetBranchName(Branch branch);
        
        bool IsBehindRemote(Branch branch);
        
        bool IsAheadOfRemote(Branch branch);
    }
}