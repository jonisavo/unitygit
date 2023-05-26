using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace UnityGit.GUI
{
    internal static class Icons
    {
        public enum Name
        {
            Merge,
            Branch,
            Refresh,
            Push,
            Pull
        }

        private static readonly Dictionary<string, Texture2D> IconCache;
        private static readonly Dictionary<string, Texture2D> ProSkinIconCache;

        private static bool _initialized;

        static Icons()
        {
            IconCache = new Dictionary<string, Texture2D>();
            ProSkinIconCache = new Dictionary<string, Texture2D>();
            
            RefreshCache();
        }

        public static void RefreshCache()
        {
            IconCache.Clear();
            ProSkinIconCache.Clear();

            var iconNames = Enum.GetNames(typeof(Name));

            const string basePath = "Packages/io.savolainen.unitygit/GUI/Icons/";

            foreach (var iconName in iconNames)
            {
                var normalTexture = AssetDatabase.LoadAssetAtPath<Texture2D>($"{basePath}{iconName}.png");
                var proSkinTexture = AssetDatabase.LoadAssetAtPath<Texture2D>($"{basePath}d_{iconName}.png");
                
                var iconNameLower = iconName.ToLowerInvariant();
                
                if (normalTexture != null)
                    IconCache.Add(iconNameLower, normalTexture);
                
                if (proSkinTexture != null)
                    ProSkinIconCache.Add(iconNameLower, proSkinTexture);
            }
            
            _initialized = IconCache.Keys.Count > 0;
        }
        
        [CanBeNull]
        public static Texture2D GetIcon(Name name)
        {
            if (!_initialized)
                RefreshCache();

            var lowercaseName = name.ToString().ToLowerInvariant();
            
            if (EditorGUIUtility.isProSkin && ProSkinIconCache.ContainsKey(lowercaseName))
                return ProSkinIconCache[lowercaseName];

            if (IconCache.ContainsKey(lowercaseName))
                return IconCache[lowercaseName];
            
            return null;
        }
    }
}
