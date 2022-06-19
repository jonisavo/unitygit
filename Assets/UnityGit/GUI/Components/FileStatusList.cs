using System.Collections.Generic;
using LibGit2Sharp;
using UIComponents;
using UIComponents.Experimental;
using UnityEngine.UIElements;
using UnityGit.Core.Services;

namespace UnityGit.GUI.Components
{
    [Layout("FileStatusList/FileStatusList")]
    [Stylesheet("FileStatusList/FileStatusList.style")]
    [Dependency(typeof(ICommitService), provide: typeof(CommitService))]
    public class FileStatusList : UIList<StatusEntry, FileStatusItem>
    {
        private readonly IRepository _repository;
        private readonly ICommitService _commitService;
        private readonly string _header;
        
        [Query("file-status-list-foldout")]
        private readonly Foldout _foldout;
        [Query("file-status-list-select-all-button")]
        private readonly Button _selectAllButton;
        [Query("file-status-list-deselect-all-button")]
        private readonly Button _deselectAllButton;

        public FileStatusList(IRepository repository, List<StatusEntry> statusEntries, string header) : base(statusEntries)
        {
            _repository = repository;
            _commitService = Provide<ICommitService>();
            _commitService.FileSelectionChanged += OnFileSelectionChanged;
            _header = header;
            
            _selectAllButton.clicked += SelectAllFiles;
            _deselectAllButton.clicked += DeselectAllFiles;

            var listView = this.Q<ListView>("file-status-list-listview");
            SetUpListView(listView, Items, 32);

            RefreshHeader();

            if (Items.Count == 0)
                SetFoldoutEnabled(false);
        }

        ~FileStatusList()
        {
            _commitService.FileSelectionChanged -= OnFileSelectionChanged;
            _selectAllButton.clicked -= SelectAllFiles;
            _deselectAllButton.clicked -= DeselectAllFiles;
        }

        private void RefreshHeader()
        {
            var selectedFileCount = CountSelectedFiles();
            
            if (selectedFileCount == 0)
                _foldout.text = $"{_header} ({Items.Count} files)";
            else
                _foldout.text = $"{_header} ({Items.Count} files, {selectedFileCount} selected)";
        }

        private void SetFoldoutEnabled(bool value)
        {
            _foldout.SetEnabled(value);
            _foldout.value = value;
        }

        private int CountSelectedFiles()
        {
            var count = 0;
            
            foreach (var entry in Items)
                if (_commitService.IsFileSelected(_repository, entry.FilePath))
                    count++;

            return count;
        }

        private void SelectAllFiles()
        {
            foreach (var entry in Items)
                _commitService.SelectFile(_repository, entry.FilePath);
        }
        
        private void DeselectAllFiles()
        {
            foreach (var entry in Items)
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