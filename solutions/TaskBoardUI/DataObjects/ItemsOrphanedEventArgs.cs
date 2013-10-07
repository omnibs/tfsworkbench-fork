// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemsOrphanedEventArgs.cs" company="None">
//   None
// </copyright>
// <summary>
//   The items orphaned event args class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.TaskBoardUI.DataObjects
{
    using System;
    using System.Collections.Generic;

    using TfsWorkbench.Core.Interfaces;

    /// <summary>
    /// The items orphaned event args class.
    /// </summary>
    public class ItemsOrphanedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsOrphanedEventArgs"/> class.
        /// </summary>
        /// <param name="swimLaneView">The swim lane view.</param>
        /// <param name="orphanedItems">The orphaned items.</param>
        public ItemsOrphanedEventArgs(SwimLaneView swimLaneView, IEnumerable<IWorkbenchItem> orphanedItems)
        {
            this.SwimLaneView = swimLaneView;
            this.OrphanedItems = orphanedItems;
        }

        /// <summary>
        /// Gets the swim lane view.
        /// </summary>
        /// <value>The swim lane view.</value>
        public SwimLaneView SwimLaneView { get; private set; }

        /// <summary>
        /// Gets the orphaned items.
        /// </summary>
        /// <value>The orphaned items.</value>
        public IEnumerable<IWorkbenchItem> OrphanedItems { get; private set; }
    }
}