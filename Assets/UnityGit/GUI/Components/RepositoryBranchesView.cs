using System.Linq;
using LibGit2Sharp;
using UIComponents;
using UnityEngine.UIElements;
using UnityGit.Core.Services;

namespace UnityGit.GUI.Components
{
    [Layout("RepositoryBranchesView/RepositoryBranchesView")]
    [Stylesheet("RepositoryBranchesView/RepositoryBranchesView.style")]
    [Dependency(typeof(IGitCommandService), provide: typeof(GitCommandService))]
    public class RepositoryBranchesView : UnityGitUIComponent, IOnAttachToPanel
    {
        [Query("repository-branches-header")]
        private readonly RepositoryHeader _header;
        [Query("repository-branches-local-list")]
        private readonly BranchList _localBranchList;
        [Query("repository-branches-remote-list")]
        private readonly BranchList _remoteBranchList;

        private readonly IRepository _repository;

        [Provide(CastFrom = typeof(IGitCommandService))]
        private readonly GitCommandService _gitCommandService;

        public RepositoryBranchesView(IRepository repository)
        {
            _repository = repository;
        }

        public override async void OnInit()
        {
            _gitCommandService.CommandFinished += InitializeLists;
            await _header.SetRepository(_repository);
            _header.RefreshButtonClicked += OnRefreshButtonClicked;
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
