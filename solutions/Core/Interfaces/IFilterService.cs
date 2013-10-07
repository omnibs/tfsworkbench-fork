// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFilterService.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IFilterService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TfsWorkbench.Core.Interfaces
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The filter service interface.
    /// </summary>
    public interface IFilterService
    {
        /// <summary>
        /// Occurs when [filters changed].
        /// </summary>
        event EventHandler FiltersChanged;

        /// <summary>
        /// Gets a value indicating whether this instance has filter.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has filter; otherwise, <c>false</c>.
        /// </value>
        bool HasFilter { get; }

        /// <summary>
        /// Gets or sets the filter provider.
        /// </summary>
        /// <value>The filter provider.</value>
        IFilterProvider FilterProvider { get; set; }

        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The filtered items.</returns>
        IEnumerable<IWorkbenchItem> ApplyFilter(IEnumerable<IWorkbenchItem> items);

        /// <summary>
        /// Determines whether the specified workbench item is excluded.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>
        /// <c>true</c> if the specified workbench item is excluded; otherwise, <c>false</c>.
        /// </returns>
        bool IsExcluded(IWorkbenchItem workbenchItem);
    }
}
