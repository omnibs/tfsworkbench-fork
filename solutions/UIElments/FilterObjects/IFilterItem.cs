// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFilterItem.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IFilterItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.ComponentModel;
using TfsWorkbench.Core.Interfaces;

namespace TfsWorkbench.UIElements.FilterObjects
{
    /// <summary>
    /// The filter item interface.
    /// </summary>
    public interface IFilterItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the display text.
        /// </summary>
        /// <value>The display text.</value>
        string DisplayText { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        bool IsSelected { get; set; }

        /// <summary>
        /// Gets the parent filter.
        /// </summary>
        /// <value>The parent filter.</value>
        IFilterItem ParentFilter { get;  }

        /// <summary>
        /// Gets the child filters.
        /// </summary>
        /// <value>The child filters.</value>
        ObservableCollection<IFilterItem> ChildFilters { get; }

        /// <summary>
        /// Determines whether the specified workbench item is matched.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>
        /// <c>true</c> if the specified workbench item is matched; otherwise, <c>false</c>.
        /// </returns>
        bool IsMatch(IWorkbenchItem workbenchItem);

        /// <summary>
        /// Sets the selected to true without cascade.
        /// </summary>
        void SelectWithoutCascade();
    }
}