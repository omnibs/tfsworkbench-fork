// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReportProxyWrapper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IReportProxyWrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ReportViewer
{
    using System;

    using TfsWorkbench.Core.Interfaces;

    /// <summary>
    /// The report proxy wrapper internace.
    /// </summary>
    public interface IReportProxyWrapper
    {
        /// <summary>
        /// Gets the report display path.
        /// </summary>
        /// <value>The report display path.</value>
        string ReportDisplayPath { get; }

        /// <summary>
        /// Gets the report root tree.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="reportServiceEndPoint">The report service end point.</param>
        /// <param name="reportFolderPath">The report folder path.</param>
        /// <returns>
        /// The root report node for the specified project data.
        /// </returns>
        IReportNode GetRootReportNode(IProjectData projectData, Uri reportServiceEndPoint, string reportFolderPath);
    }
}
