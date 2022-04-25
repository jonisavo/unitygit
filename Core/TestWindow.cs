using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GitUnity.Core;
using GitUnity.GUI.Components;
using LibGit2Sharp;
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
            {
                container.Add(new Label("Project repository is not valid."));
            }
            else
            {
                container.Add(new Label(status.ProjectRepository.Repository.Info.Path));

                var statusOptions = new StatusOptions
                {
                    ExcludeSubmodules = true,
                    IncludeIgnored = false,
                    IncludeUnaltered = false
                };
                var items = status.ProjectRepository.Repository.RetrieveStatus(statusOptions);
                var untrackedItems =
                    items.Untracked.Where(item => !item.State.HasFlag(FileStatus.Nonexistent))
                        .ToList();

                var trackedItems =
                    items.Where(item => !item.State.HasFlag(FileStatus.NewInWorkdir))
                        .ToList();
                
                container.Add(new FileStatusList(trackedItems, "Tracked"));
                
                container.Add(new FileStatusList(untrackedItems, "Untracked"));
            }

            if (status.PackageRepositories.Count == 0)
            {
                container.Add(new Label("No package repositories found."));
            }
            else
            {
                var foldout = new Foldout
                {
                    text = "Package repositories"
                };

                foreach (var packageRepo in status.PackageRepositories)
                {
                    var packageRepoContainer = new VisualElement();
                    packageRepoContainer.Add(new Label(packageRepo.Key));
                    packageRepoContainer.Add(new Label(packageRepo.Value.Info.Path));
                    foldout.Add(packageRepoContainer);
                }

                container.Add(foldout);
            }
            
            Debug.Log("Hi");
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