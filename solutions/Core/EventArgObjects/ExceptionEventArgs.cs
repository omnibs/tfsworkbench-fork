// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionEventArgs.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ExceptionEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.EventArgObjects
{
    using System;

    /// <summary>
    /// The exception event args class.
    /// </summary>
    public class ExceptionEventArgs : ContextEventArgs<Exception>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionEventArgs"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public ExceptionEventArgs(Exception error)
            : base(error)
        {
        }
    }
}