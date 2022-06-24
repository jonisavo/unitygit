using System;
using System.Collections.Generic;
using UIComponents;
using UIComponents.Cache;
using UnityEngine;

namespace UnityGit.Core.Services
{
    public abstract class Service
    {
        private static readonly Dictionary<Type, FieldCache> FieldCaches =
            new Dictionary<Type, FieldCache>();
        
        private readonly DependencyInjector _dependencyInjector;

        protected Service()
        {
            var type = GetType();
            
            _dependencyInjector = DependencyInjector.GetInjector(type);
            
            if (!FieldCaches.ContainsKey(type))
                FieldCaches.Add(type, new FieldCache(type));
            
            PopulateProvideFields();
        }
        
        protected T Provide<T>() where T : class
        {
            return _dependencyInjector.Provide<T>();
        }
        
        private void PopulateProvideFields()
        {
            var fieldCache = FieldCaches[GetType()];
            var provideAttributeDictionary = fieldCache.ProvideAttributes;

            foreach (var fieldInfo in provideAttributeDictionary.Keys)
            {
                object value;

                try
                {
                    value = _dependencyInjector.Provide(fieldInfo.FieldType);
                }
                catch (MissingProviderException)
                {
                    Debug.LogError($"[${GetType().Name}] Could not provide {fieldInfo.FieldType.Name} to {fieldInfo.Name}");
                    continue;
                }
                
                fieldInfo.SetValue(this, value);
            }
        }
    }
}