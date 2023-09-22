using UIComponents;
using UIComponents.DependencyInjection;
using UnityGit.Core.Services;

namespace UnityGit.Editor.Toolbar
{
    [Dependency(typeof(IStatusService), provide: typeof(StatusService))]
    [Dependency(typeof(ICheckoutService), provide: typeof(CheckoutService))]
    [Dependency(typeof(IBranchService), provide: typeof(BranchService))]
    internal partial class BranchToolbarButtonServices : DependencyConsumer
    {
        [Provide]
        public IStatusService StatusService;
        
        [Provide]
        public ICheckoutService CheckoutService;
        
        [Provide]
        public IBranchService BranchService;
    }

}
