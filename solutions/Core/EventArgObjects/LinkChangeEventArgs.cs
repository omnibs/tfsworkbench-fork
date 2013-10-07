// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkChangeEventArgs.cs" company="None">
//   None
// </copyright>
// <summary>
//   The links changed event args class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.EventArgObjects
{
    using Interfaces;

    /// <summary>
    /// The link changed event args class.
    /// </summary>
    public class LinkChangeEventArgs : ContextEventArgs<ILinkItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkChangeEventArgs"/> class.
        /// </summary>
        /// <param name="alteredLink">The altered link.</param>
        public LinkChangeEventArgs(ILinkItem alteredLink)
            : base(alteredLink)
        {
        }
    }
}