#if true

using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;
using UIComponents.DependencyInjection;
using UnityEditor;
using UnityEngine;
using UnityGit.Core.Services;
using UnityGit.GUI;
using UnityGit.Editor.Windows;
using UnityToolbarExtender;

namespace UnityGit.Editor.Toolbar
{
    [InitializeOnLoad]
    public static class BranchToolbarButton
    {
        private static class Styles
        {
            public static readonly GUIStyle BranchButtonStyle;

            static Styles()
            {
                BranchButtonStyle = new GUIStyle("DropDown")
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

        private static readonly BranchToolbarButtonServices Services;

        private static readonly GUIContent BranchButtonContent;

        static BranchToolbarButton()
        {
            ToolbarExtender.LeftToolbarGUI.Add(DoBranchToolbarItem);
            Services = new BranchToolbarButtonServices();

            var texture = Icons.GetIcon(Icons.Name.Merge);
            
            BranchButtonContent = new GUIContent("Branch", texture, "Switch branch");
        }

        private static void DoBranchToolbarItem()
        {
            BranchButtonContent.text = GetToolbarButtonText();

            GUILayout.Space(6);

            using (new EditorGUI.DisabledScope(!Services.StatusService.HasProjectRepository()))
                DoBranchToolbarButton();
        }

        private static string GetToolbarButtonText()
        {
            var hasRepo = Services.StatusService.HasProjectRepository();
            
            if (!hasRepo)
                return "No repository";

            var head = Services.StatusService.ProjectRepository.Head;

            return Services.BranchService.GetBranchName(head);
        }

        private static void AddBranchButtonsToMenu(GenericMenu menu, IEnumerable<Branch> branches)
        {
            foreach (var branch in branches)
            {
                var branchName = branch.FriendlyName;
                var isCurrent = branch.IsCurrentRepositoryHead;

                menu.AddItem(new GUIContent(branchName), isCurrent, item =>
                {
                    var itemAsBranch = item as Branch;
                    
                    if (itemAsBranch == null || itemAsBranch.IsCurrentRepositoryHead)
                        return;

                    var repository = Services.StatusService.ProjectRepository;

                    Services.CheckoutService.CheckoutBranch(repository, itemAsBranch);
                }, branch);   
            }
        }

        private static void DoBranchToolbarButton()
        {
            if (!GUILayout.Button(BranchButtonContent, Styles.BranchButtonStyle))
                return;
            
            var menu = new GenericMenu();
            var branchPartitions =
                Services.StatusService.ProjectRepository.Branches.GroupBy((branch) => branch.IsRemote);

            foreach (var partition in branchPartitions)
            {
                AddBranchButtonsToMenu(menu, partition);
                menu.AddSeparator("");
            }
            
            menu.AddItem(new GUIContent("Open window..."), false, BranchesWindow.ShowWindow);

            var rect = GUILayoutUtility.GetLastRect();

            rect.y += 24f;

            menu.DropDown(rect);
        }
    }
}

#endif
