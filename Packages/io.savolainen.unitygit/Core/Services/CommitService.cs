using System;
using System.Collections.Generic;
using LibGit2Sharp;
using UIComponents;

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
    
    [Dependency(typeof(IProgressService), provide: typeof(ProgressService))]
    [Dependency(typeof(ILogService), provide: typeof(UnityGitLogService))]
    public sealed partial class CommitService : Service, ICommitService
    {
        public event ICommitService.CommitCreatedDelegate CommitCreated;

        public event ICommitService.FileSelectionChangedDelegate FileSelectionChanged;
        
        [Provide]
        private ILogService _logService;

        [Provide]
        private IProgressService _progressService;

        private readonly Dictionary<IRepository, HashSet<string>>
            _committedFilesDictionary = new Dictionary<IRepository, HashSet<string>>();
        
        public void SelectFile(IRepository repository, string filePath)
        {
            if (!_committedFilesDictionary.ContainsKey(repository))
                _committedFilesDictionary.Add(repository, new HashSet<string>());

            if (_committedFilesDictionary[repository].Add(filePath))
                FileSelectionChanged?.Invoke(repository, filePath, true);
        }

        public void DeselectFile(IRepository repository, string filePath)
        {
            if (!_committedFilesDictionary.ContainsKey(repository))
                return;

            if (_committedFilesDictionary[repository].Remove(filePath))
                FileSelectionChanged?.Invoke(repository, filePath, false);

            if (_committedFilesDictionary[repository].Count == 0)
                _committedFilesDictionary.Remove(repository);
        }

        public bool IsFileSelected(IRepository repository, string filePath)
        {
            if (!_committedFilesDictionary.ContainsKey(repository))
                return false;

            return _committedFilesDictionary[repository].Contains(filePath);
        }

        public void CommitSelected(string message, Signature commitSignature)
        {
            foreach (var repository in _committedFilesDictionary.Keys)
                CommitToRepository(repository, message, commitSignature);
        }

        public int GetSelectedCount()
        {
            var count = 0;

            foreach (var selectedFiles in _committedFilesDictionary.Values)
                count += selectedFiles.Count;
            
            return count;
        }

        private void CommitToRepository(IRepository repository, string message, Signature commitSignature)
        {
            var selectedCount = GetSelectedCount();

            if (_committedFilesDictionary.Count == 0)
                return;
            
            foreach (var filePath in _committedFilesDictionary[repository])
            {
                Commands.Stage(repository, filePath);
                FileSelectionChanged?.Invoke(repository, filePath, false);
            }
            
            _committedFilesDictionary[repository].Clear();
            
            var progressId = _progressService.Start(
                $"Committing {selectedCount} files",
                $"Creating commit to {_committedFilesDictionary.Keys.Count} repositories",
                new ProgressOptions { Synchronous = true }
            );
            
            _logService.LogMessage($"Committing {selectedCount} files...");

            try
            {
                var commit = repository.Commit(
                    message, commitSignature, commitSignature);

                CommitCreated?.Invoke(commit);
                
                _progressService.FinishWithSuccess(progressId);
                
                _logService.LogMessage("Commit created:");
                _logService.LogMessage(message);
            }
            catch (EmptyCommitException)
            {
                _progressService.FinishWithError(progressId, "Can not create empty commit");
                _logService.LogError("Can not create empty commit.");
            }
            catch (Exception exception)
            { 
                _progressService.FinishWithError(progressId, $"Commit failed with message {exception.Message}");
                _logService.LogException(exception);
            }
        }
    }
}
