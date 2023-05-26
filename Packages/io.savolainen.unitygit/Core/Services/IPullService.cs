using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface IPullService
    {
        MergeResult Pull(Repository repository);
    }
}