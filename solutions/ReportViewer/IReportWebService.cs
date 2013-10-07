// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReportWebService.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IReportService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ReportViewer
{
    using System.Net;

    /// <summary>
    /// The report web service interface.
    /// </summary>
    public interface IReportWebService
    {
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The service URL.</value>
        string Url { get; set; }

        /// <summary>
        /// Gets or sets the credentials.
        /// </summary>
        /// <value>The credentials.</value>
        ICredentials Credentials { get; set; }

        /// <summary>
        /// Lists the children.
        /// </summary>
        /// <param name="path">The item path.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns>A list of child objects.</returns>
        CatalogItemBase[] ListBaseChildren(string path, bool recursive);
    }
}
