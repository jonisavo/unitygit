using LibGit2Sharp;

namespace UnityGit.GUI.Services
{
    public interface IDiffService
    {
        public void DiffFile(IRepository repository, string filePath);
    }
}