// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkbenchItemRepository.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IWorkbenchItemRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Interfaces
{
    using System;
    using System.Collections.Generic;

    using TfsWorkbench.Core.EventArgObjects;

    /// <summary>
    /// The workbench item repository interface.
    /// </summary>
    public interface IWorkbenchItemRepository : IEnumerable<IWorkbenchItem>
    {
        /// <summary>
        /// Occurs when [link change].
        /// </summary>
        event EventHandler<ItemLinkChangeEventArgs> LinkChanged;

        /// <summary>
        /// Occurs when [item state changed].
        /// </summary>
        event EventHandler<ItemStateChangeEventArgs> ItemStateChanged;

        /// <summary>
        /// Occurs when [filters changed].
        /// </summary>
        event EventHandler FiltersChanged;

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        event EventHandler<RepositoryChangedEventArgs<IWorkbenchItem>> CollectionChanged;

        /// <summary>
        /// Gets the unfiltered workbench items.
        /// </summary>
        /// <value>The unfiltered workbench items.</value>
        IEnumerable<IWorkbenchItem> UnfilteredList { get; }

        /// <summary>
        /// Adds the specified workbench item.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        void Add(IWorkbenchItem workbenchItem);

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="workbenchItems">The workbench items.</param>
        void AddRange(IEnumerable<IWorkbenchItem> workbenchItems);

        /// <summary>
        /// Removes the specified workbench item.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        void Remove(IWorkbenchItem workbenchItem);

        /// <summary>
        /// Removes the range.
        /// </summary>
        /// <param name="workbenchItems">The workbench items.</param>
        void RemoveRange(IEnumerable<IWorkbenchItem> workbenchItems);

        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();

        /// <summary>
        /// Called when [refresh collection].
        /// </summary>
        void OnRefreshCollection();

        /// <summary>
        /// Called when [workbench item state changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.ItemStateChangeEventArgs"/> instance containing the event data.</param>
        void OnItemStateChanged(object sender, ItemStateChangeEventArgs e);
    }
}