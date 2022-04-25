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
            Debug.Log("File changed");
            Refresh();
        }

        public void CreateGUI()
        {
            var status = UnityGitStatus.Global;
            var container = new VisualElement();
            rootVisualElement.Add(container);

            var button = new Button(Refresh)
            {
                text = "Refresh"
            };
            container.Add(button);

            if (!status.HasProjectRepository())
                container.Add(new Label("Project repository is not valid."));
            else
                container.Add(new RepositoryStatusView(status.ProjectRepository, Application.productName));

            if (!status.HasPackageRepositories())
                container.Add(new Label("No package repositories found."));
            else
                foreach (var packageRepo in status.PackageRepositories)
                {
                    var packageName = RepositoryUtilities.GetRepositoryName(packageRepo);
                    container.Add(new RepositoryStatusView(packageRepo, packageName));
                }
        }

        private void Refresh()
        {
            UnityGitStatus.Global.Clear();
            UnityGitStatus.Global.PopulateRepositories();
            rootVisualElement.Clear();
            CreateGUI();
        }
    }
}