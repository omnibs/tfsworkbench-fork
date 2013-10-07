// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IElementDragController.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IElementDragController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.DragHelpers
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The element drag controller interface.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    public interface IElementDragController<TElement>
    {
        /// <summary>
        /// Gets or sets the item drag failed.
        /// </summary>
        /// <value>The item drag failed.</value>
        event EventHandler<DragDataEventArgs<TElement>> ItemDragFailed;

        /// <summary>
        /// Gets or sets the items dragged.
        /// </summary>
        /// <value>The items dragged.</value>
        event EventHandler<DragDataEventArgs<TElement>> ItemsDragged;

        /// <summary>
        /// Gets or sets the items dropped.
        /// </summary>
        /// <value>The items dropped.</value>
        event EventHandler<DragDataEventArgs<TElement>> ItemsDropped;

        /// <summary>
        /// Gets the drag targets.
        /// </summary>
        /// <value>The drag targets.</value>
        IEnumerable<IDragTarget<TElement>> DragTargets { get; }

        /// <summary>
        /// Registers the drag target.
        /// </summary>
        /// <param name="dragTarget">The drag target.</param>
        void RegisterDragTarget(IDragTarget<TElement> dragTarget);

        /// <summary>
        /// Releases the drag target.
        /// </summary>
        /// <param name="dragTarget">The drag target.</param>
        void ReleaseDragTarget(IDragTarget<TElement> dragTarget);

        /// <summary>
        /// Unhooks the drag events.
        /// </summary>
        void ReleaseResources();
    }
}