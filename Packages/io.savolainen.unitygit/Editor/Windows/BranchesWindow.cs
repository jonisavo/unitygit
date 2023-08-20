using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityGit.GUI.Components;

namespace UnityGit.Editor.Windows
{
    public class BranchesWindow : EditorWindow
    {
        [MenuItem("Window/Git/Branches", priority = 20)]
        public static void ShowWindow()
        {
            var window = GetWindow<BranchesWindow>();
            window.position = new Rect(0, 0, 300, 600);
            window.minSize = new Vector2(250, 350);
            window.titleContent = new GUIContent("Branches");
        }
        
        public void CreateGUI()
        {
            var container = new VisualElement();
            container.style.height = Length.Percent(100);
            container.style.flexGrow = 1;
            rootVisualElement.Clear();
            rootVisualElement.Add(container);

            container.Add(new BranchesWindowView());
        }
    }
}