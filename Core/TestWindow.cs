using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GitUnity.Core;
using GitUnity.GUI.Components;
using RepositoryStatus = GitUnity.Core.RepositoryStatus;

namespace GitUnity.GUI
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
            Debug.Log("File changed");
            Refresh();
        }

        public void CreateGUI()
        {
            var status = RepositoryStatus.Global;
            var container = new VisualElement();
            rootVisualElement.Add(container);

            var button = new Button(Refresh)
            {
                text = "Refresh"
            };
            container.Add(button);

            if (!status.ProjectRepository.IsValid())
                container.Add(new Label("Project repository is not valid."));
            else
                container.Add(new RepositoryStatusView(status.ProjectRepository.Repository, "Project repository"));

            if (status.PackageRepositories.Count == 0)
                container.Add(new Label("No package repositories found."));
            else
                foreach (var packageRepo in status.PackageRepositories)
                {
                    var packagePathParts = packageRepo.Key.Split(Path.DirectorySeparatorChar);
                    var packageName = packagePathParts[packagePathParts.Length - 2];
                    container.Add(new RepositoryStatusView(packageRepo.Value, packageName));
                }
        }

        private void Refresh()
        {
            RepositoryStatus.Global.Clear();
            RepositoryStatus.Global.PopulateRepositories();
            rootVisualElement.Clear();
            CreateGUI();
        }
    }
}