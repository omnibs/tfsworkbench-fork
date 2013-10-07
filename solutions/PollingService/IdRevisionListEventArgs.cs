// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdRevisionListEventArgs.cs" company="None">
//   None
// </copyright>
// <summary>
//   The item id list event args class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.PollingService
{
    using System.Collections.Generic;

    using TfsWorkbench.Core.DataObjects;
    using TfsWorkbench.Core.EventArgObjects;

    /// <summary>
    /// The item id list event args class.
    /// </summary>
    public class IdRevisionListEventArgs : ContextEventArgs<IEnumerable<IdAndRevision>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdRevisionListEventArgs"/> class.
        /// </summary>
        /// <param name="ids">The item ids and revision numbers.</param>
        public IdRevisionListEventArgs(IEnumerable<IdAndRevision> ids)
            : base(ids)
        {
        }
    }
}