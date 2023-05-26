using System.Collections.Generic;
using UIComponents.DependencyInjection;
using UnityGit.Core.Services;

namespace UnityGit.Editor.Toolbar
{
    internal class BranchToolbarButtonServices : IDependencyConsumer
    {
        public IEnumerable<IDependency> GetDependencies()
        {
            return new IDependency[]
            {
                Dependency.SingletonFor<IStatusService, StatusService>(),
                Dependency.SingletonFor<ICheckoutService, CheckoutService>(),
                Dependency.SingletonFor<IBranchService, BranchService>()
            };
        }
    }

}
