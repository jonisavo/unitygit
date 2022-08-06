﻿using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityGit.GUI.Components;

namespace UnityGit.Editor
{
    public class UnityGitLogWindow : EditorWindow
    {
        [MenuItem("Git/UnityGit Log")]
        public static void ShowWindow()
        {
            var window = GetWindow<UnityGitLogWindow>();
            window.position = new Rect(0, 0, 300, 600);
            window.minSize = new Vector2(250, 350);
            window.titleContent = new GUIContent("UnityGit Log");
        }
        
        public void CreateGUI()
        {
            var container = new VisualElement();
            container.style.height = Length.Percent(100);
            container.style.flexGrow = 1;
            rootVisualElement.Clear();
            rootVisualElement.Add(container);

            container.Add(new UnityGitLog());
        }
    }
}