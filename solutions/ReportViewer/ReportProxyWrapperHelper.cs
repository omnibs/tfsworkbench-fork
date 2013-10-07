// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportProxyWrapperHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ReportProxyWrapperHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ReportViewer
{
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.ReportViewer.Properties;

    /// <summary>
    /// The report proxy wrapper helper.
    /// </summary>
    internal static class ReportProxyWrapperHelper
    {
        /// <summary>
        /// The 2005 resport service wrapper.
        /// </summary>
        private static IReportProxyWrapper reportService2005;

        /// <summary>
        /// The 2008 report service wrapper.
        /// </summary>
        private static IReportProxyWrapper reportService2008;

        /// <summary>
        /// Gets or sets the report service2005.
        /// </summary>
        /// <value>The report service2005.</value>
        public static IReportProxyWrapper ReportService2005
        {
            get
            {
                return
                    reportService2005 = reportService2005
                        ?? new ReportProxyWrapper(new ReportService2005.ReportingService(), Settings.Default.ReportViewerPath2005);
            }

            set
            {
                reportService2005 = value;
            }
        }

        /// <summary>
        /// Gets or sets the report service2008.
        /// </summary>
        /// <value>The report service2008.</value>
        public static IReportProxyWrapper ReportService2008
        {
            get
            {
                return
                    reportService2008 = reportService2008
                    ?? new ReportProxyWrapper(new ReportService2008.ReportingService2005(), Settings.Default.ReportViewerPath2008);
            }

            set
            {
                reportService2008 = value;
            }
        }

        /// <summary>
        /// Gets the default project report folder.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <returns>The default report path for a SSRS 2005 project.</returns>
        public static string Get2005ProjectReportFolder(IProjectData projectData)
        {
            return string.Concat("/", projectData.ProjectName);
        }
    }
}
