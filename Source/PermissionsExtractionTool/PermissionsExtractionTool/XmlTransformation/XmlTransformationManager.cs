// --------------------------------------------------------------------------------------------------------------------
// <copyright file="xmltransformationmanager.cs" company="Microsoft Corporation">
//   Microsoft Visual Studio ALM Rangers
// </copyright>
// <summary>
//   Transforms the xml output into an HTML file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.PermissionsExtractionTool.XmlTransformation
{
    using System;
    using System.Xml.Xsl;

    /// <summary>
    /// Simple class to handle the XSL transformation
    /// </summary>
    public class XmlTransformationManager
    {
        /// <summary>
        /// Transform the program xml output into a another XML format
        /// </summary>
        /// <param name="inputXmlFile">input xml file</param>
        /// <param name="xslFileName">transformation file name</param>
        /// <param name="xmlOutputFileName">output file name</param>
        /// <returns>true if the transformation was successful</returns>
        public static bool TransformXmlUsingXsl(string inputXmlFile, string xslFileName, string xmlOutputFileName)
        {
            try
            {
                var myXslTransform = new XslCompiledTransform();
                myXslTransform.Load(xslFileName);
                myXslTransform.Transform(inputXmlFile, xmlOutputFileName);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
