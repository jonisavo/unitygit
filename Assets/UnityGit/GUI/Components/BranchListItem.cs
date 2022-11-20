using LibGit2Sharp;
using UIComponents;
using UnityEngine.UIElements;
using UnityGit.Core.Services;

namespace UnityGit.GUI.Components
{
    [Layout("Components/BranchListItem/BranchListItem")]
    [Stylesheet("Components/BranchListItem/BranchListItem.style")]
    [Dependency(typeof(IBranchService), provide: typeof(BranchService))]
    [Dependency(typeof(IPushService), provide: typeof(PushService))]
    [Dependency(typeof(IPullService), provide: typeof(PullService))]
    [Dependency(typeof(ICommitService), provide: typeof(CommitService))]
    [Dependency(typeof(IGitCommandService), provide: typeof(GitCommandService))]
    public partial class BranchListItem : UnityGitUIComponent, IOnAttachToPanel
    {
        [Query("branch-list-item-image")]
        private Image _icon;
        [Query("branch-list-item-name-label")]
        private Label _name;
        
        [Query("branch-list-item-pull-button")]
        private Button _pullButton;
        [Query("branch-list-item-pull-button-image")]
        private Image _pullButtonImage;
        [Query("branch-list-item-push-button")]
        private Button _pushButton;
        [Query("branch-list-item-push-button-image")]
        private Image _pushButtonImage;

        [Provide]
        private IBranchService _branchService;
        [Provide]
        private IPullService _pullService;
        [Provide]
        private ICommitService _commitService;
        [Provide]
        private IPushService _pushService;
        [Provide(CastFrom = typeof(IGitCommandService))]
        private GitCommandService _gitCommandService;
        
        private readonly IRepository _repository;
        private Branch _branch;

        public BranchListItem(IRepository repository)
        {
            _repository = repository;
        }

        public override void OnInit()
        {
            _gitCommandService.CommandStarted += OnGitCommandStarted;
            _gitCommandService.CommandFinished += OnGitCommandFinished;
            _commitService.CommitCreated += OnCommitCreated;
            
            _pullButton.clicked += OnPullButtonClicked;
            _pushButton.clicked += OnPushButtonClicked;
            
            _icon.image = Icons.GetIcon(Icons.Name.Branch);
            _pullButtonImage.image = Icons.GetIcon(Icons.Name.Pull);
            _pushButtonImage.image = Icons.GetIcon(Icons.Name.Push);
            
            UpdateLabel();
            UpdateUpdateButtons();
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

            if (!Initialized)
                return;
            
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
