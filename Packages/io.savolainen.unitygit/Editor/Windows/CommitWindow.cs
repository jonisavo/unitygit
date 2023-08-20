using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityGit.GUI.Components;

namespace UnityGit.Editor.Windows
{
    public class CommitWindow : EditorWindow
    {
        private bool _shouldRedraw;
        
        [MenuItem("Window/Git/Commit", priority = 10)]
        public static void ShowWindow()
        {
            var window = GetWindow<CommitWindow>();
            window.position = new Rect(0, 0, 300, 600);
            window.minSize = new Vector2(250, 350);
            window.titleContent = new GUIContent("Commit");
        }

        private void OnEnable()
        {
            GitAssetModificationProcessor.FileChanged += OnFileChanged;
            GitAssetPostprocessor.OnAssetsProcessed += RequestRedraw;
            EditorApplication.update += RedrawIfRequested;
        }

        private void OnDisable()
        {
            GitAssetModificationProcessor.FileChanged -= OnFileChanged;
            GitAssetPostprocessor.OnAssetsProcessed -= RequestRedraw;
            EditorApplication.update -= RedrawIfRequested;
        }

        private void RequestRedraw()
        {
            _shouldRedraw = true;
        }

        private void RedrawIfRequested()
        {
            if (_shouldRedraw)
                CreateGUI();
        }

        private void OnProjectChange()
        {
            RequestRedraw();
        }

        private void OnFileChanged(string path)
        {
            RequestRedraw();
        }

        public void CreateGUI()
        {
            _shouldRedraw = false;
            
            var container = new VisualElement();
            container.style.height = Length.Percent(100);
            container.style.flexGrow = 1;
            rootVisualElement.Clear();
            rootVisualElement.Add(container);

            container.Add(new CommitWindowView());
        }
    }
}
