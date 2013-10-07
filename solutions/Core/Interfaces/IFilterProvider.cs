// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFilterProvider.cs" company="None">
//   None
// </copyright>
// <summary>
//   The filter provider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Interfaces
{
    /// <summary>
    /// The filter provider interface.
    /// </summary>
    public interface IFilterProvider
    {
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        string Description { get; }

        /// <summary>
        /// Determines whether the specified workbench item is included in the filtered set.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>
        /// <c>true</c> if the specified workbench item is match; otherwise, <c>false</c>.
        /// </returns>
        bool IsIncluded(IWorkbenchItem workbenchItem);
    }
}