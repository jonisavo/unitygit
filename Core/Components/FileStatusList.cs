using System.Collections.Generic;
using LibGit2Sharp;
using UIComponents.Core;
using UnityEngine.UIElements;
using UnityGit.GUI.Services;

namespace UnityGit.GUI.Components
{
    [Layout("FileStatusList/FileStatusList.uxml")]
    [Stylesheet("FileStatusList/FileStatusList.style.uss")]
    [InjectDependency(typeof(ICommitService), provider: typeof(CommitService))]
    public class FileStatusList : UIList<StatusEntry, FileStatusItem>
    {
        private readonly IRepository _repository;
        private readonly ICommitService _commitService;
        private readonly string _header;
        private readonly Foldout _foldout;
        private readonly Button _selectAllButton;
        private readonly Button _deselectAllButton;

        private readonly List<StatusEntry> _statusEntries;

        public FileStatusList(IRepository repository, List<StatusEntry> statusEntries, string header) : base(statusEntries)
        {
            _repository = repository;
            _commitService = Provide<ICommitService>();
            _commitService.FileSelectionChanged += OnFileSelectionChanged;
            _header = header;
            _selectAllButton = this.Q<Button>("file-status-list-select-all-button");
            _selectAllButton.clicked += SelectAllFiles;
            _deselectAllButton = this.Q<Button>("file-status-list-deselect-all-button");
            _deselectAllButton.clicked += DeselectAllFiles;
            _statusEntries = statusEntries;

            var listView = this.Q<ListView>("file-status-list-listview");
            SetUpListView(listView, _statusEntries, 32);
            
            _foldout = this.Q<Foldout>("file-status-list-foldout");
            
            RefreshHeader();

            if (_statusEntries.Count == 0)
                SetFoldoutEnabled(false);
        }

        private void RefreshHeader()
        {
            var selectedFileCount = CountSelectedFiles();
            
            if (selectedFileCount == 0)
                _foldout.text = $"{_header} ({_statusEntries.Count} files)";
            else
                _foldout.text = $"{_header} ({_statusEntries.Count} files, {selectedFileCount} selected)";
        }

        private void SetFoldoutEnabled(bool value)
        {
            _foldout.SetEnabled(value);
            _foldout.value = value;
        }

        private int CountSelectedFiles()
        {
            var count = 0;
            
            foreach (var entry in _statusEntries)
                if (_commitService.IsFileSelected(_repository, entry.FilePath))
                    count++;

            return count;
        }

        private void SelectAllFiles()
        {
            foreach (var entry in _statusEntries)
                _commitService.SelectFile(_repository, entry.FilePath);
        }
        
        private void DeselectAllFiles()
        {
            foreach (var entry in _statusEntries)
                _commitService.DeselectFile(_repository, entry.FilePath);
        }
        
        private void OnFileSelectionChanged(IRepository repository, string filePath, bool selected)
        {
            RefreshHeader();
        }

        public override FileStatusItem MakeItem()
        {
            return new FileStatusItem(_repository);
        }

        public override void BindItem(FileStatusItem item, int index)
        {
            item.SetStatusEntry(Items[index]);
        }
    }
}