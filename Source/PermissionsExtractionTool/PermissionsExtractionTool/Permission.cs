// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Permission.cs" company="Microsoft Corporation">
//   Microsoft Visual Studio ALM Rangers
// </copyright>
// <summary>
//   The permission.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.PermissionsExtractionTool
{
    using System.Xml.Serialization;

    /// <summary>
    /// The permission.
    /// </summary>
    [XmlRoot(ElementName = "Permission")]
    public class Permission
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Permission"/> class.
        /// </summary>
        /// <param name="scope">
        /// The scope.
        /// </param>
        /// <param name="displayName">
        /// The display name.
        /// </param>
        /// <param name="internalName">
        /// The internal name.
        /// </param>
        /// <param name="permissionConstant">
        /// The permission constant.
        /// </param>
        /// <param name="permissionId">
        /// The permission id.
        /// </param>
        /// <param name="permissionState">
        /// The permission state.
        /// </param>
        public Permission(
            PermissionScope scope, 
            string displayName, 
            string internalName, 
            string permissionConstant, 
            int permissionId, 
            string permissionState)
        {
            this.Scope = scope;
            this.PermissionConstant = permissionConstant;
            this.DisplayName = displayName;
            this.PermissionId = permissionId;
            this.InternalName = internalName;
            this.Value = permissionState;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Permission"/> class.
        /// </summary>
        public Permission()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the internal name.
        /// </summary>
        [XmlIgnore]
        public string InternalName { get; set; }

        /// <summary>
        /// Gets or sets the permission constant.
        /// </summary>
        public string PermissionConstant { get; set; }

        /// <summary>
        /// Gets or sets the permission id.
        /// </summary>
        [XmlIgnore]
        public int PermissionId { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        [XmlIgnore]
        public PermissionScope Scope { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the Group Member Inheritance.
        /// </summary>
        public string GroupMemberInheritance { get; set; }
        #endregion
    }
}