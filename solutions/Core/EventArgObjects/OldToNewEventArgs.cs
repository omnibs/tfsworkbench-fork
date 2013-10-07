// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OldToNewEventArgs.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ItemChangeEventArgsBaseBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.EventArgObjects
{
    using System;

    /// <summary>
    /// The old to new event args base class.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    public class OldToNewEventArgs<T> : ContextEventArgs<Tuple<T, T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OldToNewEventArgs{T}"/> class.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public OldToNewEventArgs(T oldValue, T newValue)
            : base(new Tuple<T, T>(oldValue, newValue))
        {
        }

        /// <summary>
        /// Gets the old value.
        /// </summary>
        /// <value>The old value.</value>
        public T OldValue
        {
            get
            {
                return this.Context.Item1;
            }
        }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        /// <value>The new value.</value>
        public T NewValue
        {
            get
            {
                return this.Context.Item2;
            }
        }
    }
}