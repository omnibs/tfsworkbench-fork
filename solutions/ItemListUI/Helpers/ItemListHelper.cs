// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemListHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ItemListHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ItemListUI.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    using Core.DataObjects;
    using Core.Helpers;
    using Core.Interfaces;

    using TfsWorkbench.UIElements;

    /// <summary>
    /// Initializes instance of ItemListHelper
    /// </summary>
    internal class ItemListHelper
    {
        /// <summary>
        /// The unassigned indicator text.
        /// </summary>
        public const string UnassignedIndicator = "{Unassigned}";

        /// <summary>
        /// The item list instance.
        /// </summary>
        private readonly ItemList itemList;

        /// <summary>
        /// The container map dictionary.
        /// </summary>
        private readonly Dictionary<MultiSelectControlItemGroup, DependencyObject> containerMap =
            new Dictionary<MultiSelectControlItemGroup, DependencyObject>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemListHelper"/> class.
        /// </summary>
        /// <param name="itemList">The item list.</param>
        public ItemListHelper(ItemList itemList)
        {
            if (itemList == null)
            {
                throw new ArgumentNullException("itemList");
            }

            this.itemList = itemList;
            this.itemList.PART_MainListView.ItemContainerGenerator.StatusChanged += this.OnContainerStatusChanged;
        }

        /// <summary>
        /// Adds the associated collection.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        public void AddAssociatedCollection(IWorkbenchItem workbenchItem)
        {
            this.AddAssociatedCollection(new[] { workbenchItem });
        }

        /// <summary>
        /// Adds the associated collection for each workbench item.
        /// </summary>
        /// <param name="workbenchItems">The workbench items.</param>
        public void AddAssociatedCollection(IEnumerable<IWorkbenchItem> workbenchItems)
        {
            SendOrPostCallback callback = delegate
            {
                foreach (var workbenchItem in workbenchItems.ToArray())
                {
                    MultiSelectControlItemGroup controls;
                    if (this.TryGetControls(workbenchItem, out controls))
                    {
                        continue;
                    }

                    this.itemList.ControlItemGroups.Add(
                        new MultiSelectControlItemGroup(this.itemList.DataProvider.GetControlItemGroup(workbenchItem)));
                }
            };

            this.itemList.Dispatcher.Invoke(DispatcherPriority.Send, callback, null);
        }

        /// <summary>
        /// Removes the associated collection.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        public void RemoveAssociatedCollection(IWorkbenchItem workbenchItem)
        {
            this.RemoveAssociatedCollection(new[] { workbenchItem });
        }

        /// <summary>
        /// Removes the associated collection for each workbench item.
        /// </summary>
        /// <param name="workbenchItems">The workbench items.</param>
        public void RemoveAssociatedCollection(IEnumerable<IWorkbenchItem> workbenchItems)
        {
            SendOrPostCallback callback = delegate
            {
                foreach (var workbenchItem in workbenchItems.ToArray())
                {
                    MultiSelectControlItemGroup controls;
                    if (!this.TryGetControls(workbenchItem, out controls))
                    {
                        continue;
                    }

                    // Find the corresponding UI element;
                    DependencyObject container;
                    if (this.containerMap.TryGetValue(controls, out container))
                    {
                        this.containerMap.Remove(controls);

                        var disposableChilden = container.GetAllDisposableChildren();

                        foreach (var disposable in disposableChilden)
                        {
                            disposable.Dispose();
                        }
                    }

                    this.itemList.ControlItemGroups.Remove(controls);
                    controls.WorkbenchItem = null;
                    controls.Dispose();
                }
            };

            this.itemList.Dispatcher.Invoke(DispatcherPriority.Send, callback, null);
        }

        /// <summary>
        /// Removes all associated collections.
        /// </summary>
        public void RemoveAllAssociatedCollections()
        {
            this.RemoveAssociatedCollection(this.itemList.ControlItemGroups.Select(c => c.WorkbenchItem));

            foreach (var itemCollection in this.itemList.ControlItemGroups)
            {
                itemCollection.Dispose();
            }
        }

        /// <summary>
        /// Determines whether the specified workbench item is included.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>
        /// <c>true</c> if the specified workbench item is included; otherwise, <c>false</c>.
        /// </returns>
        public bool IsIncluded(IWorkbenchItem workbenchItem)
        {
            return this.IsCorrectType(workbenchItem) 
                    && this.IsSelectedUser(workbenchItem);
        }

        /// <summary>
        /// Determines whether [the specified workbenchItem item] [is correct type].
        /// </summary>
        /// <param name="workbenchItem">The workbenchItem item.</param>
        /// <returns>
        /// <c>true</c> if [the specified workbenchItem item] [is correct type]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCorrectType(IWorkbenchItem workbenchItem)
        {
            var itemType = workbenchItem.GetTypeName();
            return this.itemList.WorkbenchItemTypeName.Equals(itemType);
        }

        /// <summary>
        /// Determines whether [the specified workbench item] is assigned to [is selected user].
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>
        /// <c>true</c> if [the specified workbench item] is assigned to [is selected user]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSelectedUser(IWorkbenchItem workbenchItem)
        {
            var ownerName = workbenchItem.GetOwner();

            return ownerName == null || this.IsSelectedUser(ownerName);
        }

        /// <summary>
        /// Determines whether [the specified assigned to] [is selected user].
        /// </summary>
        /// <param name="owner">The assigned to.</param>
        /// <returns>
        /// <c>true</c> if [the specified assigned to] [is selected user]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsSelectedUser(string owner)
        {
            if (!this.itemList.PART_OwnerFilterSelector.ValueSelections.Any())
            {
                return true;
            }

            Func<SelectedValue, bool> isMatch = sv => sv.IsSelected && sv.Text.Replace(UnassignedIndicator, string.Empty).Equals(owner);

            return this.itemList.PART_OwnerFilterSelector.ValueSelections.Any(isMatch);
        }

        /// <summary>
        /// Tries the get control item collection for the specified workbench item.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="controlItemGroup">The control item collection.</param>
        /// <returns>
        /// <c>true</c> if [the specified workbench item] [has an existing collection]; otherwise, <c>false</c>.
        /// </returns>
        private bool TryGetControls(IWorkbenchItem workbenchItem, out MultiSelectControlItemGroup controlItemGroup)
        {
            controlItemGroup = this.itemList.ControlItemGroups.FirstOrDefault(cc => Equals(cc.WorkbenchItem, workbenchItem));

            return controlItemGroup != null;
        }

        /// <summary>
        /// Called when [container status changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnContainerStatusChanged(object sender, EventArgs e)
        {
            if (this.itemList.PART_MainListView.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                this.MapContainers();
            }
        }

        /// <summary>
        /// Maps the containers.
        /// </summary>
        private void MapContainers()
        {
            foreach (var itemCollection in this.itemList.PART_MainListView.Items.OfType<MultiSelectControlItemGroup>())
            {
                if (this.containerMap.ContainsKey(itemCollection))
                {
                    continue;
                }

                var container = this.itemList.PART_MainListView.ItemContainerGenerator.ContainerFromItem(itemCollection);

                if (container == null)
                {
                    continue;
                }

                this.containerMap.Add(itemCollection, container);
            }
        }
    }
}
