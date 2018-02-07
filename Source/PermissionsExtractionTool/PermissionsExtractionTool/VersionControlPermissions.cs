// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionControlPermissions.cs" company="Microsoft Corporation">
//   Microsoft Visual Studio ALM Rangers
// </copyright>
// <summary>
//   The version control permissions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.PermissionsExtractionTool
{
    using System.Collections.Generic;

    /// <summary>
    /// The version control permissions.
    /// </summary>
    public class VersionControlPermissions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the version control permissions list.
        /// </summary>
        public List<Permission> VersionControlPermissionsList { get; set; }

        #endregion
    }

    /// <summary>
    /// The version control permissions.
    /// </summary>
    public class GitVersionControlPermissions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the version control permissions list.
        /// </summary>
        public List<GitPermission> VersionControlPermissionsList { get; set; }

        #endregion
    }
}