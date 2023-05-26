using System.Collections.Generic;
using UnityEditor;
using UnityGit.GUI.Components;

namespace UnityGit.Editor
{
    public static class UnityGitSettingsProviderRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateUnityGitSettingsProvider()
        {
            var provider = new SettingsProvider("Project/UnityGit", SettingsScope.Project)
            {
                label = "UnityGit",
                activateHandler = (searchContext, rootElement) =>
                {
                    rootElement.Add(new SettingsProviderView());
                },
                keywords = new HashSet<string>(new[] {"Git", "Repository"})
            };

            return provider;
        }
    }
}