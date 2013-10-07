// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IParentContainer.cs" company="None">
//   None
// </copyright>
// <summary>
//   The parent container interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Interfaces
{
    /// <summary>
    /// The parent container interface.
    /// </summary>
    public interface IParentContainer
    {
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        IWorkbenchItem Parent { get; }
    }
}