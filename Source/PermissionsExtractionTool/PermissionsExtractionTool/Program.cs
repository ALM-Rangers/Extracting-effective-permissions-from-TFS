// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft Corporation">
//   Microsoft Visual Studio ALM Rangers
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.TeamFoundation.Git.Client;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Microsoft.ALMRangers.PermissionsExtractionTool
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using CommandLine;
    using Microsoft.ALMRangers.PermissionsExtractionTool.Properties;
    using TeamFoundation.Client;
    using TeamFoundation.Framework.Client;
    using TeamFoundation.Framework.Common;
    using TeamFoundation.VersionControl.Client;
    using TeamFoundation.WorkItemTracking.Client;
    using XmlTransformation;

    /// <summary>
    /// The program.
    /// </summary>
    internal static class Program
    {       
        /// <summary>
        /// Sends a failure message of the ERROR output and wait for a press on the enter key.
        /// </summary>
        /// <param name="message">Message about the failure</param>
        /// <param name="ex">Optional exception to trace to stderr.</param>
        private static void Fail(string message = null, Exception ex = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Console.Error.WriteLine(message);
            }

            if (ex != null)
            {
                Console.Error.WriteLine(ex.ToString());
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Console entry point
        /// </summary>
        /// <param name="args">command line arguments</param>
        /// <returns>0 if all extractions are successful.</returns>
        private static int Main(string[] args)
        {
            var options = new Options();

            var parser = new Parser(settings =>
            {
                settings.CaseSensitive = false;
                settings.HelpWriter = Console.Error;
                settings.ParsingCulture = CultureInfo.InvariantCulture;
            });

            var result = parser.ParseArguments(args, options);

            if (!result)
            {
                Fail();
                return -1;
            }

            TfsTeamProjectCollection tfs =
                TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(options.Collection));
            try
            {
                tfs.EnsureAuthenticated();
            }
            catch (Exception ex)
            {
                Fail("Connection to TFS failed", ex);
                return -1;
            }

            // Getting Identity Service
            var ims = tfs.GetService<IIdentityManagementService>();

            var someExtractionFail = false;
            foreach (var userName in options.Users)
            {
                Console.WriteLine("===== Extracting Permissions for User {0} ======", userName);
                var fileName = Helpers.GenerateFileName(userName, options.OutputFile);
                var extractionStatus = ExtractPermissionForUserName(ims, userName, options, fileName, tfs);

                if (!extractionStatus)
                {
                    someExtractionFail = true;
                }
            }

            if (someExtractionFail)
            {
                Fail("An error occured during the extraction");
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Run the extraction algorithm for a specific user
        /// </summary>
        /// <param name="ims">Identity management service</param>
        /// <param name="userName">user name</param>
        /// <param name="options">command line parameters</param>
        /// <param name="fileName">File name</param>
        /// <param name="tfs">team project collection</param>
        /// <returns>true if successful</returns>
        private static bool ExtractPermissionForUserName(IIdentityManagementService ims, string userName, Options options, string fileName, TfsTeamProjectCollection tfs)
        {
            TeamFoundationIdentity userIdentity = ims.ReadIdentity(
                IdentitySearchFactor.AccountName,
                userName,
                MembershipQuery.None,
                ReadIdentityOptions.IncludeReadFromSource);

            if (userIdentity == null)
            {
                Console.WriteLine("User {0} can't connect to the Collection {1}. Please verifiy!", userName, options.Collection);
                Console.ReadLine();
                return false;
            }

            // get workItem store
            var workItemStore = tfs.GetService<WorkItemStore>();

            ////Initiate Report

            // Initiate a new object of Permission Report
            var permissionsReport = new PermissionsReport
            {
                Date = DateTime.Now,
                TfsCollectionUrl = options.Collection,
                UserName = userName,
                TeamProjects = new List<TfsTeamProject>()
            };

            try
            {
                // retrieve list of Team Projects in the given collection
                ProjectCollection workItemsProjects = workItemStore.Projects;
                foreach (Project teamProject in workItemsProjects)
                {
                    if (options.Projects != null)
                    {
                        if (!options.Projects.Contains(teamProject.Name))
                        {
                            Console.WriteLine(" ...skipping Team Project: {0}", teamProject.Name);
                            continue;
                        }
                    }

                    // Create project security token
                    string projectSecurityToken = "$PROJECT:" + teamProject.Uri.AbsoluteUri;

                    // Project Permissions   
                    var server = tfs.GetService<ISecurityService>();
                    var vcs = tfs.GetService<VersionControlServer>();
                    TeamFoundation.Git.Client.GitRepositoryService gitRepostoryService = tfs.GetService<TeamFoundation.Git.Client.GitRepositoryService>();

                    Console.WriteLine("==== Extracting Permissions for {0} Team Project ====", teamProject.Name);
                    List<string> groups = GetUserGroups(tfs, teamProject.Uri.AbsoluteUri, userIdentity);
                    List<Permission> projectLevelPermissions = ExtractGenericSecurityNamespacePermissions(server, PermissionScope.TeamProject, userIdentity, projectSecurityToken, ims, groups);

                    // Version Control Permissions
                    List<Permission> versionControlPermissions = ExtractVersionControlPermissions(server, groups, userIdentity, teamProject.Name, ims, vcs);
                    List<GitPermission> gitVersionControlPermissions = ExtractGitVersionControlPermissions(server, groups, userIdentity, teamProject.Name, ims, vcs, gitRepostoryService);

                    // Build Permissions
                    List<Permission> buildPermissions = ExtractBuildPermissions(server, projectSecurityToken, userIdentity);

                    // WorkItems Area Permissions
                    List<AreaPermission> areasPermissions = ExtractAreasPermissions(server, teamProject, userIdentity, ims, groups);

                    // WorkItems Iteration Permissions
                    List<IterationPermission> iterationPermissions = ExtractIterationPermissions(server, teamProject, userIdentity, ims, groups);

                    // Workspace Permissions
                    // var workspacePermission = ExtractGenericSecurityNamespacePermissions(server, PermissionScope.Workspaces, userIdentity, projectSecurityToken, ims, groups);

                    // Set TFS report Data
                    // Create Team Project node in XML file
                    var tfsTeamProject = new TfsTeamProject
                    {
                        Name = teamProject.Name,
                        AreaPermissions = areasPermissions,
                        BuildPermissions = buildPermissions,
                        IterationPermissions = iterationPermissions,
                        ProjectLevelPermissions =
                            new ProjectLevelPermissions
                            {
                                ProjectLevelPermissionsList
                                    =
                                    projectLevelPermissions
                            },
                        GitVersionControlPermissions = new GitVersionControlPermissions
                        {
                            VersionControlPermissionsList = gitVersionControlPermissions
                        },
                        VersionControlPermissions = new VersionControlPermissions
                        {
                            VersionControlPermissionsList = versionControlPermissions
                        }
                    };

                    tfsTeamProject.VersionControlPermissions.VersionControlPermissionsList.AddRange(versionControlPermissions);
                    permissionsReport.TeamProjects.Add(tfsTeamProject);
                }

                // Generate output file
                FileInfo fi = new FileInfo(fileName);
                if (!Directory.Exists(fi.DirectoryName))
                {
                    Console.Write("Creating Output Directory {0}", fi.DirectoryName);
                    Directory.CreateDirectory(fi.DirectoryName);
                }

                var fs = new FileStream(fileName, FileMode.Create);
                var streamWriter = new StreamWriter(fs, Encoding.UTF8);

                using (streamWriter)
                {
                    var xmlSerializer = new XmlSerializer(typeof(PermissionsReport));
                    xmlSerializer.Serialize(streamWriter, permissionsReport);
                    streamWriter.Flush();
                }

                if (options.Html)
                {
                    var tranformationFileName = Path.Combine(Path.GetDirectoryName(fileName), "ALMRanger.xsl");
                    File.WriteAllText(tranformationFileName, Resources.ALMRangers_SampleXslt);
                    var htmlOuput = Path.ChangeExtension(fileName, ".html");

                    var logoFile = Path.Combine(Path.GetDirectoryName(fileName), "ALMRangers_Logo.png");
                    Resources.ALMRangers_Logo.Save(logoFile);
                    XmlTransformationManager.TransformXmlUsingXsl(fileName, tranformationFileName, htmlOuput);
                }

                return true;
            }
            catch (TeamFoundationServiceException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Extracts the generic security namespace permissions.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="permissionScope">The permission scope.</param>
        /// <param name="userIdentity">The user identity.</param>
        /// <param name="securityToken">The security token.</param>
        /// <param name="identityManagementService">The identityManagementService.</param>
        /// <param name="groups">The groups.</param>
        /// <returns>List of Permissions</returns>
        private static List<Permission> ExtractGenericSecurityNamespacePermissions(ISecurityService server, PermissionScope permissionScope, TeamFoundationIdentity userIdentity, string securityToken, IIdentityManagementService identityManagementService, IEnumerable<string> groups)
        {
            SecurityNamespace genericSecurityNamespace = server.GetSecurityNamespace(Helpers.GetSecurityNamespaceId(permissionScope, server));

            var result = new List<Permission>();
            try
            {
                AccessControlList userAccessList =
                    genericSecurityNamespace.QueryAccessControlList(
                        securityToken,
                        new List<IdentityDescriptor> {userIdentity.Descriptor},
                        true);
                result.AddRange(Helpers.AccessControlEntryToPermission(genericSecurityNamespace,
                    userAccessList.AccessControlEntries, false, string.Empty));

                // handle group inheritance 
                foreach (string group in groups)
                {
                    TeamFoundationIdentity groupIdentity = identityManagementService.ReadIdentity(
                        IdentitySearchFactor.Identifier, group, MembershipQuery.None,
                        ReadIdentityOptions.IncludeReadFromSource);

                    AccessControlList groupAccessList =
                        genericSecurityNamespace.QueryAccessControlList(
                            securityToken,
                            new List<IdentityDescriptor> {groupIdentity.Descriptor},
                            true);
                    result.AddRange(Helpers.AccessControlEntryToPermission(genericSecurityNamespace,
                        groupAccessList.AccessControlEntries, true, groupIdentity.DisplayName));
                }
            }
            catch (TeamFoundationServiceException ex)
            {
                Console.Error.WriteLine("ERROR: " + ex.Message);
            }

            var modifiedPermissions = Helpers.RemoveDuplicatePermissionsAndCombineGroups(result);
            return modifiedPermissions;
        }

        /// <summary>
        /// Extract iteration permissions for a specific identity
        /// </summary>
        /// <param name="server">Server security service</param>
        /// <param name="teamProject">Team project</param>
        /// <param name="userIdentity">User identity</param>
        /// <param name="identityManagementService">The identityManagementService</param> 
        /// <param name="groups">List of groups</param>
        /// <returns>Security namespace</returns>
        private static List<IterationPermission> ExtractIterationPermissions(ISecurityService server, Project teamProject, TeamFoundationIdentity userIdentity, IIdentityManagementService identityManagementService, List<string> groups)
        {
            var result = new List<IterationPermission>();
            var lstIterations = Helpers.FlattenTree(teamProject.IterationRootNodes);

            // root Area Node
            var iterationPermissionRoot = new IterationPermission { IterationName = teamProject.Name, IterationPermissions = new List<Permission>() };
            if (lstIterations.Any())
            { 
                iterationPermissionRoot.IterationPermissions.AddRange(ExtractGenericSecurityNamespacePermissions(server, PermissionScope.WorkItemIterations, userIdentity, lstIterations.First().ParentNode.Uri.AbsoluteUri, identityManagementService, groups));
            }
            if (iterationPermissionRoot.IterationPermissions.Count > 0)
            {
                result.Add(iterationPermissionRoot);
            }

            Console.WriteLine("== Extract WorkItems Iteration Permissions ==");
            foreach (Node iteration in lstIterations)
            {
                var iterationPermission = new IterationPermission
                {
                    IterationName = iteration.Path,
                    IterationPermissions = new List<Permission>()
                };
                iterationPermission.IterationPermissions.AddRange(ExtractGenericSecurityNamespacePermissions(server, PermissionScope.WorkItemIterations, userIdentity, iteration.Uri.AbsoluteUri, identityManagementService, groups));

                if (iterationPermission.IterationPermissions.Count > 0)
                {
                    result.Add(iterationPermission);
                }
            }

            return result;
        }

        /// <summary>
        /// Extracts area permission for a specific identity
        /// </summary>
        /// <param name="server">Server security service</param>
        /// <param name="teamProject">Team project</param>
        /// <param name="userIdentity">User identity</param>
        /// <param name="identityManagementService">The identityManagementService</param>
        /// <param name="groups">List of groups</param>
        /// <returns>The list of the permissions</returns>
        private static List<AreaPermission> ExtractAreasPermissions(ISecurityService server, Project teamProject, TeamFoundationIdentity userIdentity, IIdentityManagementService identityManagementService, List<string> groups)
        {
            var result = new List<AreaPermission>();
            Console.WriteLine("== Extract WorkItems Area Permissions ==");

            // root Area Node
            var areaPermissionRoot = new AreaPermission { AreaName = teamProject.Name, AreaPermissions = new List<Permission>() };
            areaPermissionRoot.AreaPermissions.AddRange(ExtractGenericSecurityNamespacePermissions(server, PermissionScope.WorkItemAreas, userIdentity, teamProject.AreaRootNodeUri.AbsoluteUri, identityManagementService, groups));
            if (areaPermissionRoot.AreaPermissions.Count > 0)
            {
                result.Add(areaPermissionRoot);
            }

            IEnumerable<Node> areaList = Helpers.FlattenTree(teamProject.AreaRootNodes);
            foreach (Node area in areaList)
            {
                var areaPermission = new AreaPermission { AreaName = area.Path, AreaPermissions = new List<Permission>() };
                areaPermission.AreaPermissions.AddRange(ExtractGenericSecurityNamespacePermissions(server, PermissionScope.WorkItemAreas, userIdentity, area.Uri.AbsoluteUri, identityManagementService, groups));
                if (areaPermission.AreaPermissions.Count > 0)
                {
                    Console.WriteLine("  -- Adding Permissions for {0}", area.Path);
                    result.Add(areaPermission);
                }
            }

            return result;
        }

        /////// <summary>
        /////// Extracts work item permissions for a specific identity
        /////// </summary>
        /////// <param name="server">Server security service</param>
        /////// <param name="projectSecurityToken">Project Security Token</param>
        /////// <param name="userIdentity">User identity</param>
        /////// <returns>The list of the permissions</returns>
        ////private static List<Permission> ExtractWorkItemsPermissions(
        ////    ISecurityService server,
        ////    string projectSecurityToken,
        ////    TeamFoundationIdentity userIdentity)
        ////{
        ////    var result = new List<Permission>();
        ////    SecurityNamespace workItemsSecurityNamespace = server.GetSecurityNamespace(Helpers.GetSecurityNamespaceId(PermissionScope.WorkItemAreas, server));
        ////    if (workItemsSecurityNamespace != null)
        ////    {
        ////        Console.WriteLine("== Extract WorkItems Permissions ==");
        ////        AccessControlList workItemsAccessList = workItemsSecurityNamespace.QueryAccessControlList(
        ////            projectSecurityToken,
        ////            new List<IdentityDescriptor> { userIdentity.Descriptor },
        ////            true);
        ////        string allowedWorkItemPermissions = string.Empty;
        ////        string denyWorkItemPermissions = string.Empty;
        ////        foreach (AccessControlEntry ace in workItemsAccessList.AccessControlEntries)
        ////        {
        ////            if (0 != ace.Allow)
        ////            {
        ////                allowedWorkItemPermissions = ((QueryItemPermissions)ace.Allow).ToString();
        ////                result.AddRange(
        ////                    Helpers.GetActionDetailsByName(allowedWorkItemPermissions, "Allow", PermissionScope.WorkItemAreas));
        ////            }

        ////            if (0 != ace.Deny)
        ////            {
        ////                denyWorkItemPermissions = ((QueryItemPermissions)ace.Deny).ToString();
        ////                result.AddRange(
        ////                    Helpers.GetActionDetailsByName(denyWorkItemPermissions, "Deny", PermissionScope.WorkItemAreas));
        ////            }
        ////        }
        ////    }

        ////    return result;
        ////}
        
        /// <summary>
        /// Extracts build permissions for a specific identity.
        /// </summary>
        /// <param name="server">Server security service</param>
        /// <param name="projectSecurityToken">Project Security Token</param>
        /// <param name="userIdentity">User identity</param>
        /// <returns>The list of the permissions</returns>
        private static List<Permission> ExtractBuildPermissions(ISecurityService server, string projectSecurityToken, TeamFoundationIdentity userIdentity)
        {
            var result = new List<Permission>();
            SecurityNamespace buildSecurityNamespace = server.GetSecurityNamespace(Helpers.GetSecurityNamespaceId(PermissionScope.TeamBuild, server));
            Console.WriteLine("== Extract Build Permissions ==");
            AccessControlList buildAccessList = buildSecurityNamespace.QueryAccessControlList(
                projectSecurityToken,
                new List<IdentityDescriptor> { userIdentity.Descriptor },
                true);

            foreach (AccessControlEntry ace in buildAccessList.AccessControlEntries)
            {
                foreach (AclAndName aclAndName in Helpers.EnumerateAcls(ace, false))
                {
                    if (aclAndName.Acl != 0)
                    {
                        var allowedBuildPermissions = ((EnumerationsList.BuildPermissions) ace.Allow).ToString();
                        result.AddRange(
                            Helpers.GetActionDetailsByName(allowedBuildPermissions, aclAndName.Name,
                                PermissionScope.TeamBuild));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Extracts version control permissions for a specific identity.
        /// </summary>
        /// <param name="server">TFS Security service</param>
        /// <param name="groups">Security groups</param>
        /// <param name="userIdentity">User Identity</param>
        /// <param name="projectSecurityToken">Project Security Token</param>
        /// <param name="ims">TFS Identity service</param>
        /// <param name="vcs">TFS VC Service</param>
        /// <returns>The list of the permissions</returns>
        private static List<Permission> ExtractVersionControlPermissions(
            ISecurityService server,
            IEnumerable<string> groups,
            TeamFoundationIdentity userIdentity,
            string projectSecurityToken,
            IIdentityManagementService ims,
            VersionControlServer vcs)
        {
            Console.WriteLine("== Extract Version Control Permissions ==");
            TeamProject teamProject = vcs.TryGetTeamProject(projectSecurityToken);
            if (teamProject == null)
            {
                Console.WriteLine("Could not get team project object from TFS/VSTS");
                return new List<Permission>();
            }

            return ExtractGenericSecurityNamespacePermissions(server, PermissionScope.SourceControl, userIdentity, teamProject.ServerItem, ims, groups);
        }

        /// <summary>
        /// Sample method on how to extracts the git version control permissions.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="groups">The groups.</param>
        /// <param name="userIdentity">The user identity.</param>
        /// <param name="projectSecurityToken">The project security token.</param>
        /// <param name="identityManagementService">The identityManagementService.</param>
        /// <param name="vcs">The VCS.</param>
        /// <param name="gitService">The git service.</param>
        /// <returns>List of Permissions</returns>
        private static List<GitPermission> ExtractGitVersionControlPermissions(
            ISecurityService server,
            List<string> groups,
            TeamFoundationIdentity userIdentity,
            string projectSecurityToken,
            IIdentityManagementService identityManagementService,
            VersionControlServer vcs,
            GitRepositoryService gitService)
        {
            Console.WriteLine("== Extract Git Version Control Permissions ==");
            SecurityNamespace gitVersionControlSecurityNamespace = server.GetSecurityNamespace(Helpers.GetSecurityNamespaceId(PermissionScope.GitSourceControl, server));
            IList<GitRepository> gitProjectRepoService = gitService.QueryRepositories(projectSecurityToken);

            var groupsIdentities = new List<TeamFoundationIdentity>(groups.Count);
            foreach (string group in groups)
            {
                TeamFoundationIdentity groupIdentity = identityManagementService.ReadIdentity(
                    IdentitySearchFactor.Identifier, group, MembershipQuery.None,
                    ReadIdentityOptions.IncludeReadFromSource);
                groupsIdentities.Add(groupIdentity);
            }

            var results = new List<GitPermission>();

            foreach (GitRepository repo in gitProjectRepoService)
            {
                string primaryBranch = (repo.DefaultBranch ?? "(no default branch)").StartsWith("refs/heads/") ?
                    repo.DefaultBranch.Substring(11) :
                    repo.DefaultBranch;

                Console.WriteLine("  -- Repo: {0} branch {1}", repo.Name, primaryBranch);

                // Repository Security Token is repoV2/TeamProjectId/RepositoryId
                string repoIdToken = string.Format("repoV2{0}{1}{2}{3}",
                    gitVersionControlSecurityNamespace.Description.SeparatorValue,
                    repo.ProjectReference.Id,
                    gitVersionControlSecurityNamespace.Description.SeparatorValue,
                    repo.Id);

                AccessControlList versionControlAccessList =
                    gitVersionControlSecurityNamespace.QueryAccessControlList(
                        repoIdToken,
                        new List<IdentityDescriptor> {userIdentity.Descriptor},
                        true);

                var gitVersionControlPermissions = new List<Permission>();
                foreach (AccessControlEntry ace in versionControlAccessList.AccessControlEntries)
                {
                    foreach (AclAndName aclAndName in Helpers.EnumerateAcls(ace, false))
                    {
                        if (aclAndName.Acl != 0)
                        {
                            string versionControlPermissions = ((EnumerationsList.GitPermissions) aclAndName.Acl).ToString();
                            gitVersionControlPermissions.AddRange(
                                Helpers.GetActionDetailsByName(versionControlPermissions, aclAndName.Name,
                                    PermissionScope.GitSourceControl));
                        }
                    }
                }

                foreach (TeamFoundationIdentity groupIdentity in groupsIdentities)
                {
                    AccessControlList groupAccessList =
                        gitVersionControlSecurityNamespace.QueryAccessControlList(
                            repoIdToken,
                            new List<IdentityDescriptor> {groupIdentity.Descriptor},
                            true);

                    foreach (AccessControlEntry ace in groupAccessList.AccessControlEntries)
                    {
                        foreach (AclAndName aclAndName in Helpers.EnumerateAcls(ace, true))
                        {
                            if (aclAndName.Acl != 0)
                            {
                                string versionControlPermissions = ((EnumerationsList.GitPermissions)aclAndName.Acl).ToString();
                                gitVersionControlPermissions.AddRange(
                                    Helpers.GetActionDetailsByName(versionControlPermissions, aclAndName.Name,
                                        PermissionScope.GitSourceControl));
                            }
                        }
                    }
                }

                List<Permission> modifiedPermissions = Helpers.RemoveDuplicatePermissionsAndCombineGroups(gitVersionControlPermissions);
                results.AddRange(modifiedPermissions.Select(perm => new GitPermission(perm, repo.Name, primaryBranch)));
            }

            return results;
        }

        /// <summary>
        /// Returns the security group of the user
        /// </summary>
        /// <param name="tpc">Team project collection</param>
        /// <param name="teamProjectUri">Team Project Uri</param>
        /// <param name="userIdentity">User identity</param>
        /// <returns>The groups where the user is present.</returns>
        private static List<string> GetUserGroups(TfsTeamProjectCollection tpc, string teamProjectUri, TeamFoundationIdentity userIdentity)
        {
            Console.WriteLine("  -- Getting UserGroups...");
            List<string> groups = new List<string>();

            // Get the group security service.
            var gss = tpc.GetService<Microsoft.TeamFoundation.Server.IGroupSecurityService2>();

            Microsoft.TeamFoundation.Server.Identity[] appGroups = gss.ListApplicationGroups(teamProjectUri);

            foreach (Microsoft.TeamFoundation.Server.Identity group in appGroups)
            {
                Microsoft.TeamFoundation.Server.Identity[] groupMembers = gss.ReadIdentities(Microsoft.TeamFoundation.Server.SearchFactor.Sid, new[] { group.Sid }, Microsoft.TeamFoundation.Server.QueryMembership.Expanded);
                foreach (Microsoft.TeamFoundation.Server.Identity member in groupMembers)
                {
                    if (member.Members != null)
                    {
                        groups.AddRange(from memberSid in member.Members where memberSid == userIdentity.Descriptor.Identifier select @group.Sid);
                    }
                }
            }

            return groups;
        }
    }
}
