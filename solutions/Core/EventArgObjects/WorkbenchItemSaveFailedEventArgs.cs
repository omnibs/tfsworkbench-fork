// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkbenchItemSaveFailedEventArgs.cs" company="None">
//   None
// </copyright>
// <summary>
//   The save failed event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.EventArgObjects
{
    using System;
    using System.Collections.Generic;

    using Interfaces;

    /// <summary>
    /// The save failed event args.
    /// </summary>
    public class WorkbenchItemSaveFailedEventArgs : ContextEventArgs<Tuple<IWorkbenchItem, IEnumerable<string>>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchItemSaveFailedEventArgs"/> class.
        /// </summary>
        /// <param name="workbenchItem">The task board item.</param>
        /// <param name="errors">The errors.</param>
        public WorkbenchItemSaveFailedEventArgs(IWorkbenchItem workbenchItem, IEnumerable<string> errors)
            : base(new Tuple<IWorkbenchItem, IEnumerable<string>>(workbenchItem, errors))
        {
        }

        /// <summary>
        /// Gets WorkbenchItem.
        /// </summary>
        /// <value>The task board item.</value>
        public IWorkbenchItem WorkbenchItem
        {
            get
            {
                return this.Context.Item1;
            }
        }

        /// <summary>
        /// Gets the save Errors.
        /// </summary>
        /// <value>The errors.</value>
        public IEnumerable<string> Errors
        {
            get
            {
                return this.Context.Item2;
            }
        }
    }
}