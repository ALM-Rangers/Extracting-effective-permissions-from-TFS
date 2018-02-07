// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Options.cs" company="Microsoft Corporation">
//   Microsoft Visual Studio ALM Rangers
// </copyright>
// <summary>
//  Command line parameters
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.PermissionsExtractionTool
{
    using CommandLine;
    using CommandLine.Text;

    /// <summary>
    /// Command line parameters based on CommandLineParser package syntax
    /// </summary>
    internal class Options
    {
        /// <summary>
        /// Gets or sets user list for permission extraction.
        /// </summary>
        [OptionArray('u', "users", Required = true, HelpText = @"User name (domain\username format for TFS or email for VSTS) (at least one)")]
        public string[] Users { get; set; }

        /// <summary>
        /// Gets or sets the team collection URL
        /// </summary>
        [Option("collection", Required = true, HelpText = "Team Project collection URL. E.g. http://servername:8080/tfs/Collection for TFS, or https://youraccount.visualstudio.com for VSTS")]
        public string Collection { get; set; }

        /// <summary>
        /// Gets or sets the Team Project
        /// </summary>
        [OptionArray('p', "project", HelpText = "Restricts the Team Projects to scan")]
        public string[] Projects { get; set; }

        /// <summary>
        /// Gets or sets the output file pattern
        /// </summary>
        [Option('f', "outfile", HelpText = "Output file (the userName will be added before the extension)", Required = true)]
        public string OutputFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the HTML output is generated
        /// </summary>
        [Option("html", HelpText = "Generates an html output based on the ALM Rangers XSL tranformation")]
        public bool Html { get; set; }

        /// <summary>
        /// Generate command line help
        /// </summary>
        /// <returns>Command line usage</returns>
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this);
        }
    }
}