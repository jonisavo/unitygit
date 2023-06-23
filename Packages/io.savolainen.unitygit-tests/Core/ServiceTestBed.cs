using System;
using JetBrains.Annotations;
using UIComponents.DependencyInjection;
using UnityGit.Core.Services;

namespace UnityGit.Tests.Core
{
    public sealed class ServiceTestBed<TService> where TService : Service
    {
        private readonly DiContext _diContext = new DiContext();

        private readonly Type _componentType = typeof(TService);

        public TService Instantiate(Func<TService> factoryPredicate)
        {
            var previousContext = DiContext.Current;

            DiContext.ChangeCurrent(_diContext);

            TService service;

            try
            {
                service = factoryPredicate();
            }
            finally
            {
                DiContext.ChangeCurrent(previousContext);
            }

            return service;
        }
        
        public TService Instantiate()
        {
            return Instantiate(Activator.CreateInstance<TService>);
        }
        
        public ServiceTestBed<TService> WithSingleton<TDependency>([NotNull] TDependency value)
            where TDependency : class
        {
            var injector = _diContext.GetInjector(_componentType);

            injector.SetSingletonOverride(value);

            return this;
        }

        /// <summary>
        /// Overrides a transient dependency.
        /// </summary>
        /// <typeparam name="TDependency">Dependency type</typeparam>
        /// <param name="value">New transient value</param>
        /// <exception cref="ArgumentNullException">Thrown if value is null</exception>
        public ServiceTestBed<TService> WithTransient<TDependency>([NotNull] TDependency value)
            where TDependency : class
        {
            var injector = _diContext.GetInjector(_componentType);

            injector.SetTransientInstance(value);

            return this;
        }
    }
}
