// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHighlightProvider.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ISearchProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Interfaces
{
    /// <summary>
    /// The highlight provider interface.
    /// </summary>
    public interface IHighlightProvider
    {
        /// <summary>
        /// Gets a value indicating whether this instance is search provider.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is search provider; otherwise, <c>false</c>.
        /// </value>
        bool IsHighlightProvider { get; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        string Title { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        string Description { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        bool IsActive { get; }

        /// <summary>
        /// Highlights the specified workbench item.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        void Highlight(IWorkbenchItem workbenchItem);
    }
}
