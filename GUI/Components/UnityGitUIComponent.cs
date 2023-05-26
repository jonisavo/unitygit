using UIComponents;
using UIComponents.Editor;

namespace UnityGit.GUI.Components
{
    [AssetPrefix("Packages/io.savolainen.unitygit/GUI/Files/")]
    [Stylesheet("Styles/Dimensions.uss")]
    [Stylesheet("Styles/Windows.uss")]
    [Dependency(typeof(IAssetResolver), provide: typeof(AssetDatabaseAssetResolver))]
    public abstract class UnityGitUIComponent : UIComponent {}
}
