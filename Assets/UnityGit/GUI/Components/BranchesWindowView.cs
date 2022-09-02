using UIComponents;
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

        public override void OnInit()
        {
            var statusService = Provide<IStatusService>();
            
            if (statusService.HasProjectRepository())
                _scrollView.Add(new RepositoryBranchesView(statusService.ProjectRepository));

            foreach (var repository in statusService.PackageRepositories)
                _scrollView.Add(new RepositoryBranchesView(repository));
        }
    }
}
