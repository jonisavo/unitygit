using UIComponents;
using UIComponents.Experimental;
using UnityEngine.UIElements;
using UnityGit.Core.Services;

namespace UnityGit.GUI.Components
{
    [Layout("BranchesWindowView/BranchesWindowView")]
    [Stylesheet("BranchesWindowView/BranchesWindowView.style")]
    [Stylesheet("Windows")]
    [Dependency(typeof(IStatusService), provide: typeof(StatusService))]
    public class BranchesWindowView : UnityGitUIComponent
    {
        [Query("branches-window-view-scroll-view")]
        private readonly ScrollView _scrollView;

        private readonly IStatusService _statusService;
        
        public BranchesWindowView()
        {
            _statusService = Provide<IStatusService>();
            
            if (_statusService.HasProjectRepository())
                _scrollView.Add(new RepositoryBranchesView(_statusService.ProjectRepository));

            foreach (var repository in _statusService.PackageRepositories)
                _scrollView.Add(new RepositoryBranchesView(repository));
        }
    }
}