// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkItem.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the LinkItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.DataObjects
{
    using Interfaces;

    /// <summary>
    /// Initialises and instance of TfsWorkbench.Core.DataObjects.LinkItem
    /// </summary>
    internal class LinkItem : ILinkItem
    {
        /// <summary>
        /// Gets or sets the name of the link.
        /// </summary>
        /// <value>The name of the link.</value>
        public string LinkName { get; set; }

        /// <summary>
        /// Gets or sets the child.
        /// </summary>
        /// <value>The child.</value>
        public IWorkbenchItem Child { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public IWorkbenchItem Parent { get; set; }
    }
}
