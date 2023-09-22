using UIComponents;
using UIComponents.DependencyInjection;
using ILogger = UIComponents.ILogger;

namespace UnityGit.Core.Services
{
    [Dependency(typeof(ILogger), provide: typeof(DebugLogger))]
    public abstract class Service : DependencyConsumer
    {
        protected readonly ILogger Logger;

        protected Service()
        {
            Logger = Provide<ILogger>();
        }
    }
}
