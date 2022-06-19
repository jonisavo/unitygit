using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public class BranchService : IBranchService
    {
        public string GetBranchName(Branch branch)
        {
            return branch.FriendlyName;
        }
    }
}