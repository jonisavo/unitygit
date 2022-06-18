using LibGit2Sharp;
using UIComponents;
using UnityEditor;
using UnityEngine;
using UnityGit.Core.Services;
using UnityGit.GUI;
using UnityToolbarExtender;

namespace UnityGit.Editor.Toolbar
{
    [InitializeOnLoad]
    [Dependency(typeof(IStatusService), provide: typeof(StatusService))]
    [Dependency(typeof(ICheckoutService), provide: typeof(CheckoutService))]
    public static class BranchToolbarButton
    {
        private static class Styles
        {
            public static readonly GUIStyle branchButtonStyle;

            static Styles()
            {
                branchButtonStyle = new GUIStyle("DropDown")
                {
                    stretchWidth = false,
                    clipping = TextClipping.Overflow,
                    fontSize = 14,
                    alignment = TextAnchor.MiddleLeft,
                    imagePosition = ImagePosition.ImageLeft,
                    fontStyle = FontStyle.Bold
                };
            }
        }
        
        private static readonly IStatusService StatusService;
        private static readonly ICheckoutService CheckoutService;

        private static readonly GUIContent BranchButtonContent;

        private static readonly GUILayoutOption[] BranchButtonOptions = new []
        {
            GUILayout.MaxWidth(100)
        };
        
        private const string IconName = "UnityGit/Icons/icons8-merge-git-24";
        
        static BranchToolbarButton()
        {
            ToolbarExtender.LeftToolbarGUI.Add(DoBranchToolbarItem);
            var dependencyInjector = DependencyInjector.GetInjector(typeof(BranchToolbarButton));
            StatusService = dependencyInjector.Provide<IStatusService>();
            CheckoutService = dependencyInjector.Provide<ICheckoutService>();
            
            var texture = Icons.GetIcon(Icons.Name.Merge);
            
            BranchButtonContent = new GUIContent("Branch", texture, "Switch branch");
        }

        private static void DoBranchToolbarItem()
        {
            string title;

            var hasRepo = StatusService.HasProjectRepository();

            if (!hasRepo)
                title = "No repository";
            else
                title = StatusService.ProjectRepository.Head.FriendlyName;

            BranchButtonContent.text = title;

            if (BranchButtonContent.image == null)
                BranchButtonContent.image = Icons.GetIcon(Icons.Name.Merge);

            using (new EditorGUI.DisabledScope(!hasRepo))
                DoBranchToolbarButton();
        }

        private static void DoBranchToolbarButton()
        {
            if (!GUILayout.Button(BranchButtonContent, Styles.branchButtonStyle))
                return;
            
            var menu = new GenericMenu();

            foreach (var branch in StatusService.ProjectRepository.Branches)
            {
                var branchName = branch.FriendlyName;
                var isCurrent = branch.IsCurrentRepositoryHead;

                menu.AddItem(new GUIContent(branchName), isCurrent, item =>
                {
                    var itemAsBranch = item as Branch;
                    
                    if (itemAsBranch == null || itemAsBranch.IsCurrentRepositoryHead)
                        return;

                    CheckoutService.CheckoutBranch(StatusService.ProjectRepository, itemAsBranch);
;               }, branch);
            }

            var rect = GUILayoutUtility.GetLastRect();
            
            rect.y += rect.height;
            
            menu.DropDown(rect);
        }
    }
}