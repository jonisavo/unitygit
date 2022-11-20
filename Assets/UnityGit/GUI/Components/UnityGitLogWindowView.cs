using UIComponents;
using UnityEngine.UIElements;
using UnityGit.Core.Services;
using UnityGit.UnityGit.Core.Data;

namespace UnityGit.GUI.Components
{
    [Layout("Components/UnityGitLogWindowView/UnityGitLogWindowView")]
    [Stylesheet("Components/UnityGitLogWindowView/UnityGitLogWindowView.style")]
    [RootClass("ugit-full-height")]
    [Dependency(typeof(IGitCommandService), provide: typeof(GitCommandService))]
    [Dependency(typeof(IRepositoryService), provide: typeof(RepositoryService))]
    public partial class UnityGitLogWindowView : UnityGitUIComponent
    {
        [Query("test-textfield")]
        private TextField _commandTextField;

        [Query("test-run-button")]
        private Button _runButton;

        [Query]
        private UnityGitLog _unityGitLog;

        [Provide]
        private IGitCommandService _gitCommandService;

        [Provide]
        private IRepositoryService _repositoryService;
        
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
