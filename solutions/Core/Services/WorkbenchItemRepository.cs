// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkbenchItemRepository.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the WorkbenchItemRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;

    /// <summary>
    /// The workbench item repository.
    /// </summary>
    internal class WorkbenchItemRepository : IWorkbenchItemRepository
    {
        private readonly IFilterService filterService;

        /// <summary>
        /// The workbench item collection.
        /// </summary>
        private readonly Collection<IWorkbenchItem> workbenchItemCollection = new Collection<IWorkbenchItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchItemRepository"/> class.
        /// </summary>
        public WorkbenchItemRepository()
            : this(ServiceManager.Instance.GetService<ILinkManagerService>(), ServiceManager.Instance.GetService<IFilterService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchItemRepository"/> class.
        /// </summary>
        /// <param name="linkManagerService">The link manager service.</param>
        /// <param name="filterService">The filter service.</param>
        public WorkbenchItemRepository(ILinkManagerService linkManagerService, IFilterService filterService)
        {
            if (linkManagerService == null)
            {
                throw new ArgumentNullException("linkManagerService");
            }
            if (filterService == null)
            {
                throw new ArgumentNullException("filterService");
            }

            this.filterService = filterService;

            linkManagerService.LinkAdded += this.OnLinkAdded;
            linkManagerService.LinkRemoved += this.OnLinkRemoved;
            this.filterService.FiltersChanged += this.OnFiltersChanged;
        }

        /// <summary>
        /// Occurs when [link change].
        /// </summary>
        public event EventHandler<ItemLinkChangeEventArgs> LinkChanged;

        /// <summary>
        /// Occurs when [item state changed].
        /// </summary>
        public event EventHandler<ItemStateChangeEventArgs> ItemStateChanged;

        /// <summary>
        /// Occurs when [filters changed].
        /// </summary>
        public event EventHandler FiltersChanged;

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event EventHandler<RepositoryChangedEventArgs<IWorkbenchItem>> CollectionChanged;

        /// <summary>
        /// Gets the unfiltered workbench items.
        /// </summary>
        /// <value>The unfiltered workbench items.</value>
        public IEnumerable<IWorkbenchItem> UnfilteredList
        {
            get
            {
                return this.workbenchItemCollection;
            }
        }

        /// <summary>
        /// Adds the specified workbench item.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <exception cref="ArgumentNullException" />
        public void Add(IWorkbenchItem workbenchItem)
        {
            if (workbenchItem == null)
            {
                throw new ArgumentNullException("workbenchItem");
            }

            if (this.TryAddWorkbenchItem(workbenchItem) && this.CollectionChanged != null)
            {
                this.CollectionChanged(this, new RepositoryChangedEventArgs<IWorkbenchItem>(ChangeActionOption.Add, new[] { workbenchItem }));
            }
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="workbenchItems">The workbench items.</param>
        /// <exception cref="ArgumentNullException" />
        public void AddRange(IEnumerable<IWorkbenchItem> workbenchItems)
        {
            if (workbenchItems == null)
            {
                throw new ArgumentNullException("workbenchItems");
            }

            var addedItems = workbenchItems.Where(this.TryAddWorkbenchItem).ToArray();

            if (addedItems.Any() && this.CollectionChanged != null)
            {
                this.CollectionChanged(this, new RepositoryChangedEventArgs<IWorkbenchItem>(ChangeActionOption.Add, addedItems));
            }
        }

        /// <summary>
        /// Removes the specified workbench item.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <exception cref="ArgumentNullException" />
        public void Remove(IWorkbenchItem workbenchItem)
        {
            if (workbenchItem == null)
            {
                throw new ArgumentNullException("workbenchItem");
            }

            if (this.TryRemoveWorkbenchItem(workbenchItem) && this.CollectionChanged != null)
            {
                this.CollectionChanged(this, new RepositoryChangedEventArgs<IWorkbenchItem>(ChangeActionOption.Remove, new[] { workbenchItem }));
            }

            ReleaseWorkbenchItemResources(workbenchItem);
        }

        /// <summary>
        /// Removes the range.
        /// </summary>
        /// <param name="workbenchItems">The workbench items.</param>
        /// <exception cref="ArgumentNullException" />
        public void RemoveRange(IEnumerable<IWorkbenchItem> workbenchItems)
        {
            if (workbenchItems == null)
            {
                throw new ArgumentNullException("workbenchItems");
            }

            var removedItems = workbenchItems.Where(this.TryRemoveWorkbenchItem).ToArray();

            if (removedItems.Any() && this.CollectionChanged != null)
            {
                this.CollectionChanged(this, new RepositoryChangedEventArgs<IWorkbenchItem>(ChangeActionOption.Remove, removedItems));
            }

            ReleaseWorkbenchItemResources(workbenchItems);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            var clearedItems = this.workbenchItemCollection.ToArray();

            this.workbenchItemCollection.Clear();

            foreach (var workbenchItem in clearedItems)
            {
                workbenchItem.StateChanged -= this.OnItemStateChanged;
            }

            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, new RepositoryChangedEventArgs<IWorkbenchItem>(ChangeActionOption.Clear, clearedItems));
            }

            ReleaseWorkbenchItemResources(clearedItems);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<IWorkbenchItem> GetEnumerator()
        {
            return this.filterService.HasFilter
                ? this.filterService.ApplyFilter(this.workbenchItemCollection).GetEnumerator()
                : this.workbenchItemCollection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Called when [refresh collection].
        /// </summary>
        public void OnRefreshCollection()
        {
            if (this.CollectionChanged == null)
            {
                return;
            }

            this.CollectionChanged(this, new RepositoryChangedEventArgs<IWorkbenchItem>(ChangeActionOption.Refresh, this));
        }

        /// <summary>
        /// Called when [workbench item state changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.ItemStateChangeEventArgs"/> instance containing the event data.</param>
        public void OnItemStateChanged(object sender, ItemStateChangeEventArgs e)
        {
            if (this.ItemStateChanged == null)
            {
                return;
            }

            this.ItemStateChanged(sender, e);
        }

        /// <summary>
        /// Releases the workbench item resources.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        private static void ReleaseWorkbenchItemResources(IWorkbenchItem workbenchItem)
        {
            // Destroy any existing links.
            workbenchItem.ClearLinks();

            // Kill the referenced resources.
            workbenchItem.ReleaseResources();
        }

        /// <summary>
        /// Releases the workbench item resources.
        /// </summary>
        /// <param name="workbenchItems">The workbench items.</param>
        private static void ReleaseWorkbenchItemResources(IEnumerable<IWorkbenchItem> workbenchItems)
        {
            foreach (var workbenchItem in workbenchItems)
            {
                ReleaseWorkbenchItemResources(workbenchItem);
            }
        }

        /// <summary>
        /// Called when [link added].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.LinkChangeEventArgs"/> instance containing the event data.</param>
        private void OnLinkAdded(object sender, LinkChangeEventArgs e)
        {
            if (this.LinkChanged == null)
            {
                return;
            }

            this.LinkChanged(this, new ItemLinkChangeEventArgs(e.Context.Parent, null, e.Context));
        }

        /// <summary>
        /// Called when [link removed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.LinkChangeEventArgs"/> instance containing the event data.</param>
        private void OnLinkRemoved(object sender, LinkChangeEventArgs e)
        {
            if (this.LinkChanged == null)
            {
                return;
            }

            this.LinkChanged(this, new ItemLinkChangeEventArgs(e.Context.Parent, e.Context, null));
        }

        /// <summary>
        /// Called when [filters changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnFiltersChanged(object sender, EventArgs e)
        {
            this.OnRefreshCollection();

            if (this.FiltersChanged != null)
            {
                this.FiltersChanged(sender, e);
            }
        }

        /// <summary>
        /// Tries to add the specified workbench item.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns><c>True</c> if the item is added; otherwise <c>false</c>.</returns>
        private bool TryAddWorkbenchItem(IWorkbenchItem workbenchItem)
        {
            if (this.workbenchItemCollection.Contains(workbenchItem))
            {
                return false;
            }

            workbenchItem.StateChanged += this.OnItemStateChanged;

            this.workbenchItemCollection.Add(workbenchItem);

            return true;
        }

        /// <summary>
        /// Tries to remove the specified workbench item.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns><c>True</c> if the item is removed; otherwise <c>false</c>.</returns>
        private bool TryRemoveWorkbenchItem(IWorkbenchItem workbenchItem)
        {
            if (!this.workbenchItemCollection.Contains(workbenchItem))
            {
                return false;
            }

            workbenchItem.StateChanged -= this.OnItemStateChanged;

            this.workbenchItemCollection.Remove(workbenchItem);

            return true;
        }
    }
}
