using System.Linq;
using LibGit2Sharp;
using UnityEngine.UIElements;

namespace UnityGit.GUI.Components
{
    public class RepositoryStatusView : UIComponent<RepositoryStatusView>
    {
        public RepositoryStatusView(IRepository repository, string name)
        {
            var repositoryPath = repository.Info.Path;

            this.Q<Label>("repository-status-name-label").text = name;
            this.Q<Label>("repository-status-path-label").text = repositoryPath;
            
            var statusOptions = new StatusOptions
            {
                ExcludeSubmodules = true,
                IncludeIgnored = false,
                IncludeUnaltered = false
            };
            var items = repository.RetrieveStatus(statusOptions);
            var untrackedItems =
                items.Untracked.Where(item => !item.State.HasFlag(FileStatus.Nonexistent))
                    .ToList();

            var trackedItems =
                items.Where(item => !item.State.HasFlag(FileStatus.NewInWorkdir))
                    .ToList();
            
            Add(new FileStatusList(trackedItems, "Tracked"));
            Add(new FileStatusList(untrackedItems, "Untracked"));
        }
    }
}