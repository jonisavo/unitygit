using LibGit2Sharp;
using UIComponents;
using UnityEditor;
using UnityEngine.UIElements;
using UnityGit.Core.Services;
using UnityGit.Core.Utilities;

namespace UnityGit.GUI.Components
{
    [Layout("Components/FileStatusItem/FileStatusItem")]
    [Stylesheet("Components/FileStatusItem/FileStatusItem.style")]
    [Dependency(typeof(ICommitService), provide: typeof(CommitService))]
    [Dependency(typeof(IRestoreService), provide: typeof(RestoreService))]
    [Dependency(typeof(IDiffService), provide: typeof(DiffService))]
    public partial class FileStatusItem : UnityGitUIComponent
    {
        [Query("changed-file-state")]
        private Label _stateLabel;
        [Query("changed-file-filename")]
        private Label _filenameLabel;
        [Query("changed-file-toggle")]
        private Toggle _selectionToggle;
        
        [Provide]
        private ICommitService _commitService;
        [Provide]
        private IRestoreService _restoreService;
        [Provide]
        private IDiffService _diffService;
        
        private readonly IRepository _repository;
        private StatusEntry _statusEntry;
        private bool _ignored;

        public FileStatusItem(IRepository repository)
        {
            _repository = repository;
        }

        public override void OnInit()
        {
            _commitService.FileSelectionChanged += OnFileSelectionChanged;

            this.AddManipulator(new ContextualMenuManipulator(BuildContextMenu));
            
            UpdateStatusEntry();
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

            if (FileStatusUtilities.Exists(_statusEntry))
            {
                evt.menu.AppendAction("View", (action) =>
                {
                    EditorUtility.OpenWithDefaultApp(_statusEntry.FilePath);
                });
            }
            
            if (FileStatusUtilities.IsModified(_statusEntry))
            {
                evt.menu.AppendAction("Diff", (action) =>
                {
                    _diffService.DiffFile(_repository, _statusEntry.FilePath);
                });
            }

            string restoreName;

            if (FileStatusUtilities.IsNew(_statusEntry))
                restoreName = "Delete";
            else
                restoreName = "Restore";
            
            evt.menu.AppendAction(restoreName, (action) =>
            {
                _restoreService.RestoreFile(_repository, _statusEntry.FilePath);
            });
        }

        public void SetStatusEntry(StatusEntry statusEntry)
        {
            _statusEntry = statusEntry;
            _ignored = _statusEntry.State.HasFlag(FileStatus.Ignored);
            
            if (Initialized)
                UpdateStatusEntry();
        }

        private void UpdateStatusEntry()
        {
            _filenameLabel.text = _statusEntry.FilePath;

            var isSelected = _commitService.IsFileSelected(_repository, _statusEntry.FilePath);
            
            _selectionToggle.SetValueWithoutNotify(isSelected);

            ApplyCurrentState();

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

        private static readonly string[] StateClassNames = new[]
        {
            "state-added", "state-modified", "state-removed", "state-renamed", "state-ignored", "state-unknown"
        };

        private StateLabelOptions GetStateLabelOptions()
        {
            var state = _statusEntry.State;

            if (_ignored)
                return IgnoredStateOptions;

            if (FileStatusUtilities.IsNew(state))
                return AddedStateOptions;

            if (FileStatusUtilities.IsRenamed(state))
                return RenamedStateOptions;

            if (FileStatusUtilities.IsModified(state))
                return ModifiedStateOptions;

            if (FileStatusUtilities.IsDeleted(state))
                return RemovedStateOptions;

            return UnknownStateOptions;
        }
        
        private void ApplyCurrentState()
        {
            var stateLabelOptions = GetStateLabelOptions();

            _stateLabel.text = stateLabelOptions.Text;
            
            foreach (var className in StateClassNames)
                _stateLabel.RemoveFromClassList(className);

            _stateLabel.AddToClassList(stateLabelOptions.USSClass);
        }
    }
}
