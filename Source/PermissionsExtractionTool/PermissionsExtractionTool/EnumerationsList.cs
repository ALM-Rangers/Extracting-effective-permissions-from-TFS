// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerationsList.cs" company="Microsoft Corporation">
//   Microsoft Visual Studio ALM Rangers
// </copyright>
// <summary>
//   Defines the EnumerationsList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.PermissionsExtractionTool
{
    using System;

    /// <summary>
    ///    Enumerations List
    /// </summary>
    public static class EnumerationsList
    {
        #region EnumerationsList
        /// <summary>
        /// Git Version Control Permissions. Derived from Microsoft.TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions
        /// version 15 but only includes the individual flag values, not the multi-bit values, to make it easier to select flag
        /// names from a bitmask.
        /// </summary>
        [Flags]
        public enum GitPermissions
        {
            None = 0x0,
            Administer = 0x1,
            GenericRead = 0x2,
            GenericContribute = 0x4,
            ForcePush = 0x8,
            CreateBranch = 0x10,
            CreateTag = 0x20,
            ManageNote = 0x40,
            PolicyExempt = 0x80,
            CreateRepository = 0x0100,
            DeleteRepository = 0x0200,
            RenameRepository = 0x0400,
            EditPolicies = 0x0800,
            RemoveOthersLocks = 0x1000,
            ManagePermissions = 0x2000,
            PullRequestContribute = 0x4000,
            Unknown0x8000 = 0x8000,  // Just in case
            Unknown0x10000 = 0x10000,  // Just in case
        }

        /// <summary>
        ///     The build permissions.
        /// </summary>
        [Flags]
        public enum BuildPermissions
        {
            // derived from Microsoft.TeamFoundation.Build.Common.BuildPermissions, Microsoft.TeamFoundation.Build.Common, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a

            /// <summary>
            ///     The none.
            /// </summary>
            None = 0x0, 

            /// <summary>
            ///     The view builds.
            /// </summary>
            ViewBuilds = 0x1, 

            /// <summary>
            ///     The edit build quality.
            /// </summary>
            EditBuildQuality = 0x2, 

            /// <summary>
            ///     The retain indefinitely.
            /// </summary>
            RetainIndefinitely = 0x4, 

            /// <summary>
            ///     The delete builds.
            /// </summary>
            DeleteBuilds = 0x8, 

            /// <summary>
            ///     The manage build qualities.
            /// </summary>
            ManageBuildQualities = 0x10, 

            /// <summary>
            ///     The destroy builds.
            /// </summary>
            DestroyBuilds = 0x20, 

            /// <summary>
            ///     The update build information.
            /// </summary>
            UpdateBuildInformation = 0x40, 

            /// <summary>
            ///     The queue builds.
            /// </summary>
            QueueBuilds = 0x80, 

            /// <summary>
            ///     The manage build queue.
            /// </summary>
            ManageBuildQueue = 0x100, 

            /// <summary>
            ///     The stop builds.
            /// </summary>
            StopBuilds = 0x200, 

            /// <summary>
            ///     The view build definition.
            /// </summary>
            ViewBuildDefinition = 0x400, 

            /// <summary>
            ///     The edit build definition.
            /// </summary>
            EditBuildDefinition = 0x800, 

            /// <summary>
            ///     The delete build definition.
            /// </summary>
            DeleteBuildDefinition = 0x1000, 

            /// <summary>
            ///     The override build check in validation.
            /// </summary>
            OverrideBuildCheckInValidation = 0x2000, 

            /// <summary>
            ///     The all permissions.
            /// </summary>
            AllPermissions = 0x3FFF
        }

        /// <summary>
        /// PermissionState
        /// </summary>
        public enum PermissionState
        {
            /// <summary>
            /// Allow
            /// </summary>
            Allow,

            /// <summary>
            /// Deny
            /// </summary>
            Deny,

            /// <summary>
            /// InheritedAllow
            /// </summary>
            InheritedAllow,

            /// <summary>
            /// InheritedDeny
            /// </summary>
            InheritedDeny
        }

        #endregion
    }
}