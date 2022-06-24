using System.Collections.Generic;
using LibGit2Sharp;
using UIComponents;
using UIComponents.Experimental;
using UnityEngine.UIElements;

namespace UnityGit.GUI.Components
{
    [Layout("BranchList/BranchList")]
    [Stylesheet("BranchList/BranchList.style")]
    public class BranchList : UIList<Branch, BranchListItem>
    {
        public new class UxmlFactory : UxmlFactory<BranchList, UxmlTraits> {}

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _header =
                new UxmlStringAttributeDescription { name = "header" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ((BranchList)ve)._headerLabel.text = _header.GetValueFromBag(bag, cc);
            }
        }
        
        [Query("branch-list-header-label")]
        private readonly Label _headerLabel;
        [Query("branch-list-header-count-label")]
        private readonly Label _countLabel;
        [Query("branch-list-listview")]
        private readonly ListView _listView;

        private IRepository _repository;

        public void Initialize(IRepository repository, IEnumerable<Branch> branches)
        {
            _repository = repository;

            base.SetItems(branches);
            
            _countLabel.text = $"{Items.Count.ToString()} branches";
            SetUpListView(_listView, Items, 32);
            
            _listView.Refresh();
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