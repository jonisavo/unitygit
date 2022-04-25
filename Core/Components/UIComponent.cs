using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GitUnity.GUI.Components
{
    public abstract class UIComponent<T> : VisualElement where T : UIComponent<T>
    {
        private const string AssetPath = "Packages/io.savolainen.git-unity.gui/Assets/Components";

        private static readonly string ComponentName = typeof(T).Name;
        private static readonly string LayoutFileName = ComponentName + ".uxml";
        private static readonly string StyleFileName = ComponentName + ".style.uss";
        
        protected UIComponent()
        {
            LoadLayout();
            LoadStyles();
        }

        private void LoadLayout()
        {
            var path = string.Join("/", AssetPath, ComponentName, LayoutFileName);
            var layoutAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
            if (layoutAsset)
                layoutAsset.CloneTree(this);
            else
                Debug.LogWarningFormat("Could not find layout for {0}", ComponentName);
        }

        private void LoadStyles()
        {
            var path = Path.Combine(AssetPath, ComponentName, StyleFileName);
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
            if (styleSheet)
                styleSheets.Add(styleSheet);
            else
                Debug.LogWarningFormat("Could not find styles for {0}", ComponentName);
        }
    }
}