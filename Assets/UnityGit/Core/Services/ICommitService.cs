using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface ICommitService
    {
        public delegate void CommitCreatedDelegate(Commit commit);

        public event CommitCreatedDelegate CommitCreated;
        
        public delegate void FileSelectionChangedDelegate(IRepository repository, string filePath, bool selected);

        public event FileSelectionChangedDelegate FileSelectionChanged;

        public void SelectFile(IRepository repository, string filePath);

        public void DeselectFile(IRepository repository, string filePath);

        public bool IsFileSelected(IRepository repository, string filePath);

        public int GetSelectedCount();

        public void CommitSelected(string message, Signature commitSignature);
    }
}