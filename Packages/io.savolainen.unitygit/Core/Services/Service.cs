using System.Collections.Generic;
using UIComponents;
using UIComponents.DependencyInjection;
using ILogger = UIComponents.ILogger;

namespace UnityGit.Core.Services
{
    [Dependency(typeof(ILogger), provide: typeof(DebugLogger))]
    public abstract class Service : IDependencyConsumer
    {
        protected readonly ILogger Logger;
        
        private readonly DependencyInjector _dependencyInjector;

        protected Service()
        {
            DiContext.Current.RegisterConsumer(this);
            _dependencyInjector = DiContext.Current.GetInjector(GetType());
            Logger = Provide<ILogger>();
            UIC_PopulateProvideFields();
        }
        
        protected T Provide<T>() where T : class
        {
            return _dependencyInjector.Provide<T>();
        }

        public abstract IEnumerable<IDependency> GetDependencies();

        protected abstract void UIC_PopulateProvideFields();
    }
}
