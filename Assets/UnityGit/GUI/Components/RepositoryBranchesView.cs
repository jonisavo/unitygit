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

        public RepositoryBranchesView(IRepository repository)
        {
            _header.SetRepository(repository);
            _localBranchList.SetItems(repository.Branches.Where(b => !b.IsRemote));
            _remoteBranchList.SetItems(repository.Branches.Where(b => b.IsRemote));
        }
    }
}