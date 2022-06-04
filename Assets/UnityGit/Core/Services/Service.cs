using UIComponents;

namespace UnityGit.Core.Services
{
    public abstract class Service
    {
        private readonly DependencyInjector _dependencyInjector;

        protected Service()
        {
            _dependencyInjector = DependencyInjector.GetInjector(GetType());
        }
        
        protected T Provide<T>() where T : class
        {
            return _dependencyInjector.Provide<T>();
        }
    }
}