// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemLinksChangedEventArgs.cs" company="EMC Consulting">
//   EMC Consulting 2010
// </copyright>
// <summary>
//   The item links changed event args class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Emcc.ScrumMastersWorkbench.Core.EventArgObjects
{
    using System;

    using Interfaces;

    /// <summary>
    /// The item links changed event args class.
    /// </summary>
    public class ItemLinksChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemLinksChangedEventArgs"/> class.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        public ItemLinksChangedEventArgs(IWorkbenchItem workbenchItem)
        {
            this.WorkbenchItem = workbenchItem;
        }

        /// <summary>
        /// Gets the workbench item.
        /// </summary>
        /// <value>The workbench item.</value>
        public IWorkbenchItem WorkbenchItem { get; private set; }
    }
}