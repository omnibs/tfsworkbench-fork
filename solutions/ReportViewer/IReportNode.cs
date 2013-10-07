// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReportNode.cs" company="None">
//   None
// </copyright>
// <summary>
//   The reoprt node interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ReportViewer
{
    using System.Collections.Generic;

    /// <summary>
    /// The reoprt node interface.
    /// </summary>
    public interface IReportNode
    {
        /// <summary>
        /// Gets the node children.
        /// </summary>
        ICollection<IReportNode> Children { get; }

        /// <summary>
        /// Gets the catalog item.
        /// </summary>
        /// <value>The catalog item.</value>
        CatalogItemBase CatalogItem { get; }
    }
}