using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface ICommitService
    {
        delegate void CommitCreatedDelegate(Commit commit);

        event CommitCreatedDelegate CommitCreated;
        
        delegate void FileSelectionChangedDelegate(IRepository repository, string filePath, bool selected);

        event FileSelectionChangedDelegate FileSelectionChanged;

        void SelectFile(IRepository repository, string filePath);

        void DeselectFile(IRepository repository, string filePath);

        bool IsFileSelected(IRepository repository, string filePath);

        int GetSelectedCount();

        void CommitSelected(string message, Signature commitSignature);
    }
}