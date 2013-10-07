// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextEventArgs.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the TypedEventArgsBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.EventArgObjects
{
    using System;

    /// <summary>
    /// The type event args base class.
    /// </summary>
    /// <typeparam name="T">The event context type.</typeparam>
    public class ContextEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContextEventArgs{T}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ContextEventArgs(T context)
        {
            this.Context = context;
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public T Context { get; protected set; }
    }
}
