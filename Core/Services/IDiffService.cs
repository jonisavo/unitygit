using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface IDiffService
    {
        void DiffFile(IRepository repository, string filePath);
    }
}