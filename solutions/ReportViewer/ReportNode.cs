// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportNode.cs" company="None">
//   None
// </copyright>
// <summary>
//   The report node class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ReportViewer
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The report node class.
    /// </summary>
    internal class ReportNode : IReportNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportNode"/> class.
        /// </summary>
        public ReportNode()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportNode"/> class.
        /// </summary>
        /// <param name="catalogItem">The catalog item.</param>
        public ReportNode(CatalogItemBase catalogItem)
        {
            this.CatalogItem = catalogItem;
            this.Children = new Collection<IReportNode>();
        }

        /// <summary>
        /// Gets the node children.
        /// </summary>
        /// <value></value>
        public ICollection<IReportNode> Children { get; private set; }

        /// <summary>
        /// Gets the catalog item.
        /// </summary>
        /// <value>The catalog item.</value>
        public CatalogItemBase CatalogItem { get; private set; }
    }
}