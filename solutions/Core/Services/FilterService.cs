// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterService.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the FilterService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TfsWorkbench.Core.Interfaces;

    /// <summary>
    /// The filter service class.
    /// </summary>
    internal class FilterService : IFilterService
    {
        /// <summary>
        /// The filter predicate.
        /// </summary>
        private IFilterProvider internalFilterProvider;

        /// <summary>
        /// Occurs when [filters changed].
        /// </summary>
        public event EventHandler FiltersChanged;

        /// <summary>
        /// Gets a value indicating whether this instance has filter.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has filter; otherwise, <c>false</c>.
        /// </value>
        public bool HasFilter
        {
            get
            {
                return this.internalFilterProvider != null;
            }
        }

        /// <summary>
        /// Gets or sets the filter provider.
        /// </summary>
        /// <value>The filter provider.</value>
        public IFilterProvider FilterProvider
        {
            get
            {
                return this.internalFilterProvider;
            }

            set
            {
                if (this.internalFilterProvider == value)
                {
                    return;
                }

                this.internalFilterProvider = value;

                if (this.FiltersChanged != null)
                {
                    this.FiltersChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified workbench item is excluded.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>
        /// <c>true</c> if the specified workbench item is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(IWorkbenchItem workbenchItem)
        {
            return this.HasFilter && !this.internalFilterProvider.IsIncluded(workbenchItem);
        }

        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The filtered items.</returns>
        public IEnumerable<IWorkbenchItem> ApplyFilter(IEnumerable<IWorkbenchItem> items)
        {
            return this.internalFilterProvider == null
                    ? items
                    : items.Where(this.internalFilterProvider.IsIncluded);
        }
    }
}
