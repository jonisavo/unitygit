using UnityEditor;
using UnityEngine.TestTools;

namespace UnityGit.Core.Services
{
    public interface IDialogService
    {
        bool Confirm(string message);

        void Error(string message);
    }
    
    /// <summary>Acts as a wrapper for Unity's EditorUtility.DisplayDialog method.</summary>
    [ExcludeFromCoverage]
    public sealed class DialogService : IDialogService
    {
        public bool Confirm(string message)
        {
            return EditorUtility.DisplayDialog("[UnityGit] Are you sure?", message, "Yes", "No");
        }

        public void Error(string message)
        {
            EditorUtility.DisplayDialog("[UnityGit] Error", message, "OK");
        }
    }
}
