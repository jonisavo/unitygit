using System.Collections.Generic;
using UnityEngine.UIElements;

namespace UnityGit.GUI.Components
{
    public abstract class UIList<TItem, TElement> : UnityGitUIComponent where TElement : VisualElement
    {
        protected readonly List<TItem> Items;

        protected UIList()
        {
            Items = new List<TItem>();
        }

        protected UIList(List<TItem> items)
        {
            Items = items;
        }

        public abstract TElement MakeItem();

        public abstract void BindItem(TElement element, int index);

        private void BindItemInternal(VisualElement element, int index)
        {
            BindItem(element as TElement, index);
        }

        public virtual void SetItems(IEnumerable<TItem> items)
        {
            Items.Clear();
            Items.AddRange(items);
        }

        protected void SetUpListView(ListView listView, List<TItem> items, int itemHeight)
        {
            listView.itemsSource = items;
            listView.makeItem = MakeItem;
            listView.bindItem = BindItemInternal;
            listView.fixedItemHeight = itemHeight;
            if (items.Count <= 10)
                listView.style.minHeight = items.Count * itemHeight;
        }
    }
}
