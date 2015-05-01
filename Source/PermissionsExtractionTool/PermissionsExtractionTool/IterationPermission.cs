// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationPermission.cs" company="Microsoft Corporation">
//   Microsoft Visual Studio ALM Rangers
// </copyright>
// <summary>
//   The iteration permission.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.PermissionsExtractionTool
{
    using System.Collections.Generic;

    /// <summary>
    /// The iteration permission.
    /// </summary>
    public class IterationPermission
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the iteration name.
        /// </summary>
        public string IterationName { get; set; }

        /// <summary>
        /// Gets or sets the iteration permissions.
        /// </summary>
        public List<Permission> IterationPermissions { get; set; }

        #endregion
    }
}