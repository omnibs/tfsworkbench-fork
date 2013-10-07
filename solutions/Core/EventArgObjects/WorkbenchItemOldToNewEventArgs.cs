// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkbenchItemOldToNewEventArgsBase.cs" company="None">
//   None
// </copyright>
// <summary>
//   The item change event args base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.EventArgObjects
{
    using Interfaces;

    /// <summary>
    /// The item change event args base.
    /// </summary>
    /// <typeparam name="T">The changed value type.</typeparam>
    public class WorkbenchItemOldToNewEventArgs<T> : OldToNewEventArgs<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchItemOldToNewEventArgs{T}"/> class.
        /// </summary>
        /// <param name="workbenchWorkbenchItem">The workbench item.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected WorkbenchItemOldToNewEventArgs(IWorkbenchItem workbenchWorkbenchItem, T oldValue, T newValue)
            : base(oldValue, newValue)
        {
            this.WorkbenchItem = workbenchWorkbenchItem;
        }

        /// <summary>
        /// Gets the workbench item.
        /// </summary>
        /// <value>The workbench item.</value>
        public IWorkbenchItem WorkbenchItem { get; private set; }
    }
}