using LibGit2Sharp;
using UIComponents;
using UIComponents.Experimental;
using UnityEngine.UIElements;
using UnityGit.Core.Services;

namespace UnityGit.GUI.Components
{
    [Layout("BranchListItem/BranchListItem")]
    [Stylesheet("BranchListItem/BranchListItem.style")]
    [Dependency(typeof(IBranchService), provide: typeof(BranchService))]
    [Dependency(typeof(IPushService), provide: typeof(PushService))]
    [Dependency(typeof(IPullService), provide: typeof(PullService))]
    [Dependency(typeof(ICommitService), provide: typeof(CommitService))]
    [Dependency(typeof(IGitCommandService), provide: typeof(GitCommandService))]
    public class BranchListItem : UnityGitUIComponent, IOnAttachToPanel
    {
        [Query("branch-list-item-image")]
        private readonly Image _icon;
        [Query("branch-list-item-name-label")]
        private readonly Label _name;
        
        [Query("branch-list-item-pull-button")]
        private readonly Button _pullButton;
        [Query("branch-list-item-pull-button-image")]
        private readonly Image _pullButtonImage;
        [Query("branch-list-item-push-button")]
        private readonly Button _pushButton;
        [Query("branch-list-item-push-button-image")]
        private readonly Image _pushButtonImage;

        [Provide]
        private readonly IBranchService _branchService;
        [Provide]
        private readonly IPullService _pullService;
        [Provide]
        private readonly ICommitService _commitService;
        [Provide]
        private readonly IPushService _pushService;
        // Not declared as an interface, since we need to use events.
        private readonly GitCommandService _gitCommandService;
        
        private readonly IRepository _repository;
        private Branch _branch;

        public BranchListItem(IRepository repository)
        {
            _repository = repository;
            
            _gitCommandService = Provide<IGitCommandService>() as GitCommandService;
            _gitCommandService.CommandStarted += OnGitCommandStarted;
            _gitCommandService.CommandFinished += OnGitCommandFinished;
            
            _commitService.CommitCreated += OnCommitCreated;

            _pullButton.clicked += OnPullButtonClicked;
            _pushButton.clicked += OnPushButtonClicked;
            
            _icon.image = Icons.GetIcon(Icons.Name.Branch);
            _pullButtonImage.image = Icons.GetIcon(Icons.Name.Pull);
            _pushButtonImage.image = Icons.GetIcon(Icons.Name.Push);
        }
        
        ~BranchListItem()
        {
            _commitService.CommitCreated -= OnCommitCreated;
            _gitCommandService.CommandStarted -= OnGitCommandStarted;
            _gitCommandService.CommandFinished -= OnGitCommandFinished;
            _pullButton.clicked -= OnPullButtonClicked;
            _pushButton.clicked -= OnPushButtonClicked;
        }

        public void OnAttachToPanel(AttachToPanelEvent evt)
        {
            UpdateUpdateButtons();
        }

        private void OnGitCommandStarted()
        {
            UpdateUpdateButtons();
        }

        private void OnGitCommandFinished()
        {
            UpdateUpdateButtons();
        }

        private void OnCommitCreated(Commit commit)
        {
            if (_branch.Tip == commit)
                UpdateUpdateButtons();
        }
        
        private void UpdateLabel()
        {
            _name.text = _branchService.GetBranchName(_branch);

            if (_branch.IsCurrentRepositoryHead)
                _name.text = "* " + _name.text;
            
            if (_branch.IsCurrentRepositoryHead)
                _name.AddToClassList("branch-list-item-current-branch");
            else
                _name.RemoveFromClassList("branch-list-item-current-branch");
        }

        private void UpdateUpdateButtons()
        {
            if (_branchService.IsBehindRemote(_branch))
                _pullButton.RemoveFromClassList("disabled-button");
            else
                _pullButton.AddToClassList("disabled-button");
            
            if (_branchService.IsAheadOfRemote(_branch))
                _pushButton.RemoveFromClassList("disabled-button");
            else
                _pushButton.AddToClassList("disabled-button");
            
            _pushButton.SetEnabled(!_gitCommandService.IsRunning);
        }

        public void SetBranch(Branch branch)
        {
            _branch = branch;
            UpdateLabel();
            UpdateUpdateButtons();
        }
        
        private void OnPullButtonClicked()
        {
            _pullService.Pull(_repository as Repository);
        }
        
        private void OnPushButtonClicked()
        {
            _pushService.Push(_repository, _branch);
        }
    }
}