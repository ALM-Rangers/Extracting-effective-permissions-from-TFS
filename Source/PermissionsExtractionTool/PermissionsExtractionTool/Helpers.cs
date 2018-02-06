// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Helpers.cs" company="Microsoft Corporation">
//   Microsoft Visual Studio ALM Rangers
// </copyright>
// <summary>
//   The helpers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.PermissionsExtractionTool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.TeamFoundation.Framework.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using TeamFoundation;
    using TeamFoundation.Build.Common;
    using TeamFoundation.Server;
    using TeamFoundation.VersionControl.Common;

    /// <summary>
    /// The helpers.
    /// </summary>
    public static class Helpers
    {
        #region Static Fields

        /// <summary>
        /// The permissions.
        /// </summary>
        private static readonly List<Permission> Permissions = new List<Permission>
        {
            // Team Project
            new Permission(PermissionScope.TeamProject, "Create test runs", "PublishTestResults", PermissionActionIdConstants.PublishTestResults, AuthorizationProjectPermissions.PublishTestResults, string.Empty), 
            new Permission(PermissionScope.TeamProject, "Delete team project", "Delete", PermissionActionIdConstants.Delete, AuthorizationProjectPermissions.Delete, string.Empty), 
            new Permission(PermissionScope.TeamProject, "Delete test runs", "DeleteTestResults", PermissionActionIdConstants.DeleteTestResults, AuthorizationProjectPermissions.DeleteTestResults, string.Empty), 
            new Permission(PermissionScope.TeamProject, "Edit project-level information", "GenericWrite", PermissionActionIdConstants.GenericWrite, AuthorizationProjectPermissions.GenericWrite, string.Empty), 
            new Permission(PermissionScope.TeamProject, "Manage test configurations", "ManageTestConfigurations", PermissionActionIdConstants.ManageTestConfigurations, AuthorizationProjectPermissions.ManageTestConfigurations, string.Empty), 
            new Permission(PermissionScope.TeamProject, "Manage test environments", "ManageTestEnvironments", PermissionActionIdConstants.ManageTestEnvironments, AuthorizationProjectPermissions.ManageTestEnvironments, string.Empty), 
            new Permission(PermissionScope.TeamProject, "View project-level information", "GenericRead", PermissionActionIdConstants.GenericRead, AuthorizationProjectPermissions.GenericRead, string.Empty), 
            new Permission(PermissionScope.TeamProject, "View test runs", "ViewTestResults", PermissionActionIdConstants.ViewTestResults, AuthorizationProjectPermissions.ViewTestResults, string.Empty), 

            // Team Build
            new Permission(PermissionScope.TeamBuild, "Administer build permissions", "AdministerBuildPermissions", PermissionStringConstants.AdministerBuildPermissions, BuildPermissions.AdministerBuildPermissions, string.Empty), 
            new Permission(PermissionScope.TeamBuild, "Delete build definition", "DeleteBuildDefinition", PermissionStringConstants.DeleteBuildDefinition, BuildPermissions.DeleteBuildDefinition, string.Empty), 
            new Permission(PermissionScope.TeamBuild, "Delete builds", "DeleteBuilds", PermissionStringConstants.DeleteBuilds, BuildPermissions.DeleteBuilds, string.Empty), 
            new Permission(PermissionScope.TeamBuild, "Destroy builds", "DestroyBuilds", PermissionStringConstants.DestroyBuilds, BuildPermissions.DestroyBuilds, string.Empty), 
            new Permission(PermissionScope.TeamBuild, "Edit build definition", "EditBuildDefinition", PermissionStringConstants.EditBuildDefinition, BuildPermissions.EditBuildDefinition, string.Empty), 
            new Permission(PermissionScope.TeamBuild, "Edit build quality", "EditBuildQuality", PermissionStringConstants.EditBuildQuality, BuildPermissions.EditBuildQuality, string.Empty), 
            new Permission(PermissionScope.TeamBuild, "Manage build qualities", "ManageBuildQualities", PermissionStringConstants.ManageBuildQualities, BuildPermissions.ManageBuildQualities, string.Empty), 
            new Permission(PermissionScope.TeamBuild, "Manage build queue", "ManageBuildQueue", PermissionStringConstants.ManageBuildQueue, BuildPermissions.ManageBuildQueue, string.Empty), 
            new Permission(PermissionScope.TeamBuild, "Override check-in validation by build", "OverrideBuildCheckInValidation", PermissionStringConstants.OverrideBuildCheckInValidation, BuildPermissions.OverrideBuildCheckInValidation, string.Empty), 
            new Permission(PermissionScope.TeamBuild, "Queue builds", "QueueBuilds", PermissionStringConstants.QueueBuilds, BuildPermissions.QueueBuilds, string.Empty), 
            new Permission(PermissionScope.TeamBuild, "Retain indefinitely", "RetainIndefinitely", PermissionStringConstants.RetainIndefinitely, BuildPermissions.RetainIndefinitely, string.Empty), 
            new Permission(PermissionScope.TeamBuild, "Stop builds", "StopBuilds", PermissionStringConstants.StopBuilds, BuildPermissions.StopBuilds, string.Empty), 
            new Permission(PermissionScope.TeamBuild, "Update build information", "UpdateBuildInformation", PermissionStringConstants.UpdateBuildInformation, BuildPermissions.UpdateBuildInformation, string.Empty), 
            new Permission(PermissionScope.TeamBuild, "View build definition", "ViewBuildDefinition", PermissionStringConstants.ViewBuildDefinition, BuildPermissions.ViewBuildDefinition, string.Empty), 
            new Permission(PermissionScope.TeamBuild, "View builds", "ViewBuilds", PermissionStringConstants.ViewBuilds, BuildPermissions.ViewBuilds, string.Empty), 

            // Work Item Areas
            new Permission(PermissionScope.WorkItemAreas, "Create child nodes", "CreateChildren", PermissionActionIdConstants.CreateChildren, AuthorizationCssNodePermissions.CreateChildren, string.Empty), 
            new Permission(PermissionScope.WorkItemAreas, "Delete this node", "Delete", PermissionActionIdConstants.Delete, AuthorizationCssNodePermissions.Delete, string.Empty), 
            new Permission(PermissionScope.WorkItemAreas, "Edit this node", "GenericWrite", PermissionActionIdConstants.GenericWrite, AuthorizationCssNodePermissions.GenericWrite, string.Empty), 
            new Permission(PermissionScope.WorkItemAreas, "Edit work items in this node", "WorkItemWrite", PermissionActionIdConstants.WorkItemWrite, AuthorizationCssNodePermissions.WorkItemWrite, string.Empty), 
            new Permission(PermissionScope.WorkItemAreas, "Manage test plans", "ManageTestPlans", PermissionActionIdConstants.ManageTestPlans, AuthorizationCssNodePermissions.ManageTestPlans, string.Empty), 
            new Permission(PermissionScope.WorkItemAreas, "View permissions for this node", "GenericRead", PermissionActionIdConstants.GenericRead, AuthorizationCssNodePermissions.GenericRead, string.Empty), 
            new Permission(PermissionScope.WorkItemAreas, "View work items in this node", "WorkItemRead", PermissionActionIdConstants.WorkItemRead, AuthorizationCssNodePermissions.WorkItemRead, string.Empty), 

            // Work Item Iterations
            new Permission(PermissionScope.WorkItemIterations, "Create child nodes", "CreateChildren", PermissionActionIdConstants.CreateChildren, AuthorizationIterationNodePermissions.CreateChildren, string.Empty), 
            new Permission(PermissionScope.WorkItemIterations, "Delete this node", "Delete", PermissionActionIdConstants.Delete, AuthorizationIterationNodePermissions.Delete, string.Empty), 
            new Permission(PermissionScope.WorkItemIterations, "Edit this node", "GenericWrite", PermissionActionIdConstants.GenericWrite, AuthorizationIterationNodePermissions.GenericWrite, string.Empty), 
            new Permission(PermissionScope.WorkItemIterations, "View permissions for this node", "GenericRead", PermissionActionIdConstants.GenericRead, AuthorizationIterationNodePermissions.GenericRead, string.Empty), 

            // Source Control
            new Permission(PermissionScope.SourceControl, "Administer labels", "LabelOther", VersionedItemPermissions.LabelOther.ToString(), (int)VersionedItemPermissions.LabelOther, string.Empty), 
            new Permission(PermissionScope.SourceControl, "Check in", "Checkin", VersionedItemPermissions.Checkin.ToString(), (int)VersionedItemPermissions.Checkin, string.Empty), 
            new Permission(PermissionScope.SourceControl, "Check in other users' changes", "CheckinOther", VersionedItemPermissions.CheckinOther.ToString(), (int)VersionedItemPermissions.CheckinOther, string.Empty), 
            new Permission(PermissionScope.SourceControl, "Check out", "PendChange", VersionedItemPermissions.PendChange.ToString(), (int)VersionedItemPermissions.PendChange, string.Empty), 
            new Permission(PermissionScope.SourceControl, "Label", "Label", VersionedItemPermissions.Label.ToString(), (int)VersionedItemPermissions.Label, string.Empty),                                                                        
            new Permission(PermissionScope.SourceControl, "Lock", "Lock", VersionedItemPermissions.Lock.ToString(), (int)VersionedItemPermissions.Lock, string.Empty), 
            new Permission(PermissionScope.SourceControl, "Manage branch", "ManageBranch", VersionedItemPermissions.ManageBranch.ToString(), (int)VersionedItemPermissions.ManageBranch, string.Empty), 
            new Permission(PermissionScope.SourceControl, "Manage permissions", "AdminProjectRights", VersionedItemPermissions.AdminProjectRights.ToString(), (int)VersionedItemPermissions.AdminProjectRights, string.Empty), 
            new Permission(PermissionScope.SourceControl, "Merge", "Merge", VersionedItemPermissions.Merge.ToString(), (int)VersionedItemPermissions.Merge, string.Empty), 
            new Permission(PermissionScope.SourceControl, "Read", "Read", VersionedItemPermissions.Read.ToString(), (int)VersionedItemPermissions.Read, string.Empty), 
            new Permission(PermissionScope.SourceControl, "Revise other users' changes", "ReviseOther", VersionedItemPermissions.ReviseOther.ToString(), (int)VersionedItemPermissions.ReviseOther, string.Empty), 
            new Permission(PermissionScope.SourceControl, "Undo other users' changes", "UndoOther", VersionedItemPermissions.UndoOther.ToString(), (int)VersionedItemPermissions.UndoOther, string.Empty), 
            new Permission(PermissionScope.SourceControl, "Unlock other users' changes", "UnlockOther", VersionedItemPermissions.UnlockOther.ToString(), (int)VersionedItemPermissions.UnlockOther, string.Empty), 
                                                                      
            // Git Source Control
            new Permission(PermissionScope.GitSourceControl, "Create Branch", "CreateBranch", TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.CreateBranch.ToString(), (int)TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.CreateBranch, string.Empty), 
            new Permission(PermissionScope.GitSourceControl, "Create Repository", "CreateRepository", TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.CreateRepository.ToString(), (int)TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.CreateRepository, string.Empty), 
            new Permission(PermissionScope.GitSourceControl, "Create Tag", "CreateTag", TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.CreateTag.ToString(), (int)TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.CreateTag, string.Empty),
            new Permission(PermissionScope.GitSourceControl, "Delete Repository", "DeleteRepository", TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.DeleteRepository.ToString(), (int)TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.DeleteRepository, string.Empty), 
            new Permission(PermissionScope.GitSourceControl, "Edit Policies", "EditPolicies", TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.EditPolicies.ToString(), (int)TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.EditPolicies, string.Empty), 
            new Permission(PermissionScope.GitSourceControl, "Force Push (change history)", "ForcePush", TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.ForcePush.ToString(), (int)TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.ForcePush, string.Empty),
            new Permission(PermissionScope.GitSourceControl, "Contribute", "GenericContribute", TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.GenericContribute.ToString(), (int)TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.GenericContribute, string.Empty),
            new Permission(PermissionScope.GitSourceControl, "Read", "GenericRead", TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.GenericRead.ToString(), (int)TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.GenericRead, string.Empty),
            new Permission(PermissionScope.GitSourceControl, "Manage Note", "ManageNote", TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.ManageNote.ToString(), (int)TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.ManageNote, string.Empty),
            new Permission(PermissionScope.GitSourceControl, "Manage permissions", "ManagePermissions", TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.ManagePermissions.ToString(), (int)TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.ManagePermissions, string.Empty),
            new Permission(PermissionScope.GitSourceControl, "Remove Others' Locks", "RemoveOthersLocks", TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.RemoveOthersLocks.ToString(), (int)TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.RemoveOthersLocks, string.Empty),
            new Permission(PermissionScope.GitSourceControl, "Rename repository", "RenameRepository", TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.RenameRepository.ToString(), (int)TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.RenameRepository, string.Empty),
            new Permission(PermissionScope.GitSourceControl, "Exempt from policies", "PolicyExempt", TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.PolicyExempt.ToString(), (int)TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions.PolicyExempt, string.Empty),
        };

       #endregion

        #region Public Methods and Operators
        /// <summary>
        /// The get action details by name.
        /// </summary>
        /// <param name="actionNames">
        /// The action names.
        /// </param>
        /// <param name="permissionState">
        /// The permission state.
        /// </param>
        /// <param name="scope">
        /// The scope.
        /// </param>
        /// <returns>IEnumerable of Permission</returns>
        public static IEnumerable<Permission> GetActionDetailsByName(
            string actionNames, 
            string permissionState, 
            PermissionScope scope)
        {
            string[] actions = actionNames.Split(',');
            return (from item in actions
                    select (from constPermission in Permissions
                            where constPermission.InternalName == item.TrimStart() && constPermission.Scope == scope
                            select constPermission).FirstOrDefault()
                    into selectedPermission
                    where selectedPermission != null
                    select
                        new Permission(
                        scope, 
                        selectedPermission.DisplayName, 
                        selectedPermission.InternalName, 
                        selectedPermission.PermissionConstant, 
                        selectedPermission.PermissionId, 
                        permissionState)).ToList();
        }

        /// <summary>
        /// Gets the security namespace identifier.
        /// </summary>
        /// <param name="permissionScope">The permission scope.</param>
        /// <param name="server">The server.</param>
        /// <returns>Guid</returns>
        public static Guid GetSecurityNamespaceId(PermissionScope permissionScope, ISecurityService server)
        {
            var securityNamespaceId = Guid.Empty;
            switch (permissionScope)
            {
                case PermissionScope.TeamProject:
                    securityNamespaceId = server.GetSecurityNamespaces().SingleOrDefault(secuirtyNamespace => secuirtyNamespace.Description.Name == "Project").Description.NamespaceId;
                    break;
                case PermissionScope.Workspaces:
                    securityNamespaceId = server.GetSecurityNamespaces().SingleOrDefault(secuirtyNamespace => secuirtyNamespace.Description.Name == "Workspaces").Description.NamespaceId;
                    break;
                case PermissionScope.WorkItems:
                    securityNamespaceId = server.GetSecurityNamespaces().SingleOrDefault(secuirtyNamespace => secuirtyNamespace.Description.Name == "WorkItemQueryFolders").Description.NamespaceId;
                    break;
                case PermissionScope.TeamBuild:
                    securityNamespaceId = server.GetSecurityNamespaces().SingleOrDefault(secuirtyNamespace => secuirtyNamespace.Description.Name == "Build").Description.NamespaceId;
                    break;
                case PermissionScope.WorkItemAreas:
                    securityNamespaceId = server.GetSecurityNamespaces().SingleOrDefault(secuirtyNamespace => secuirtyNamespace.Description.Name == "CSS").Description.NamespaceId;
                    break;
                case PermissionScope.WorkItemIterations:
                    securityNamespaceId = server.GetSecurityNamespaces().SingleOrDefault(secuirtyNamespace => secuirtyNamespace.Description.Name == "Iteration").Description.NamespaceId;
                    break;
                case PermissionScope.SourceControl:
                    securityNamespaceId = server.GetSecurityNamespaces().SingleOrDefault(secuirtyNamespace => secuirtyNamespace.Description.Name == "VersionControlItems").Description.NamespaceId;
                    break;
                case PermissionScope.GitSourceControl:
                    securityNamespaceId = server.GetSecurityNamespaces().SingleOrDefault(secuirtyNamespace => secuirtyNamespace.Description.Name == "Git Repositories").Description.NamespaceId;
                    break;

                // case PermissionScope.Warehouse:
                //    securityNamespaceId = server.GetSecurityNamespaces().SingleOrDefault(secuirtyNamespace => secuirtyNamespace.Description.Name == "Warehouse").Description.NamespaceId;
                //    break;
                case PermissionScope.Collection:
                    securityNamespaceId = server.GetSecurityNamespaces().SingleOrDefault(secuirtyNamespace => secuirtyNamespace.Description.Name == "CollectionManagement").Description.NamespaceId;
                    break;
            }

            return securityNamespaceId;
        }

        /// <summary>
        /// Accesses the control entry to permission.
        /// </summary>
        /// <param name="securityNamespace">The security namespace.</param>
        /// <param name="accessControlEntries">The access control entries.</param>
        /// <param name="appendGroupInheritance">if set to <c>true</c> [append group inheritance].</param>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>List of Permissions</returns>
        public static List<Permission> AccessControlEntryToPermission(SecurityNamespace securityNamespace, IEnumerable<Microsoft.TeamFoundation.Framework.Client.AccessControlEntry> accessControlEntries, bool appendGroupInheritance, string groupName)
        {
            var permissionsList = new List<Permission>();
            foreach (Microsoft.TeamFoundation.Framework.Client.AccessControlEntry ace in accessControlEntries)
            {
                if (0 != ace.Allow)
                {
                    foreach (var action in securityNamespace.Description.Actions)
                    {
                        if ((ace.Allow & action.Bit) == action.Bit)
                        {
                            permissionsList.Add(new Permission { DisplayName = action.DisplayName, InternalName = action.Name, Scope = PermissionScope.TeamProject, PermissionConstant = action.Name, PermissionId = 0, Value = "Allow" });
                        }
                    }
                }

                if (0 != ace.Deny)
                {
                    foreach (var action in securityNamespace.Description.Actions)
                    {
                        if ((ace.Deny & action.Bit) == action.Bit)
                        {
                            permissionsList.Add(new Permission { DisplayName = action.DisplayName, InternalName = action.Name, Scope = PermissionScope.TeamProject, PermissionConstant = action.Name, PermissionId = 0, Value = "Deny" });
                        }
                    }
                }
            }

            if (appendGroupInheritance)
            {
                AppendGroupInheritanceInformation(permissionsList, groupName);
            }

            return permissionsList;
        }

        /// <summary>
        /// Removes the duplicate permissions in the list and combine them if it is possible
        /// </summary>
        /// <param name="permissions">Permission list to process</param>
        /// <returns>A simplified permission list</returns>
        public static List<Permission> RemoveDuplicatePermissionsAndCombineGroups(List<Permission> permissions)
        {
            List<Permission> permissionsList = new List<Permission>();

            foreach (var permission in permissions)
            {
                var duplicatePermissions = (from dupPermission in permissions
                                            where dupPermission.InternalName == permission.InternalName
                                            select dupPermission).ToList();
                if (duplicatePermissions.Count > 1)
                {
                    // Get inhirted group names
                    string[] groupNames = new string[duplicatePermissions.Count];
                    int counter = 0;
                    foreach (var item in duplicatePermissions)
                    {
                        groupNames[counter] = item.GroupMemberInheritance;
                        counter++;
                    }

                    Permission modifiedPermission = duplicatePermissions[0];
                    if (!permissionsList.Exists(x => x.PermissionConstant == modifiedPermission.PermissionConstant))
                    {
                        if (modifiedPermission.GroupMemberInheritance != null)
                        {
                            modifiedPermission.GroupMemberInheritance = string.Join(",", groupNames);
                        }

                        // Add only one Item
                        permissionsList.Add(modifiedPermission);
                    }
                }
                else
                {
                    permissionsList.Add(permission);
                }
            }

            return permissionsList;
        }

        /// <summary>
        /// Add inheritance info in the permission list
        /// </summary>
        /// <param name="permissions">Permission list to alter</param>
        /// <param name="groupName">Group inheritance to add</param>
        public static void AppendGroupInheritanceInformation(IEnumerable<Permission> permissions, string groupName)
        {
            foreach (Permission permission in permissions)
            {
                permission.GroupMemberInheritance = groupName;
                permission.Value = permission.Value.Insert(0, "Inherited ");
            }
        }

        /// <summary>
        /// Returns a flat list of all the nodes in the input collection.
        /// </summary>
        /// <param name="nodes">input node list</param>
        /// <returns> An iterator on the flat node list</returns>
        public static IEnumerable<Node> FlattenTree(NodeCollection nodes)
        {
            foreach (Node node in nodes)
            {
                yield return node;
                if (node.HasChildNodes)
                {
                    foreach (var childNode in FlattenTree(node.ChildNodes))
                    {
                        yield return childNode;
                    }
                }
            }
        }

        /// <summary>
        /// Insert the userName into the filename pattern
        /// </summary>
        /// <param name="userName">account name</param>
        /// <param name="fileName">filename pattern</param>
        /// <returns>Consolidated file name</returns>
        public static string GenerateFileName(string userName, string fileName)
        {
            var rootPart = Path.GetFileNameWithoutExtension(fileName);
            var userNamePart = Regex.Replace(userName, @"\W", "_");
            var result = Path.Combine(Path.GetDirectoryName(fileName), string.Format("{0}_{1}{2}", rootPart, userNamePart, Path.GetExtension(fileName)));
            return result;
        }
        #endregion
    }
}