// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemListHelper.cs" company="EMC Consulting">
//   EMC Consulting 2010
// </copyright>
// <summary>
//   Defines the ItemListHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Emcc.ScrumMastersWorkbench.ItemListUI
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Windows.Threading;

    using Core.DataObjects;
    using Core.EventArgObjects;
    using Core.Helpers;
    using Core.Interfaces;

    using Emcc.ScrumMastersWorkbench.UIElements;

    /// <summary>
    /// Initializes instance of ItemListHelper
    /// </summary>
    internal class ItemListHelper
    {
        /// <summary>
        /// The assigned to field name.
        /// </summary>
        public const string AssignedToFieldName = "System.AssignedTo";

        /// <summary>
        /// The unassigned indicator text.
        /// </summary>
        public const string UnassignedIndicator = "{Unassigned}";

        /// <summary>
        /// The data provider.
        /// </summary>
        private readonly IDataProvider dataProvider;

        /// <summary>
        /// The dispatcher.
        /// </summary>
        private readonly Dispatcher dispatcher;

        /// <summary>
        /// The work bench item type.
        /// </summary>
        private readonly string workbenchItemType;

        /// <summary>
        /// The control collection.
        /// </summary>
        private readonly ObservableCollection<IControlItemCollection> controlItemCollections;

        /// <summary>
        /// The selected state collection.
        /// </summary>
        private readonly IEnumerable<SelectedValue> selectedStates;

        /// <summary>
        /// The assigned to selector.
        /// </summary>
        private readonly ValueSelectorControl assignedToSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemListHelper"/> class.
        /// </summary>
        /// <param name="workbenchItemType">Type of the work bench item.</param>
        /// <param name="controlItemCollections">The control collection.</param>
        /// <param name="selectedStates">The selected states.</param>
        /// <param name="assignedToSelector">The assigned to selector.</param>
        /// <param name="dataProvider">The data provider.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        public ItemListHelper(
            string workbenchItemType,
            ObservableCollection<IControlItemCollection> controlItemCollections, 
            IEnumerable<SelectedValue> selectedStates,
            ValueSelectorControl assignedToSelector,
            IDataProvider dataProvider,
            Dispatcher dispatcher)
        {
            if (controlItemCollections == null)
            {
                throw new ArgumentNullException("controlItemCollections");
            }

            if (selectedStates == null)
            {
                throw new ArgumentNullException("selectedStates");
            }

            if (assignedToSelector == null)
            {
                throw new ArgumentNullException("assignedToSelector");
            }

            if (dataProvider == null)
            {
                throw new ArgumentNullException("dataProvider");
            }

            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            this.workbenchItemType = workbenchItemType;
            this.dataProvider = dataProvider;
            this.dispatcher = dispatcher;
            this.controlItemCollections = controlItemCollections;
            this.selectedStates = selectedStates;
            this.assignedToSelector = assignedToSelector;
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
                    IControlItemCollection controls;
                    if (this.TryGetControls(workbenchItem, out controls))
                    {
                        continue;
                    }

                    this.controlItemCollections.Add(this.dataProvider.GetControlItemCollection(workbenchItem));

                    workbenchItem.StateChanged += this.OnStateChanged;
                }
            };

            this.dispatcher.Invoke(DispatcherPriority.Send, callback, null);
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
                    workbenchItem.StateChanged -= this.OnStateChanged;

                    IControlItemCollection controls;
                    if (!this.TryGetControls(workbenchItem, out controls))
                    {
                        continue;
                    }

                    this.controlItemCollections.Remove(controls);
                    controls.WorkbenchItem = null;
                }
            };

            this.dispatcher.Invoke(DispatcherPriority.Send, callback, null);
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
            var itemType = WorkbenchItemHelper.GetType(workbenchItem);
            return this.workbenchItemType.Equals(itemType);
        }

        /// <summary>
        /// Determines whether [the specified workbench item] [is selected state].
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>
        /// <c>true</c> if [the specified workbench item] [is selected state]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSelectedState(IWorkbenchItem workbenchItem)
        {
            return this.IsSelectedState(WorkbenchItemHelper.GetState(workbenchItem));
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
            var assignedTo = workbenchItem[AssignedToFieldName];

            return assignedTo == null || this.IsSelectedUser(assignedTo.ToString());
        }

        /// <summary>
        /// Determines whether [the specified assigned to] [is selected user].
        /// </summary>
        /// <param name="assignedTo">The assigned to.</param>
        /// <returns>
        /// <c>true</c> if [the specified assigned to] [is selected user]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsSelectedUser(string assignedTo)
        {
            if (!this.assignedToSelector.ValueSelections.Any())
            {
                return true;
            }

            Func<SelectedValue, bool> isMatch = sv => sv.IsSelected && sv.Value.Replace(UnassignedIndicator, string.Empty).Equals(assignedTo);

            return this.assignedToSelector.ValueSelections.Any(isMatch);
        }

        /// <summary>
        /// Determines whether [the specified state] [is selected state].
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>
        /// <c>true</c> if [the specified state] [is selected state]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsSelectedState(string state)
        {
            return this.selectedStates.Any(ss => ss.IsSelected && ss.Value.Equals(state));
        }

        /// <summary>
        /// Called when [state changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Emcc.ScrumMastersWorkbench.Core.EventArgObjects.ItemStateChangeEventArgs"/> instance containing the event data.</param>
        private void OnStateChanged(object sender, ItemStateChangeEventArgs e)
        {
            var hasBeenExcluded = this.IsSelectedState(e.OldState) & !this.IsSelectedState(e.NewState);

            if (hasBeenExcluded)
            {
                this.RemoveAssociatedCollection(e.Item);
            }
        }

        /// <summary>
        /// Tries the get control item collection for the specfied workbench item.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="controlItemCollection">The control item collection.</param>
        /// <returns>
        /// <c>true</c> if [the specified workbench item] [has an existing collection]; otherwise, <c>false</c>.
        /// </returns>
        private bool TryGetControls(IWorkbenchItem workbenchItem, out IControlItemCollection controlItemCollection)
        {
            controlItemCollection = this.controlItemCollections.FirstOrDefault(cc => Equals(cc.WorkbenchItem, workbenchItem));

            return controlItemCollection != null;
        }
    }
}
