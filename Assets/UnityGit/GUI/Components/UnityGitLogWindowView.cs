using UIComponents;
using UnityEngine.UIElements;
using UnityGit.Core.Services;
using UnityGit.UnityGit.Core.Data;

namespace UnityGit.GUI.Components
{
    [Layout("UnityGitLogWindowView/UnityGitLogWindowView")]
    [Stylesheet("UnityGitLogWindowView/UnityGitLogWindowView.style")]
    [RootClass("ugit-full-height")]
    [Dependency(typeof(IGitCommandService), provide: typeof(GitCommandService))]
    [Dependency(typeof(IRepositoryService), provide: typeof(RepositoryService))]
    public class UnityGitLogWindowView : UnityGitUIComponent
    {
        [Query("test-textfield")]
        private readonly TextField _commandTextField;

        [Query("test-run-button")]
        private readonly Button _runButton;

        [Query]
        private readonly UnityGitLog _unityGitLog;

        [Provide]
        private readonly IGitCommandService _gitCommandService;

        [Provide]
        private readonly IRepositoryService _repositoryService;
        
        public override void OnInit()
        {
            _runButton.clicked += OnRunButtonClick;
        }

        ~UnityGitLogWindowView()
        {
            _runButton.clicked -= OnRunButtonClick;
        }

        private void OnRunButtonClick()
        {
            var repository = _repositoryService.GetProjectRepository();
            
            _gitCommandService.Run(new GitCommandInfo(
                _commandTextField.value,
                "Running command...",
                "Running command...",
                repository));
        }
    }
}
