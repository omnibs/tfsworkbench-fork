// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILinkManagerService.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ILinkManagerService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Interfaces
{
    using System;
    using System.Collections.Generic;

    using TfsWorkbench.Core.EventArgObjects;

    /// <summary>
    /// The link manager service instance.
    /// </summary>
    public interface ILinkManagerService
    {
        /// <summary>
        /// Occurs when [link added].
        /// </summary>
        event EventHandler<LinkChangeEventArgs> LinkAdded;

        /// <summary>
        /// Occurs when [link removed].
        /// </summary>
        event EventHandler<LinkChangeEventArgs> LinkRemoved;

        /// <summary>
        /// Gets the links.
        /// </summary>
        /// <value>The links.</value>
        IEnumerable<ILinkItem> Links { get; }

        /// <summary>
        /// Gets the added links.
        /// </summary>
        /// <value>The added links.</value>
        IEnumerable<ILinkItem> AddedLinks { get; }

        /// <summary>
        /// Gets the deleted links.
        /// </summary>
        /// <value>The deleted links.</value>
        IEnumerable<ILinkItem> DeletedLinks { get; }

        /// <summary>
        /// Adds the link and broadcasts the change.
        /// </summary>
        /// <param name="link">The link item.</param>
        void AddLink(ILinkItem link);

        /// <summary>
        /// Adds the link.
        /// </summary>
        /// <param name="link">The item link.</param>
        void AddLinkWithoutRasingChangeEvent(ILinkItem link);

        /// <summary>
        /// Marks the link as Removed and broadcasts the change.
        /// </summary>
        /// <param name="link">The item link.</param>
        void RemoveLink(ILinkItem link);

        /// <summary>
        /// Clears the dirty link.
        /// </summary>
        /// <param name="link">The link item.</param>
        void ClearDirtyLink(ILinkItem link);

        /// <summary>
        /// Sets the initial link status.
        /// </summary>
        void SetInitialLinkStatus();

        /// <summary>
        /// Removes the links.
        /// </summary>
        void ClearAllLinks();

        /// <summary>
        /// Removes the links.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        void ClearLinks(IWorkbenchItem workbenchItem);

        /// <summary>
        /// Syncs the links.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="actualLinks">The actual links.</param>
        void SyncLinks(IWorkbenchItem workbenchItem, IEnumerable<ILinkItem> actualLinks);
    }
}