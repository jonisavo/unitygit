using LibGit2Sharp;

namespace UnityGit.GUI.Components
{
    public interface IPullService
    {
        MergeResult Pull(Repository repository);
    }
}