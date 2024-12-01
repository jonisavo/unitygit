using LibGit2Sharp;
using UIComponents;
using UnityEditor;
using UnityEngine.UIElements;
using UnityGit.Core.Services;
using UnityGit.Core.Utilities;

namespace UnityGit.GUI.Components
{
    [Layout("Components/FileStatusItem/FileStatusItem.uxml")]
    [Stylesheet("Components/FileStatusItem/FileStatusItem.style.uss")]
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
            _selectionToggle.RegisterValueChangedCallback(OnToggleClick);

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

            if (FileStatusUtilities.Exists(_statusEntry))
            {
                evt.menu.AppendAction("View", _ =>
                {
                    EditorUtility.OpenWithDefaultApp(_statusEntry.FilePath);
                });
            }
            
            if (FileStatusUtilities.IsModified(_statusEntry))
            {
                evt.menu.AppendAction("Diff", _ =>
                {
                    _diffService.DiffFile(_repository, _statusEntry.FilePath);
                });
            }

            string restoreName;

            if (FileStatusUtilities.IsNew(_statusEntry))
                restoreName = "Delete";
            else
                restoreName = "Restore";
            
            evt.menu.AppendAction(restoreName, _ =>
            {
                _restoreService.RestoreFile(_repository, _statusEntry.FilePath);
            });
        }

        public void SetStatusEntry(StatusEntry statusEntry)
        {
            _statusEntry = statusEntry;
            _ignored = _statusEntry.State.HasFlag(FileStatus.Ignored);

            _filenameLabel.text = _statusEntry.FilePath;
            _filenameLabel.tooltip = _statusEntry.FilePath;

            var isSelected = _commitService.IsFileSelected(_repository, _statusEntry.FilePath);
            
            _selectionToggle.SetValueWithoutNotify(isSelected);

            ApplyCurrentState();
        }

        private void OnToggleClick(ChangeEvent<bool> evt)
        {
            if (_statusEntry == null)
                return;
            
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
            public readonly string Tooltip;

            public StateLabelOptions(string className, string text, string tooltip)
            {
                USSClass = className;
                Text = text;
                Tooltip = tooltip;
            }
        }

        private static readonly StateLabelOptions AddedStateOptions =
            new StateLabelOptions("state-added", "A", "Added");

        private static readonly StateLabelOptions ModifiedStateOptions =
            new StateLabelOptions("state-modified", "M", "Modified");

        private static readonly StateLabelOptions RemovedStateOptions =
            new StateLabelOptions("state-removed", "D", "Removed");

        private static readonly StateLabelOptions RenamedStateOptions =
            new StateLabelOptions("state-renamed", "R", "Renamed");

        private static readonly StateLabelOptions IgnoredStateOptions =
            new StateLabelOptions("state-ignored", "I", "Ignored");

        private static readonly StateLabelOptions UnknownStateOptions =
            new StateLabelOptions("state-unknown", "?", "Unknown");

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
            _stateLabel.tooltip = stateLabelOptions.Tooltip;
            
            foreach (var className in StateClassNames)
                _stateLabel.RemoveFromClassList(className);

            _stateLabel.AddToClassList(stateLabelOptions.USSClass);
        }
    }
}
