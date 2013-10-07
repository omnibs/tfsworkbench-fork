// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkbenchFilterCollection.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the WorkbenchFilterCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.FilterService
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.FilterService.Properties;

    /// <summary>
    /// The workbench filter colleciton class.
    /// </summary>
    [XmlRoot(ElementName = "FilterCollection", Namespace = "http://schemas.workbench/Filter")]
    public class WorkbenchFilterCollection : ObservableCollection<WorkbenchFilter>, ICloneable, IFilterProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchFilterCollection"/> class.
        /// </summary>
        public WorkbenchFilterCollection()
        {
            this.Initialise();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchFilterCollection"/> class.
        /// </summary>
        /// <param name="list">The initial list.</param>
        public WorkbenchFilterCollection(List<WorkbenchFilter> list)
            : base(list)
        {
            this.Initialise();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchFilterCollection"/> class.
        /// </summary>
        /// <param name="collection">The initial collection.</param>
        public WorkbenchFilterCollection(IEnumerable<WorkbenchFilter> collection)
            : base(collection)
        {
            this.Initialise();
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; private set; }

        /// <summary>
        /// Determines whether the specified workbench item is included in the filtered set.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>
        /// <c>true</c> if the specified workbench item is match; otherwise, <c>false</c>.
        /// </returns>
        public bool IsIncluded(IWorkbenchItem workbenchItem)
        {
            var inclusionFilters = this.Where(f => f.FilterAction == FilterActionOption.Include).ToArray();
            var exclusionFilters = this.Where(f => f.FilterAction == FilterActionOption.Exclude).ToArray();

            var itemIsIncluded = !inclusionFilters.Any() || inclusionFilters.Any(f => f.IsMatch(workbenchItem));
            var itemIsExcluded = exclusionFilters.Any(f => f.IsMatch(workbenchItem));

            return itemIsIncluded && !itemIsExcluded;
        }

        /// <summary>
        /// Includes workbench items that match the filter specification.
        /// </summary>
        /// <param name="workbenchItemTypeName">Name of the workbench item type.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="filterOperator">The filter operator.</param>
        /// <param name="value">The value.</param>
        /// <returns>The filter collection.</returns>
        public WorkbenchFilterCollection Include(string workbenchItemTypeName, string fieldName, FilterOperatorOption filterOperator, object value)
        {
            this.Add(
                new WorkbenchFilter(FilterActionOption.Include)
                    .ItemsOfType(workbenchItemTypeName)
                    .WithField(fieldName)
                    .That(filterOperator, value));

            return this;
        }

        /// <summary>
        /// Excludes workbench items that match the filter specification.
        /// </summary>
        /// <param name="workbenchItemTypeName">Name of the workbench item type.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="filterOperator">The filter operator.</param>
        /// <param name="value">The value.</param>
        /// <returns>The filter collection.</returns>
        public WorkbenchFilterCollection Exclude(string workbenchItemTypeName, string fieldName, FilterOperatorOption filterOperator, object value)
        {
            this.Add(
                new WorkbenchFilter(FilterActionOption.Exclude)
                    .ItemsOfType(workbenchItemTypeName)
                    .WithField(fieldName)
                    .That(filterOperator, value));

            return this;
        }

        /// <summary>
        /// Ands the specified filter to chain.
        /// </summary>
        /// <param name="filterToChain">The filter to chain.</param>
        /// <returns>The specfied filter including this chained instance.</returns>
        public WorkbenchFilterCollection And(WorkbenchFilter filterToChain)
        {
            if (filterToChain == null)
            {
                throw new ArgumentNullException("filterToChain");
            }

            this.Add(filterToChain);

            return this;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Description;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            var clone = new WorkbenchFilterCollection();
            foreach (var filter in this)
            {
                clone.Add(filter.Clone() as WorkbenchFilter);
            }

            return clone;
        }

        /// <summary>
        /// Initialises this instance.
        /// </summary>
        private void Initialise()
        {
            this.CollectionChanged += this.HandleCollectionChanged;
            this.UpdateDescription();
        }

        /// <summary>
        /// Handles the collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateDescription();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        foreach (var workbenchFilter in e.NewItems.OfType<WorkbenchFilter>())
                        {
                            workbenchFilter.PropertyChanged += this.OnFilterPropertyChanged;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Reset:
                    if (e.OldItems != null)
                    {
                        foreach (var workbenchFilter in e.OldItems.OfType<WorkbenchFilter>())
                        {
                            workbenchFilter.PropertyChanged -= this.OnFilterPropertyChanged;
                        }
                    }
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Called when [filter property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnFilterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateDescription();
        }

        /// <summary>
        /// Updates the description.
        /// </summary>
        private void UpdateDescription()
        {
            this.Description = !this.Any()
                       ? Resources.String017
                       : this.Select(f => f.ToString()).Aggregate((current, next) => string.Concat(current, Environment.NewLine, next));

            this.OnPropertyChanged(new PropertyChangedEventArgs("Description"));
        }
    }
}
