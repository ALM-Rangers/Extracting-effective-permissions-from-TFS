// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionScope.cs" company="Microsoft Corporation">
//   Microsoft Visual Studio ALM Rangers
// </copyright>
// <summary>
//   The permission scope.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.PermissionsExtractionTool
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The permission scope.
    /// </summary>
    [Serializable]
    public enum PermissionScope
    {
        /// <summary>
        /// The team project.
        /// </summary>
        [XmlEnum(Name = "TeamProject")]
        TeamProject, 

        /// <summary>
        /// The team build.
        /// </summary>
        [XmlEnum(Name = "TeamBuild")]
        TeamBuild, 

        /// <summary>
        /// The work item areas.
        /// </summary>
        [XmlEnum(Name = "WorkItemAreas")]
        WorkItemAreas, 

        /// <summary>
        /// The work item iterations.
        /// </summary>
        [XmlEnum(Name = "WorkItemIterations")]
        WorkItemIterations, 

        /// <summary>
        /// The source control.
        /// </summary>
        [XmlEnum(Name = "SourceControl")]
        SourceControl,

        /// <summary>
        /// The Git source control.
        /// </summary>
        [XmlEnum(Name = "GitSourceControl")]
        GitSourceControl,

        /// <summary>
        /// Workspaces 
        /// </summary>
        [XmlEnum(Name = "Workspaces")]
        Workspaces,

        /// <summary>
        /// WorkItems
        /// </summary>
        [XmlEnum(Name = "WorkItems")]
        WorkItems,

        /// <summary>
        /// Warehouse 
        /// </summary>
        [XmlEnum(Name = "Warehouse")]
        Warehouse,

        /// <summary>
        /// Collection
        /// </summary>
        [XmlEnum(Name = "Collection")]
        Collection
    }
}