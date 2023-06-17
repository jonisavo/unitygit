using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface IBranchService
    {
        string GetBranchName(Branch branch);
        
        bool IsBehindRemote(Branch branch);
        
        bool IsAheadOfRemote(Branch branch);
    }
    
    public class BranchService : IBranchService
    {
        public string GetBranchName(Branch branch)
        {
            return branch.FriendlyName;
        }

        public bool IsBehindRemote(Branch branch)
        { 
            return branch.TrackingDetails.BehindBy.HasValue && 
                   branch.TrackingDetails.BehindBy.Value > 0;
        }

        public bool IsAheadOfRemote(Branch branch)
        {
            return branch.TrackingDetails.AheadBy.HasValue && 
                   branch.TrackingDetails.AheadBy.Value > 0;
        }
    }
}
