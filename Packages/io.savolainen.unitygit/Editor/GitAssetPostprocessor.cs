using UnityEditor;

namespace UnityGit.Editor
{
    public class GitAssetPostprocessor : AssetPostprocessor
    {
        public delegate void AssetsProcessedDelegate();

        public static event AssetsProcessedDelegate OnAssetsProcessed;
        
        public static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            OnAssetsProcessed?.Invoke();
        }
    }
}