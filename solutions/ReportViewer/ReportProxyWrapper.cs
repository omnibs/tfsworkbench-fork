// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportProxyWrapper.cs" company="None">
//   None
// </copyright>
// <summary>
//   The default report proxy wrapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ReportViewer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.ReportViewer.Properties;

    /// <summary>
    /// The default report proxy wrapper.
    /// </summary>
    internal class ReportProxyWrapper : IReportProxyWrapper
    {
        /// <summary>
        /// The report service proxy instance.
        /// </summary>
        private readonly IReportWebService reportServiceProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportProxyWrapper"/> class.
        /// </summary>
        /// <param name="reportServiceProxy">The report service proxy.</param>
        /// <param name="reportPageViewerPath">The report page viewer path.</param>
        /// <exception cref="ArgumentNullException"/>
        public ReportProxyWrapper(IReportWebService reportServiceProxy, string reportPageViewerPath)
        {
            if (reportServiceProxy == null)
            {
                throw new ArgumentNullException("reportServiceProxy");
            }

            if (reportPageViewerPath == null)
            {
                throw new ArgumentNullException("reportPageViewerPath");
            }

            this.ReportDisplayPath = reportPageViewerPath;
            this.reportServiceProxy = reportServiceProxy;
        }

        /// <summary>
        /// Gets the report display path.
        /// </summary>
        /// <value>The report display path.</value>
        public string ReportDisplayPath { get; private set; }

        /// <summary>
        /// Gets the report root tree.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="reportServiceEndPoint">The report service end point.</param>
        /// <param name="reportFolderPath">The report folder path.</param>
        /// <returns>
        /// The root report node for the specified project data.
        /// </returns>
        public IReportNode GetRootReportNode(IProjectData projectData, Uri reportServiceEndPoint, string reportFolderPath)
        {
            if (projectData == null)
            {
                throw new ArgumentNullException("projectData");
            }

            if (reportServiceEndPoint == null)
            {
                throw new ArgumentNullException("reportServiceEndPoint");
            }

            if (string.IsNullOrEmpty(reportFolderPath))
            {
                throw new ArgumentNullException("reportFolderPath");
            }

            this.reportServiceProxy.Url = reportServiceEndPoint.AbsoluteUri;
            this.reportServiceProxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            var rootNode = new ReportNode();
            var nodeMap = new Dictionary<string, IReportNode>();

            var catalogItems = this.reportServiceProxy.ListBaseChildren(reportFolderPath, true)
                .Where(c => !c.Hidden && (c.IsFolder || c.IsReport))
                .OrderByDescending(c => c.IsFolder)
                .ThenBy(c => c.Name);

            foreach (var catalogItem in catalogItems)
            {
                var parentPath = catalogItem.Path.Substring(0, catalogItem.Path.LastIndexOf("/"));

                IReportNode parentNode = rootNode;
                if (!Equals(parentPath, reportFolderPath) && !nodeMap.TryGetValue(parentPath, out parentNode))
                {
                    // No parent found.
                    continue;
                }

                var reportNode = new ReportNode(catalogItem);

                parentNode.Children.Add(reportNode);
                nodeMap.Add(catalogItem.Path, reportNode);
            }

            return rootNode;
        }
    }
}