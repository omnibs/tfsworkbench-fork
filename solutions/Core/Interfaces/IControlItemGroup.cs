// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IControlItemGroup.cs" company="None">
//   None
// </copyright>
// <summary>
//   The control item collection
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// The control item collection
    /// </summary>
    public interface IControlItemGroup
    {
        /// <summary>
        /// Gets the control items.
        /// </summary>
        /// <value>The control items.</value>
        ICollection<IControlItem> ControlItems { get; }

        /// <summary>
        /// Gets or sets the task board item.
        /// </summary>
        /// <value>The task board item.</value>
        IWorkbenchItem WorkbenchItem { get; set; }

        /// <summary>
        /// Gets the <see cref="TfsWorkbench.Core.Interfaces.IControlItem"/> at the specified index.
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <value>A control item.</value>
        IControlItem this[int index]
        {
            get;
        }
    }
}