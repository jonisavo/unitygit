using System;
using System.Collections.Generic;
using LibGit2Sharp;
using UnityEditor;
using UnityEngine;

namespace UnityGit.GUI.Services
{
    public class CommitService : ICommitService
    {
        public event ICommitService.CommitCreatedDelegate CommitCreated;

        public event ICommitService.FileSelectionChangedDelegate FileSelectionChanged;

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

        public void LogSelectedFiles()
        {
            foreach (var files in _committedFilesDictionary.Values)
                foreach (var file in files)
                    Debug.Log(file);
            
            Debug.Log(GetSelectedCount());
        }

        private void CommitToRepository(IRepository repository, string message, Signature commitSignature)
        {
            var selectedCount = GetSelectedCount();
            
            foreach (var filePath in _committedFilesDictionary[repository])
            {
                repository.Index.Add(filePath);
                FileSelectionChanged?.Invoke(repository, filePath, false);
            }
            
            _committedFilesDictionary[repository].Clear();

            repository.Index.Write();

            var progressId = Progress.Start(
                $"Committing {selectedCount} files",
                $"Creating commit to {_committedFilesDictionary.Keys.Count} repositories",
                Progress.Options.Indefinite | Progress.Options.Synchronous);
            
            try
            {
                var commit = repository.Commit(
                    message, commitSignature, commitSignature);

                CommitCreated?.Invoke(commit);
                
                Progress.Report(progressId, 1, 1);
                Progress.Finish(progressId, Progress.Status.Succeeded);
            }
            catch (EmptyCommitException)
            {
                Progress.SetDescription(progressId, "Can not create empty commit");
                Progress.Finish(progressId, Progress.Status.Failed);
            }
            catch (Exception exception)
            {
               Progress.SetDescription(progressId, $"Commit failed with message {exception.Message}");
               Progress.Finish(progressId, Progress.Status.Failed);
            }
        }
    }
}