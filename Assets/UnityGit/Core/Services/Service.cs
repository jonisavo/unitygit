using System;
using System.Collections.Generic;
using UIComponents;
using UIComponents.Cache;
using UIComponents.DependencyInjection;
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

            _dependencyInjector = DiContext.Current.GetInjector(type);
            
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
            var serviceType = GetType();
            var fieldCache = FieldCaches[serviceType];
            var provideAttributeDictionary = fieldCache.ProvideAttributes;

            foreach (var fieldInfo in provideAttributeDictionary.Keys)
            {
                var fieldType = fieldInfo.FieldType;
                
                if (provideAttributeDictionary[fieldInfo].CastFrom != null)
                    fieldType = provideAttributeDictionary[fieldInfo].CastFrom;
                
                object value;

                try
                {
                    value = _dependencyInjector.Provide(fieldType);

                    if (provideAttributeDictionary[fieldInfo].CastFrom != null)
                        value = Convert.ChangeType(value, fieldInfo.FieldType);
                }
                catch (MissingProviderException)
                {
                    Debug.LogError($"[{serviceType.Name}] Could not provide {fieldInfo.FieldType.Name} to {fieldInfo.Name}");
                    continue;
                }
                catch (InvalidCastException)
                {
                    Debug.LogError($"[{serviceType.Name}] Could not cast {fieldType.Name} to {fieldInfo.FieldType.Name}");
                    continue;
                }

                fieldInfo.SetValue(this, value);
            }
        }
    }
}
