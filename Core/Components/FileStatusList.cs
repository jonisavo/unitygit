using System.Collections.Generic;
using LibGit2Sharp;
using UIComponents.Core;
using UnityEngine.UIElements;

namespace UnityGit.GUI.Components
{
    [Layout("FileStatusList/FileStatusList.uxml", RelativeTo = AssetPaths.Components)]
    [Stylesheet("FileStatusList/FileStatusList.style.uss", RelativeTo = AssetPaths.Components)]
    public class FileStatusList : UIList<StatusEntry, FileStatusItem>
    {
        public FileStatusList(List<StatusEntry> statusEntries, string header) : base(statusEntries)
        {
            var listView = this.Q<ListView>("file-status-list-listview");
            SetUpListView(listView, statusEntries, 32);
            var foldout = this.Q<Foldout>("file-status-list-foldout");
            foldout.text = $"{header} ({statusEntries.Count} files)";

            if (statusEntries.Count == 0)
            {
                foldout.SetEnabled(false);
                foldout.value = false;
            }
        }

        public override FileStatusItem MakeItem()
        {
            return new FileStatusItem();
        }

        public override void BindItem(FileStatusItem item, int index)
        {
            item.SetStatusEntry(Items[index]);
        }
    }
}