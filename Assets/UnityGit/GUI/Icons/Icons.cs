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

        private static bool _initialized;

        static Icons()
        {
            IconCache = new Dictionary<string, Texture2D>();
            
            RefreshCache();
        }

        public static void RefreshCache()
        {
            IconCache.Clear();
            
            var textures = AssetDatabase.FindAssets("t:texture2D");

            if (textures.Length == 0)
                return;
            
            foreach (var guid in textures)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                
                if (!path.Contains("UnityGit/GUI/Icons"))
                    continue;

                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

                var lastPathPartIndex = path.LastIndexOf('/') + 1;
                
                // 4 comes from ".png"
                var lastPartLength = path.Length - lastPathPartIndex - 4;
                
                var lastPathPart = path.Substring(lastPathPartIndex, lastPartLength);
                
                var name = lastPathPart.ToLowerInvariant();

                IconCache.Add(name, texture);
            }

            if (IconCache.Keys.Count > 0)
                _initialized = true;
        }
        
        [CanBeNull]
        public static Texture2D GetIcon(Name name)
        {
            if (!_initialized)
                RefreshCache();
            
            var keyName = NormalizeIconName(name.ToString());
            
            if (IconCache.ContainsKey(keyName))
                return IconCache[keyName];
            
            return null;
        }
        
        private static string NormalizeIconName(string name)
        {
            var normalizedName = name.ToLowerInvariant();
            
            var proSkinName = $"d_{normalizedName}";
            
            if (EditorGUIUtility.isProSkin && IconCache.ContainsKey(proSkinName))
                normalizedName = proSkinName;
            
            return normalizedName;
        }
    }
}
