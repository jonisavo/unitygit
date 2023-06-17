﻿using UnityEditor;

namespace UnityGit.Core.Services
{
    public interface IDialogService
    {
        bool Confirm(string message);

        bool Error(string message);
    }
    
    public sealed class DialogService : IDialogService
    {
        public bool Confirm(string message)
        {
            return EditorUtility.DisplayDialog("[UnityGit] Are you sure?", message, "Yes", "No");
        }

        public bool Error(string message)
        {
            return EditorUtility.DisplayDialog("[UnityGit] Error", message, "OK");
        }
    }
}
