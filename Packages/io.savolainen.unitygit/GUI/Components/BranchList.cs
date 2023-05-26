using System.Collections.Generic;
using LibGit2Sharp;
using UIComponents;
using UnityEngine.UIElements;

namespace UnityGit.GUI.Components
{
    [Layout("Components/BranchList/BranchList.uxml")]
    [Stylesheet("Components/BranchList/BranchList.style.uss")]
    public partial class BranchList : UIList<Branch, BranchListItem>
    {
        [Query("branch-list-header-label")]
        private Label _headerLabel;
        [Query("branch-list-header-count-label")]
        private Label _countLabel;
        [Query("branch-list-listview")]
        private ListView _listView;

        [UxmlTrait(Name = "header")]
        public string HeaderText;

        private IRepository _repository;

        public override void OnInit()
        {
            _headerLabel.text = HeaderText;
        }

        public void Initialize(IRepository repository, IEnumerable<Branch> branches)
        {
            _repository = repository;

            base.SetItems(branches);

            _countLabel.text = $"{Items.Count.ToString()} branches";
            SetUpListView(_listView, Items, 32);
            
            _listView.Rebuild();
        }

        public override BranchListItem MakeItem()
        {
            return new BranchListItem(_repository);
        }

        public override void BindItem(BranchListItem item, int index)
        {
            item.SetBranch(Items[index]);
        }
    }
}
