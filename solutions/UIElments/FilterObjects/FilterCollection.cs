// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterCollection.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the FilterCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;
using TfsWorkbench.Core.Helpers;
using TfsWorkbench.Core.Interfaces;
using TfsWorkbench.Core.EventArgObjects;

namespace TfsWorkbench.UIElements.FilterObjects
{
    public interface IFilterCollection : ICollection<TypeFilter>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when [selection changed].
        /// </summary>
        event EventHandler SelectionChanged;

        /// <summary>
        /// Gets the selected workbench items.
        /// </summary>
        /// <value>The selected workbench items.</value>
        int SelectedWorkbenchItemCount { get; }

        /// <summary>
        /// Initialises the specified project data.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        void Initialise(IProjectData projectData);

        /// <summary>
        /// Determines whether the specified workbench item is matched.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>
        /// <c>true</c> if the specified workbench item is match; otherwise, <c>false</c>.
        /// </returns>
        bool IsMatch(IWorkbenchItem workbenchItem);

        void SelectItems(IEnumerable<IWorkbenchItem> workbenchItems);

        void DeselectItem(IWorkbenchItem workbenchItem);

        /// <summary>
        /// Filters to specified item.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        void FilterToItem(IWorkbenchItem workbenchItem);
    }

    /// <summary>
    /// The filter collectio class.
    /// </summary>
    public class FilterCollection : ObservableCollection<TypeFilter>, IFilterCollection
    {
        /// <summary>
        /// The dispatcher.
        /// </summary>
        private readonly Dispatcher dispatcher;

        /// <summary>
        /// The current project data.
        /// </summary>
        private IProjectData currentProjectData;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterCollection"/> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        public FilterCollection(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        /// <summary>
        /// Occurs when [selection changed].
        /// </summary>
        public event EventHandler SelectionChanged;

        /// <summary>
        /// Gets the selected workbench items.
        /// </summary>
        /// <value>The selected workbench items.</value>
        public int SelectedWorkbenchItemCount
        {
            get
            {
                return this.SelectMany(c => c.ChildFilters).Sum(cf => cf.IsSelected ? 1 : 0);
            }
        }

        /// <summary>
        /// Initialises the specified project data.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        public void Initialise(IProjectData projectData)
        {
            if (currentProjectData == projectData)
            {
                return;
            }

            ClearExistingFilters();

            if (currentProjectData != null && projectData != currentProjectData)
            {
                currentProjectData.WorkbenchItems.CollectionChanged -= OnItemCollectionChanged;
                currentProjectData = null;
            }

            if (projectData == null)
            {
                return;
            }

            if (currentProjectData != projectData)
            {
                currentProjectData = projectData;
                currentProjectData.WorkbenchItems.CollectionChanged += OnItemCollectionChanged;
            }

            foreach (var type in projectData.ItemTypes.Where(t => !string.IsNullOrEmpty(t.TypeName)).OrderBy(t => t.TypeName))
            {
                var typeName = type.TypeName;
                var filter = new TypeFilter(typeName);
                filter.PropertyChanged += OnFilterChanged;

                var items = projectData.WorkbenchItems
                    .Where(w => w.GetTypeName().Equals(typeName))
                    .OrderBy(WorkbenchItemHelper.GetId)
                    .ToArray();

                foreach (var childFilter in items.Select(workbenchItem => CreateChildFilter(filter, workbenchItem)))
                {
                    filter.ChildFilters.Add(childFilter);
                }

                Add(filter);
            }
        }

        /// <summary>
        /// Determines whether the specified workbench item is matched.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>
        /// <c>true</c> if the specified workbench item is match; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(IWorkbenchItem workbenchItem)
        {
            var isMatch = this.Any(f => f.IsSelected && f.IsMatch(workbenchItem));

            return isMatch;
        }

        public void SelectItems(IEnumerable<IWorkbenchItem> workbenchItems)
        {
            var instanceFilters = Items.SelectMany(i => i.ChildFilters).OfType<InstanceFilter>();

            var iteratedArray = workbenchItems.ToArray();

            foreach (var instanceFilter in instanceFilters)
            {
                instanceFilter.IsSelected = iteratedArray.Any(instanceFilter.IsMatch);
            }
        }

        public void DeselectItem(IWorkbenchItem workbenchItem)
        {
            var instanceFilters = Items.SelectMany(i => i.ChildFilters).OfType<InstanceFilter>();

            var filter = instanceFilters.FirstOrDefault(f => f.IsMatch(workbenchItem));

            if (filter != null)
            {
                filter.IsSelected = false;
            }

            var childlessItems = Items.Where(i => i.ChildFilters.All(cf => !cf.IsSelected)).ToArray();

            foreach (var childlessItem in childlessItems)
            {
                childlessItem.IsSelected = false;
            }
        }

        /// <summary>
        /// Filters to specified item.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        public void FilterToItem(IWorkbenchItem workbenchItem)
        {
            foreach (var filter in Items)
            {
                filter.IsSelected = false;
                foreach (var childFilter in filter.ChildFilters.OfType<InstanceFilter>())
                {
                    childFilter.IsSelected = childFilter.IsMatch(workbenchItem);
                }
            }
        }

        /// <summary>
        /// Gets the child filter.
        /// </summary>
        /// <param name="parentFilter">The parent filter.</param>
        /// <param name="item">The workbench item.</param>
        /// <returns>
        /// A child filter for the specified workbench item.
        /// </returns>
        private IFilterItem CreateChildFilter(IFilterItem parentFilter, IWorkbenchItem item)
        {
            var instanceFilter = new InstanceFilter(parentFilter, item);
            instanceFilter.PropertyChanged += OnFilterChanged;
            return instanceFilter;
        }

        /// <summary>
        /// Clears the existing filters.
        /// </summary>
        private void ClearExistingFilters()
        {
            foreach (var filter in Items)
            {
                filter.PropertyChanged -= OnFilterChanged;

                foreach (var childFilter in filter.ChildFilters)
                {
                    childFilter.PropertyChanged -= OnFilterChanged;

                    var instanceFilter = childFilter as InstanceFilter;

                    if (instanceFilter != null)
                    {
                        instanceFilter.ReleaseResources();
                    }
                }
            }

            Clear();
        }

        /// <summary>
        /// Called when [filter changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnFilterChanged(object sender, PropertyChangedEventArgs e)
        {
            if (SelectionChanged == null  || !e.PropertyName.Equals("IsSelected"))
            {
                return;
            }

            OnPropertyChanged(new PropertyChangedEventArgs("SelectedWorkbenchItemCount"));

            SelectionChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Removes the item from the filter.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        private void RemoveItemFromFilter(IWorkbenchItem workbenchItem)
        {
            var itemType = workbenchItem.GetTypeName();

            var filterRoot = this.FirstOrDefault(f => f.DisplayText.Equals(itemType));

            if (filterRoot == null)
            {
                return;
            }

            var filterItem = filterRoot.ChildFilters.FirstOrDefault(cf => cf.IsMatch(workbenchItem));

            if (filterItem != null)
            {
                filterRoot.ChildFilters.Remove(filterItem);
            }
        }

        /// <summary>
        /// Called when [item added].
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        private void AddItemToFilter(IWorkbenchItem workbenchItem)
        {
            var itemType = workbenchItem.GetTypeName();

            var filterRoot = this.FirstOrDefault(f => f.DisplayText.Equals(itemType));

            if (filterRoot == null)
            {
                return;
            }

            var fitlerItem = CreateChildFilter(filterRoot, workbenchItem);

            filterRoot.ChildFilters.Add(fitlerItem);
        }

        /// <summary>
        /// Called when [item collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.RepositoryChangedEventArgs&lt;IWorkbenchItem&gt;"/> instance containing the event data.</param>
        private void OnItemCollectionChanged(object sender, RepositoryChangedEventArgs<IWorkbenchItem> e)
        {
            if (dispatcher == null)
            {
                return;
            }

            Action action = () =>
                {
                    switch (e.Action)
                    {
                        case ChangeActionOption.Add:
                            foreach (var item in e.Context) 
                            {
                                AddItemToFilter(item);
                            }

                            break;
                        case ChangeActionOption.Remove:
                        case ChangeActionOption.Clear:
                            foreach (var item in e.Context)
                            {
                                RemoveItemFromFilter(item);
                            }

                            break;
                        case ChangeActionOption.Refresh:
                            Initialise(currentProjectData);

                            break;
                        default:
                            return;
                    }
                };

            dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
        }
    }
}
