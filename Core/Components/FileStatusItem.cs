using LibGit2Sharp;
using UIComponents.Core;
using UnityEngine.UIElements;
using UnityGit.GUI.Services;

namespace UnityGit.GUI.Components
{
    [Layout("FileStatusItem/FileStatusItem.uxml")]
    [Stylesheet("FileStatusItem/FileStatusItem.style.uss")]
    [Dependency(typeof(ICommitService), provide: typeof(CommitService))]
    [Dependency(typeof(IRestoreService), provide: typeof(RestoreService))]
    [Dependency(typeof(IDiffService), provide: typeof(DiffService))]
    public class FileStatusItem : UIComponent
    {
        private readonly IRepository _repository;
        private StatusEntry _statusEntry;
        private bool _ignored;
        
        private readonly Label _stateLabel;
        private readonly Label _filenameLabel;
        private readonly Toggle _selectionToggle;
        
        private readonly ICommitService _commitService;
        private readonly IRestoreService _restoreService;
        private readonly IDiffService _diffService;

        public FileStatusItem(IRepository repository)
        {
            _repository = repository;
            _commitService = Provide<ICommitService>();
            _restoreService = Provide<IRestoreService>();
            _diffService = Provide<IDiffService>();
            _commitService.FileSelectionChanged += OnFileSelectionChanged;
            _stateLabel = this.Q<Label>("changed-file-state");
            _filenameLabel = this.Q<Label>("changed-file-filename");
            _selectionToggle = this.Q<Toggle>("changed-file-toggle");
            
            this.AddManipulator(new ContextualMenuManipulator(BuildContextMenu));
        }

        ~FileStatusItem()
        {
            _commitService.FileSelectionChanged -= OnFileSelectionChanged;
            _selectionToggle.UnregisterValueChangedCallback(OnToggleClick);
        }

        private void BuildContextMenu(ContextualMenuPopulateEvent evt)
        {
            if (_statusEntry == null)
                return;
            
            evt.menu.AppendAction("Restore", (action) =>
            {
                _restoreService.RestoreFile(_repository, _statusEntry.FilePath);
            });

            if (_statusEntry.State == FileStatus.ModifiedInIndex || _statusEntry.State == FileStatus.ModifiedInWorkdir)
            {
                evt.menu.AppendAction("Diff", (action) =>
                {
                    _diffService.DiffFile(_repository, _statusEntry.FilePath);
                });
            }
        }

        public void SetStatusEntry(StatusEntry statusEntry)
        {
            _statusEntry = statusEntry;
            _ignored = _statusEntry.State.HasFlag(FileStatus.Ignored);

            _filenameLabel.text = _statusEntry.FilePath;

            var isSelected = _commitService.IsFileSelected(_repository, _statusEntry.FilePath);
            
            _selectionToggle.SetValueWithoutNotify(isSelected);

            var stateLabelOptions = GetStateLabelOptions();

            _stateLabel.text = stateLabelOptions.Text;
            _stateLabel.AddToClassList(stateLabelOptions.USSClass);

            _selectionToggle.UnregisterValueChangedCallback(OnToggleClick);
            _selectionToggle.RegisterValueChangedCallback(OnToggleClick);
        }

        private void OnToggleClick(ChangeEvent<bool> evt)
        {
            if (evt.newValue)
                _commitService.SelectFile(_repository, _statusEntry.FilePath);
            else
                _commitService.DeselectFile(_repository, _statusEntry.FilePath);
        }

        private void OnFileSelectionChanged(IRepository repository, string filePath, bool selected)
        {
            if (_repository.Info.Path == repository.Info.Path && _statusEntry.FilePath == filePath)
                _selectionToggle.SetValueWithoutNotify(selected);
        }

        private readonly struct StateLabelOptions
        {
            public readonly string USSClass;
            public readonly string Text;

            public StateLabelOptions(string className, string text)
            {
                USSClass = className;
                Text = text;
            }
        }

        private static readonly StateLabelOptions AddedStateOptions =
            new StateLabelOptions("state-added", "A");

        private static readonly StateLabelOptions ModifiedStateOptions =
            new StateLabelOptions("state-modified", "M");

        private static readonly StateLabelOptions RemovedStateOptions =
            new StateLabelOptions("state-removed", "D");

        private static readonly StateLabelOptions RenamedStateOptions =
            new StateLabelOptions("state-renamed", "R");

        private static readonly StateLabelOptions IgnoredStateOptions =
            new StateLabelOptions("state-ignored", "I");

        private static readonly StateLabelOptions UnknownStateOptions =
            new StateLabelOptions("state-unknown", "?");

        private StateLabelOptions GetStateLabelOptions()
        {
            var state = _statusEntry.State;

            if (_ignored)
                return IgnoredStateOptions;

            if (state.HasFlag(FileStatus.NewInIndex) || state.HasFlag(FileStatus.NewInWorkdir))
                return AddedStateOptions;

            if (state.HasFlag(FileStatus.RenamedInIndex) || state.HasFlag(FileStatus.RenamedInWorkdir))
                return RenamedStateOptions;

            if (state.HasFlag(FileStatus.ModifiedInIndex) || state.HasFlag(FileStatus.ModifiedInWorkdir))
                return ModifiedStateOptions;

            if (state.HasFlag(FileStatus.DeletedFromIndex) || state.HasFlag(FileStatus.DeletedFromWorkdir))
                return RemovedStateOptions;

            return UnknownStateOptions;
        }
    }
}