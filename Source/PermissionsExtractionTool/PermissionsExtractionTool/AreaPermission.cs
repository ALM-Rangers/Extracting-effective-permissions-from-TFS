// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AreaPermission.cs" company="Microsoft Corporation">
//   Microsoft Visual Studio ALM Rangers
// </copyright>
// <summary>
//   The area permission.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.PermissionsExtractionTool
{
    using System.Collections.Generic;

    /// <summary>
    /// The area permission.
    /// </summary>
    public class AreaPermission
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the area name.
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// Gets or sets the area permissions.
        /// </summary>
        public List<Permission> AreaPermissions { get; set; }

        #endregion
    }
}