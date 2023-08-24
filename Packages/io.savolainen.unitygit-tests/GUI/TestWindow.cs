using UIComponents;
using UnityEditor;
using UnityEngine.UIElements;

namespace UnityGit.Tests.GUI
{
    public class TestWindow : EditorWindow
    {
        private void CreateGUI()
        {
            rootVisualElement.Add(new VisualElement());    
        }

        public void AddComponent(UIComponent component)
        {
            rootVisualElement.Add(component);
        }

        public void Clear()
        {
            rootVisualElement.Clear();
        }
    }
}
