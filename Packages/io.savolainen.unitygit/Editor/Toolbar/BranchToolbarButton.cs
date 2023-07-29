﻿#if true

using LibGit2Sharp;
using UIComponents;
using UIComponents.DependencyInjection;
using UnityEditor;
using UnityEngine;
using UnityGit.Core.Services;
using UnityGit.GUI;
using UnityToolbarExtender;

namespace UnityGit.Editor.Toolbar
{
    [InitializeOnLoad]
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
            var injector = DiContext.Current.GetInjector(typeof(BranchToolbarButton));
            var services = new BranchToolbarButtonServices();
            DiContext.Current.RegisterConsumer(services);
            injector.SetConsumer(services);
            StatusService = injector.Provide<IStatusService>();
            CheckoutService = injector.Provide<ICheckoutService>();
            BranchService = injector.Provide<IBranchService>();

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
            var hasRepo = StatusService.HasProjectRepository();
            
            if (!hasRepo)
                return "No repository";

            var head = StatusService.ProjectRepository.Head;

            return BranchService.GetBranchName(head);
        }

        private static void DoBranchToolbarButton()
        {
            if (!GUILayout.Button(BranchButtonContent, Styles.branchButtonStyle))
                return;
            
            var menu = new GenericMenu();
            var branches = StatusService.ProjectRepository.Branches;

            foreach (var branch in branches)
            {
                var branchName = branch.FriendlyName;
                var isCurrent = branch.IsCurrentRepositoryHead;

                menu.AddItem(new GUIContent(branchName), isCurrent, item =>
                {
                    var itemAsBranch = item as Branch;
                    
                    if (itemAsBranch == null || itemAsBranch.IsCurrentRepositoryHead)
                        return;

                    var repository = StatusService.ProjectRepository;

                    CheckoutService.CheckoutBranch(repository, itemAsBranch);
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

#endif