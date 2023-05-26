using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface IRestoreService
    {
        delegate void FileRestoredDelegate(IRepository repository, string filePath);

        event FileRestoredDelegate FileRestored;
        
        void RestoreFile(IRepository repository, string filePath);
    }
}