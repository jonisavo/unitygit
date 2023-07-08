using LibGit2Sharp;
using UnityEngine.TestTools;

namespace UnityGit.Core.Services
{
    public interface ICommandsService
    {
        Branch Checkout(IRepository repository, Branch branch);

        void Stage(IRepository repository, string filePath);

        Commit Commit(IRepository repository, string message, Signature author);

        MergeResult Pull(Repository repository, Signature signature, PullOptions pullOptions);
    }
    
    /// <summary>Acts as a wrapper for LibGit2Sharp.Commands.</summary>
    [ExcludeFromCoverage]
    public class CommandsService : ICommandsService
    {
        public Branch Checkout(IRepository repository, Branch branch)
        {
            return Commands.Checkout(repository, branch);
        }
        
        public void Stage(IRepository repository, string filePath)
        {
            Commands.Stage(repository, filePath);
        }
        
        public Commit Commit(IRepository repository, string message, Signature author)
        {
            return repository.Commit(message, author, author);
        }

        public MergeResult Pull(Repository repository, Signature signature, PullOptions pullOptions)
        {
            return Commands.Pull(repository, signature, pullOptions);
        }
    }
}
