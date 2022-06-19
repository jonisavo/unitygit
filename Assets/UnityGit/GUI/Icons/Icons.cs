using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace UnityGit.GUI
{
    public static class Icons
    {
        public enum Name
        {
            Merge,
            Branch
        }

        private static readonly Dictionary<string, Texture2D> IconCache;

        static Icons()
        {
            var textures = AssetDatabase.FindAssets("t:texture2D");
            
            IconCache = new Dictionary<string, Texture2D>();
            
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
        }
        
        [CanBeNull]
        public static Texture2D GetIcon(Name name)
        {
            var keyName = NormalizeIconName(name.ToString());
            
            if (IconCache.ContainsKey(keyName))
                return IconCache[keyName];
            
            return null;
        }
        
        private static string NormalizeIconName(string name)
        {
            var normalizedName = name.ToLowerInvariant();
            
            if (EditorGUIUtility.isProSkin)
                normalizedName = $"d_{normalizedName}";
            
            return normalizedName;
        }
    }
}