using UnityEditor;
using UnityEditor.VersionControl;

namespace UnityGit.Core
{
    public class GitAssetModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        public delegate void FileChangedDelegate(string path);

        public static event FileChangedDelegate FileChanged;
        
        public static void FileModeChanged(string[] paths, FileMode mode)
        {
            for (var i = 0; i < paths.Length; i++)
                FileChanged?.Invoke(paths[i]);
        }

        public static void OnWillCreateAsset(string assetName)
        {
            FileChanged?.Invoke(assetName);
        }

        public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            FileChanged?.Invoke(assetPath);
            return AssetDeleteResult.DidNotDelete;
        }

        public static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            FileChanged?.Invoke(destinationPath);
            return AssetMoveResult.DidNotMove;
        }

        public static string[] OnWillSaveAssets(string[] paths)
        {
            for (var i = 0; i < paths.Length; i++)
                FileChanged?.Invoke(paths[i]);
            return paths;
        }
    }
}