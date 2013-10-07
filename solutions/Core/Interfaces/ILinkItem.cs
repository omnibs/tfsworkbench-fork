// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILinkItem.cs" company="None">
//   None
// </copyright>
// <summary>
//   The Link Item interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Interfaces
{
    /// <summary>
    /// The Link Item interface
    /// </summary>
    public interface ILinkItem
    {
        /// <summary>
        /// Gets or sets the name of the link.
        /// </summary>
        /// <value>The name of the link.</value>
        string LinkName { get; set; }

        /// <summary>
        /// Gets or sets the child.
        /// </summary>
        /// <value>The child.</value>
        IWorkbenchItem Child { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        IWorkbenchItem Parent { get; set; }
    }
}