// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemStateChangeEventArgs.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ItemStateChangeEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.EventArgObjects
{
    using Interfaces;

    /// <summary>
    /// Initializes instance of ItemStateChangeEventArgs
    /// </summary>
    public class ItemStateChangeEventArgs : WorkbenchItemOldToNewEventArgs<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemStateChangeEventArgs"/> class.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="oldState">The old state.</param>
        /// <param name="newState">The new state.</param>
        public ItemStateChangeEventArgs(IWorkbenchItem workbenchItem, string oldState, string newState)
            : base(workbenchItem, oldState, newState)
        {
        }
    }
}