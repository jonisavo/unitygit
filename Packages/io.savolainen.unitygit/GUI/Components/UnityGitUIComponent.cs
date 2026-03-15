using UIComponents;
using UIComponents.Editor;

namespace UnityGit.GUI.Components
{
    [AssetRoot("Packages/io.savolainen.unitygit/GUI/Files/")]
    [SharedStylesheet("Styles/Dimensions.uss")]
    [SharedStylesheet("Styles/Windows.uss")]
    [Dependency(typeof(IAssetSource), provide: typeof(AssetDatabaseAssetSource))]
    public abstract class UnityGitUIComponent : UIComponent {}
}
