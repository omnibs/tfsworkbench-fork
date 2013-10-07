// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiSelectControlItemGroup.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the MultiSelectControlItemGroup type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ItemListUI
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Core.Interfaces;

    /// <summary>
    /// The multi select control item collection.
    /// </summary>
    public class MultiSelectControlItemGroup : IControlItemGroup, IDisposable
    {
        /// <summary>
        /// The base control collection.
        /// </summary>
        private IControlItemGroup baseGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiSelectControlItemGroup"/> class.
        /// </summary>
        /// <param name="baseGroup">The base collection.</param>
        public MultiSelectControlItemGroup(IControlItemGroup baseGroup)
        {
            if (baseGroup == null)
            {
                throw new ArgumentNullException("baseGroup");
            }

            this.ControlItems = new Collection<IControlItem>();

            this.baseGroup = baseGroup;

            foreach (var controlItem in this.baseGroup.ControlItems)
            {
                this.ControlItems.Add(new MultiSelectControlItem(controlItem));
            }
        }

        /// <summary>
        /// Gets the control items.
        /// </summary>
        /// <value>The control items.</value>
        public ICollection<IControlItem> ControlItems { get; private set; }

        /// <summary>
        /// Gets or sets the task board item.
        /// </summary>
        /// <value>The task board item.</value>
        public IWorkbenchItem WorkbenchItem
        {
            get { return this.baseGroup.WorkbenchItem; }
            set { this.baseGroup.WorkbenchItem = value; }
        }

        /// <summary>
        /// Gets the multi select control items.
        /// </summary>
        /// <value>The multi select control items.</value>
        public IEnumerable<MultiSelectControlItem> MultiSelectControlItems
        {
            get { return this.ControlItems.OfType<MultiSelectControlItem>(); }
        }

        /// <summary>
        /// Gets the <see cref="TfsWorkbench.Core.Interfaces.IControlItem"/> at the specified index.
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <value>A control item.</value>
        public IControlItem this[int index]
        {
            get
            {
                return this.ControlItems.ElementAt(index);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            foreach (var controlItem in this.MultiSelectControlItems)
            {
                controlItem.Dispose();
            }

            this.ControlItems.Clear();
            this.WorkbenchItem = null;
            this.baseGroup = null;
        }
    }
}
