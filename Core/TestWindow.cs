using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityGit.Core;
using UnityGit.Core.Utilities;
using UnityGit.GUI.Components;

namespace UnityGit.GUI
{
    public class TestWindow : EditorWindow
    {
        [MenuItem("Window/Test Window")]
        public static void ShowWindow()
        {
            var window = GetWindow<TestWindow>();
            window.position = new Rect(0, 0, 300, 600);
            window.titleContent = new GUIContent("Test Window");
        }

        private void OnEnable()
        {
            GitAssetModificationProcessor.FileChanged += OnFileChanged;
        }

        private void OnDisable()
        {
            GitAssetModificationProcessor.FileChanged -= OnFileChanged;
        }

        private void OnFileChanged(string path)
        {
            Refresh();
        }

        private void OnBecameVisible()
        {
            Refresh();
        }

        public void CreateGUI()
        {
            var status = UnityGitStatus.Global;
            var container = new VisualElement();
            container.style.height = Length.Percent(100);
            container.style.flexGrow = 1;
            rootVisualElement.Clear();
            rootVisualElement.Add(container);

            var button = new Button(Refresh)
            {
                text = "Refresh"
            };
            container.Add(button);

            container.Add(new CommitWindowView(status));
        }

        private void Refresh()
        {
            UnityGitStatus.Global.PopulateRepositories();
            CreateGUI();
        }
    }
}