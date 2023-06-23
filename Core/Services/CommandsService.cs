using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface ICommandsService
    {
        Branch Checkout(IRepository repository, Branch branch);
    }
    
    /// <summary>Acts as a wrapper for LibGit2Sharp.Commands.</summary>
    public class CommandsService : ICommandsService
    {
        public Branch Checkout(IRepository repository, Branch branch)
        {
            return Commands.Checkout(repository, branch);
        }
    }
}
