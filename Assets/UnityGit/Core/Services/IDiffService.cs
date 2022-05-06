using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface IDiffService
    {
        public void DiffFile(IRepository repository, string filePath);
    }
}