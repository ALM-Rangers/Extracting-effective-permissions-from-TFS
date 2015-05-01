// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionsReport.cs" company="Microsoft Corporation">
//   Microsoft Visual Studio ALM Rangers
// </copyright>
// <summary>
//   The permissions report.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.PermissionsExtractionTool
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The permissions report.
    /// </summary>
    public class PermissionsReport
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the team projects.
        /// </summary>
        [XmlArray(ElementName = "TeamProjects")]
        [XmlArrayItem(ElementName = "TeamProject")]
        public List<TfsTeamProject> TeamProjects { get; set; }

        /// <summary>
        /// Gets or sets the Team Project collection url.
        /// </summary>
        public string TfsCollectionUrl { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        #endregion
    }
}