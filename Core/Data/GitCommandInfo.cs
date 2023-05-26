using LibGit2Sharp;

namespace UnityGit.Core.Data
{
    public readonly struct GitCommandInfo
    {
        public readonly string Arguments;

        public readonly string ProgressName;
        
        public readonly string ProgressDescription;

        public readonly IRepository Repository;

        public readonly int TimeoutMs;
        
        public GitCommandInfo(string arguments, string progressName, string progressDescription, IRepository repository, int timeoutMs = 0)
        {
            Arguments = arguments;
            ProgressName = progressName;
            ProgressDescription = progressDescription;
            Repository = repository;
            TimeoutMs = timeoutMs;
        }

        public override string ToString()
        {
            return $"{ProgressDescription} (git {Arguments})";
        }
    }
}
