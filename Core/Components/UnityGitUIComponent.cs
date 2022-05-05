using UIComponents;
using UIComponents.Editor;
using UnityGit.GUI.Constants;

namespace UnityGit.GUI.Components
{
    [AssetPath(AssetPaths.Components)]
    [AssetPath(AssetPaths.Styles)]
    [Dependency(typeof(IAssetResolver), provide: typeof(AssetDatabaseAssetResolver))]
    public class UnityGitUIComponent : UIComponent {}
}