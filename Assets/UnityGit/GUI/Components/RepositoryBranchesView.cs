using System.Linq;
using LibGit2Sharp;
using UIComponents;
using UnityEngine.UIElements;
using UnityGit.Core.Services;

namespace UnityGit.GUI.Components
{
    [Layout("Components/RepositoryBranchesView/RepositoryBranchesView")]
    [Stylesheet("Components/RepositoryBranchesView/RepositoryBranchesView.style")]
    [Dependency(typeof(IGitCommandService), provide: typeof(GitCommandService))]
    public partial class RepositoryBranchesView : UnityGitUIComponent, IOnAttachToPanel
    {
        [Query("repository-branches-header")]
        private RepositoryHeader _header;
        [Query("repository-branches-local-list")]
        private BranchList _localBranchList;
        [Query("repository-branches-remote-list")]
        private BranchList _remoteBranchList;

        private readonly IRepository _repository;

        [Provide(CastFrom = typeof(IGitCommandService))]
        private GitCommandService _gitCommandService;

        public RepositoryBranchesView(IRepository repository)
        {
            _repository = repository;
        }

        public override void OnInit()
        {
            _gitCommandService.CommandFinished += InitializeLists;
            _header.SetRepository(_repository);
            _header.RefreshButtonClicked += OnRefreshButtonClicked;
            InitializeLists();
        }

        public void OnAttachToPanel(AttachToPanelEvent evt)
        {
            InitializeLists();
        }

        ~RepositoryBranchesView()
        {
            _gitCommandService.CommandFinished -= InitializeLists;
            _header.RefreshButtonClicked -= OnRefreshButtonClicked;
        }

        private void InitializeLists()
        {
            _localBranchList.Initialize(_repository, _repository.Branches.Where(b => !b.IsRemote));
            _remoteBranchList.Initialize(_repository, _repository.Branches.Where(b => b.IsRemote));
        }

        private void OnRefreshButtonClicked()
        {
            InitializeLists();
        }
    }
}
