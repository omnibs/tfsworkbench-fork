// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryChangedEventArgs.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the NotifyWorkbenchItemCollectionChangedEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.EventArgObjects
{
    using System.Collections.Generic;

    using TfsWorkbench.Core.Helpers;

    /// <summary>
    /// The repository Changed Event Args class.
    /// </summary>
    /// <typeparam name="T">The repository data type.</typeparam>
    public class RepositoryChangedEventArgs<T> : ContextEventArgs<IEnumerable<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryChangedEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="action">The change.</param>
        /// <param name="affectedItems">The affected items.</param>
        public RepositoryChangedEventArgs(ChangeActionOption action, IEnumerable<T> affectedItems)
            : base(affectedItems)
        {
            this.Action = action;
        }

        /// <summary>
        /// Gets the change.
        /// </summary>
        /// <value>The change.</value>
        public ChangeActionOption Action { get; private set; }
    }
}
