using LibGit2Sharp;
using UIComponents;
using UIComponents.Experimental;
using UnityEngine.UIElements;
using UnityGit.Core.Services;

namespace UnityGit.GUI.Components
{
    [Layout("BranchListItem/BranchListItem")]
    [Stylesheet("BranchListItem/BranchListItem.style")]
    [Dependency(typeof(IBranchService), provide: typeof(BranchService))]
    public class BranchListItem : UnityGitUIComponent, IOnAttachToPanel
    {
        [Query("branch-list-item-image")]
        private readonly Image _icon;
        [Query("branch-list-item-name-label")]
        private readonly Label _name;

        private readonly IBranchService _branchService;
        
        public BranchListItem()
        {
            _branchService = Provide<IBranchService>();
        }

        public void OnAttachToPanel(AttachToPanelEvent evt)
        {
            _icon.image = Icons.GetIcon(Icons.Name.Branch);
        }

        public void SetBranch(Branch branch)
        {
            _name.text = _branchService.GetBranchName(branch);

            if (branch.IsCurrentRepositoryHead)
                _name.text = "* " + _name.text;
            
            if (branch.IsCurrentRepositoryHead)
                _name.AddToClassList("branch-list-item-current-branch");
            else
                _name.RemoveFromClassList("branch-list-item-current-branch");
        }
    }
}