using UIComponents.Core;
using UIComponents.Editor;

namespace UnityGit.GUI.Components
{
    [Dependency(typeof(IAssetResolver), provide: typeof(AssetDatabaseAssetResolver))]
    public class UnityGitUIComponent : UIComponent {}
}