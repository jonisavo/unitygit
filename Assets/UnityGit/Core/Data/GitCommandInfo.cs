using LibGit2Sharp;

namespace UnityGit.UnityGit.Core.Data
{
    public readonly struct GitCommandInfo
    {
        public readonly string Arguments;

        public readonly string ProgressName;
        
        public readonly string ProgressDescription;

        public readonly IRepository Repository;
        
        public GitCommandInfo(string arguments, string progressName, string progressDescription, IRepository repository)
        {
            Arguments = arguments;
            ProgressName = progressName;
            ProgressDescription = progressDescription;
            Repository = repository;
        }

        public override string ToString()
        {
            return $"{ProgressDescription} (git ${Arguments})";
        }
    }
}