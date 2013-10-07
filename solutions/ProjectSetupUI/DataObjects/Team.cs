// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Team.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the Team type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI.DataObjects
{
    using System.ComponentModel;

    /// <summary>
    /// Initializes instance of Team
    /// </summary>
    internal class Team : DurationStructureBase
    {
        /// <summary>
        /// The capacity field.
        /// </summary>
        private int? capacity;

        /// <summary>
        /// Gets or sets the capacity.
        /// </summary>
        /// <value>The capacity.</value>
        public int? Capacity
        {
            get
            {
                return this.capacity;
            }

            set
            {
                this.UpdateWithNotification("Capacity", value, ref this.capacity);
            }
        }

        /// <summary>
        /// Gets or sets the work stream.
        /// </summary>
        /// <value>The work stream.</value>
        public WorkStream WorkStream { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has valid capacity.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has valid capacity; otherwise, <c>false</c>.
        /// </value>
        public bool HasValidCapacity
        {
            get
            {
                return this.capacity.HasValue && this.capacity.Value > 0;
            }
        }
    }
}
