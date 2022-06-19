using System.Linq;
using LibGit2Sharp;
using UIComponents;
using UIComponents.Experimental;

namespace UnityGit.GUI.Components
{
    [Layout("RepositoryBranchesView/RepositoryBranchesView")]
    [Stylesheet("RepositoryBranchesView/RepositoryBranchesView.style")]
    public class RepositoryBranchesView : UnityGitUIComponent
    {
        [Query("repository-branches-header")]
        private readonly RepositoryHeader _header;
        [Query("repository-branches-local-list")]
        private readonly BranchList _localBranchList;
        [Query("repository-branches-remote-list")]
        private readonly BranchList _remoteBranchList;

        private readonly IRepository _repository;

        public RepositoryBranchesView(IRepository repository)
        {
            _repository = repository;
            _header.SetRepository(repository);
            _header.RefreshButtonClicked += OnRefreshButtonClicked;
            InitializeLists();
        }

        ~RepositoryBranchesView()
        {
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