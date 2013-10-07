// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DragDataEventArgs.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the DragDataEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.DragHelpers
{
    using System;
    using System.Collections.Generic;

    using UIElements.DragHelpers;

    /// <summary>
    /// Initializes instance of DragDataEventArgs&lt;TDataType&gt;
    /// </summary>
    /// <typeparam name="TDataType">The type of the data type.</typeparam>
    public class DragDataEventArgs<TDataType> : EventArgs
    {
        /// <summary>
        /// The internal source item field.
        /// </summary>
        private readonly IDragTarget<TDataType> source;

        /// <summary>
        /// The internal target item field.
        /// </summary>
        private readonly IDragTarget<TDataType> target;

        /// <summary>
        /// The dragged items collection.
        /// </summary>
        private readonly IEnumerable<TDataType> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="DragDataEventArgs&lt;TDataType&gt;"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="items">The items.</param>
        public DragDataEventArgs(IDragTarget<TDataType> source, IDragTarget<TDataType> target, IEnumerable<TDataType> items)
        {
            this.source = source;
            this.target = target;
            this.items = items;
        }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>The source.</value>
        public IDragTarget<TDataType> Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        public IDragTarget<TDataType> Target
        {
            get { return this.target; }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public IEnumerable<TDataType> Items
        {
            get { return this.items; }
        }
    }
}