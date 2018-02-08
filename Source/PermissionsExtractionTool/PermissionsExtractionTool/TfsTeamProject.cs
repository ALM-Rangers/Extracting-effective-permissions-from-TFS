// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TfsTeamProject.cs" company="Microsoft Corporation">
//   Microsoft Visual Studio ALM Rangers
// </copyright>
// <summary>
//   The tfs team project.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.PermissionsExtractionTool
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The team project.
    /// </summary>
    [XmlRoot(ElementName = "TeamProject")]
    public class TfsTeamProject
    {
        ////Public Properties

        /// <summary>
        /// Gets or sets the area permissions.
        /// </summary>
        public List<AreaPermission> AreaPermissions { get; set; }

        /// <summary>
        /// Gets or sets the build permissions.
        /// </summary>
        public List<Permission> BuildPermissions { get; set; }

        /// <summary>
        /// Gets or sets the iteration permissions.
        /// </summary>
        public List<IterationPermission> IterationPermissions { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the project level permissions.
        /// </summary>
        [XmlElement(ElementName = "ProjectLevelPermissions")]
        public ProjectLevelPermissions ProjectLevelPermissions { get; set; }

        /// <summary>
        /// Gets or sets the version control permissions.
        /// </summary>
        [XmlElement(ElementName = "VersionControlPermissions")]
        public VersionControlPermissions VersionControlPermissions { get; set; }

        /// <summary>
        /// Gets or sets the version control permissions.
        /// </summary>
        [XmlElement(ElementName = "GitVersionControlPermissions")]
        public GitVersionControlPermissions GitVersionControlPermissions { get; set; }

        /// <summary>
        /// Gets or sets the work items permissions.
        /// </summary>
        public List<Permission> WorkItemsPermissions { get; set; }
    }
}