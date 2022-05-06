using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface IRestoreService
    {
        public delegate void FileRestoredDelegate(IRepository repository, string filePath);

        public event FileRestoredDelegate FileRestored;
        
        public void RestoreFile(IRepository repository, string filePath);
    }
}