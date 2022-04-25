using LibGit2Sharp;
using UIComponents.Core;
using UnityEngine.UIElements;

namespace UnityGit.GUI.Components
{
    [Layout("FileStatusItem/FileStatusItem.uxml", RelativeTo = AssetPaths.Components)]
    [Stylesheet("FileStatusItem/FileStatusItem.style.uss", RelativeTo = AssetPaths.Components)]
    public class FileStatusItem : UIComponent
    {
        private StatusEntry _statusEntry;
        private bool _ignored;
        
        private readonly Label _stateLabel;
        private readonly Label _filenameLabel;

        public FileStatusItem()
        {
            _stateLabel = this.Q<Label>("changed-file-state");
            _filenameLabel = this.Q<Label>("changed-file-filename");
        }

        public void SetStatusEntry(StatusEntry statusEntry)
        {
            _statusEntry = statusEntry;
            _ignored = _statusEntry.State.HasFlag(FileStatus.Ignored);
            _filenameLabel.text = _statusEntry.FilePath;

            var stateLabelOptions = GetStateLabelOptions();

            _stateLabel.text = stateLabelOptions.Text;
            _stateLabel.AddToClassList(stateLabelOptions.USSClass);
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