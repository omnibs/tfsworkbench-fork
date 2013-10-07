// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterItemBase.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the FilterItemBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TfsWorkbench.Core.Interfaces;

namespace TfsWorkbench.UIElements.FilterObjects
{
    /// <summary>
    /// The filter item base class.
    /// </summary>
    public abstract class FilterItemBase : IFilterItem
    {
        /// <summary>
        /// The child filters.
        /// </summary>
        private readonly ObservableCollection<IFilterItem> childFilters = new ObservableCollection<IFilterItem>();

        /// <summary>
        /// The is selected flag.
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterItemBase"/> class.
        /// </summary>
        /// <param name="parentFilter">The parent filter.</param>
        protected FilterItemBase(IFilterItem parentFilter)
        {
            this.ParentFilter = parentFilter;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the display text.
        /// </summary>
        /// <value>The display text.</value>
        public abstract string DisplayText { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (this.isSelected == value)
                {
                    return;
                }

                this.isSelected = value;

                foreach (var childFilter in this.ChildFilters.Where(childFilter => childFilter.IsSelected != value))
                {
                    childFilter.IsSelected = value;
                }

                if (value && this.ParentFilter != null && !this.ParentFilter.IsSelected)
                {
                    this.ParentFilter.SelectWithoutCascade();
                }

                this.OnPropertyChanged("IsSelected");
            }
        }

        /// <summary>
        /// Gets the parent filter.
        /// </summary>
        /// <value>The parent filter.</value>
        public IFilterItem ParentFilter { get; private set; }

        /// <summary>
        /// Gets the child filters.
        /// </summary>
        /// <value>The child filters.</value>
        public ObservableCollection<IFilterItem> ChildFilters 
        { 
            get 
            { 
                return this.childFilters; 
            } 
        }

        /// <summary>
        /// Gets or sets the filter predicate.
        /// </summary>
        /// <value>The filter predicate.</value>
        protected Func<IWorkbenchItem, bool> FilterPredicate { get; set; }

        /// <summary>
        /// Determines whether the specified workbench item is matched.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>
        /// <c>true</c> if the specified workbench item is matched; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(IWorkbenchItem workbenchItem)
        {
            var hasChildren = this.ChildFilters.Any();

            var isMatch = this.FilterPredicate(workbenchItem)
                          && (!hasChildren || this.ChildFilters.Any(c => c.IsSelected && c.IsMatch(workbenchItem)));

            return isMatch;
        }

        /// <summary>
        /// Sets the selected without cascade.
        /// </summary>
        public void SelectWithoutCascade()
        {
            if (this.isSelected)
            {
                return;
            }

            this.isSelected = true;

            this.OnPropertyChanged("IsSelected");
        }

        /// <summary>
        /// Called when [property changes].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged == null)
            {
                return;
            }

            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}