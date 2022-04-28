using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityGit.Core;
using UnityGit.GUI.Components;

namespace UnityGit.GUI
{
    public class TestWindow : EditorWindow
    {
        private bool _shouldRedraw;
        
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