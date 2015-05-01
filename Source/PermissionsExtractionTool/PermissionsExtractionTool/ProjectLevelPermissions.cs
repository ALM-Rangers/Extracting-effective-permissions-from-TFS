// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectLevelPermissions.cs" company="Microsoft Corporation">
//   Microsoft Visual Studio ALM Rangers
// </copyright>
// <summary>
//   The project level permissions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.PermissionsExtractionTool
{
    using System.Collections.Generic;

    /// <summary>
    /// The project level permissions.
    /// </summary>
    public class ProjectLevelPermissions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the project level permissions list.
        /// </summary>
        public List<Permission> ProjectLevelPermissionsList { get; set; }

        #endregion
    }
}