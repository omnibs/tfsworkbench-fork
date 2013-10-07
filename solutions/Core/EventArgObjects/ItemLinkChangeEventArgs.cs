// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemLinkChangeEventArgs.cs" company="None">
//   None
// </copyright>
// <summary>
//   The item link change event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.EventArgObjects
{
    using Interfaces;

    /// <summary>
    /// The item link change event args.
    /// </summary>
    public class ItemLinkChangeEventArgs : WorkbenchItemOldToNewEventArgs<ILinkItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemLinkChangeEventArgs"/> class.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="oldLink">The old link.</param>
        /// <param name="newLink">The new link.</param>
        public ItemLinkChangeEventArgs(IWorkbenchItem workbenchItem, ILinkItem oldLink, ILinkItem newLink)
            : base(workbenchItem, oldLink, newLink)
        {
        }
    }
}
