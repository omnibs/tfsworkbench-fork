// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancelEventArgs.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the CancelEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.ProjectSelector
{
    using TfsWorkbench.Core.EventArgObjects;

    /// <summary>
    /// The cancel context event args class.
    /// </summary>
    public class CancelEventArgs : ContextEventArgs<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelEventArgs"/> class.
        /// </summary>
        public CancelEventArgs()
            : base(false)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CancelEventArgs"/> is cancelled.
        /// </summary>
        /// <value><c>true</c> if cancelled; otherwise, <c>false</c>.</value>
        public bool Cancel
        {
            get
            {
                return this.Context;
            }

            set
            {
                this.Context = value;
            }
        }
    }
}
