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
    [Dependency(typeof(IBranchService), provide: typeof(BranchService))]
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
        private static readonly IBranchService BranchService;

        private static readonly GUIContent BranchButtonContent;

        static BranchToolbarButton()
        {
            ToolbarExtender.LeftToolbarGUI.Add(DoBranchToolbarItem);
            var dependencyInjector = DependencyInjector.GetInjector(typeof(BranchToolbarButton));
            StatusService = dependencyInjector.Provide<IStatusService>();
            CheckoutService = dependencyInjector.Provide<ICheckoutService>();
            BranchService = dependencyInjector.Provide<IBranchService>();
            
            var texture = Icons.GetIcon(Icons.Name.Merge);
            
            BranchButtonContent = new GUIContent("Branch", texture, "Switch branch");
        }

        private static void DoBranchToolbarItem()
        {
            BranchButtonContent.text = GetToolbarButtonText();

            GUILayout.Space(6);

            using (new EditorGUI.DisabledScope(!StatusService.HasProjectRepository()))
                DoBranchToolbarButton();
        }

        private static string GetToolbarButtonText()
        {
            string text;

            var hasRepo = StatusService.HasProjectRepository();
            
            if (!hasRepo)
                text = "No repository";
            else
                text = BranchService.GetBranchName(StatusService.ProjectRepository.Head);

            return text;
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
            
            menu.AddSeparator("");
            
            menu.AddItem(new GUIContent("Open window..."), false, BranchesWindow.ShowWindow);

            var rect = GUILayoutUtility.GetLastRect();

            rect.y += 24f;

            menu.DropDown(rect);
        }
    }
}