// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DurationStructureBase.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the DurationStructureBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI.DataObjects
{
    using System;

    /// <summary>
    /// Initialises and instance of TfsWorkbench.ProjectSetupUI.DataObjects.DurationStructureBase
    /// </summary>
    internal abstract class DurationStructureBase : NamedStructureBase
    {
        /// <summary>
        /// The start date.
        /// </summary>
        private DateTime? startDate;

        /// <summary>
        /// The end date.
        /// </summary>
        private DateTime? endDate;

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime? StartDate
        {
            get
            {
                return this.startDate;
            }

            set
            {
                this.UpdateWithNotification("StartDate", value, ref this.startDate);
            }
        }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        public DateTime? EndDate
        {
            get
            {
                return this.endDate;
            }

            set
            {
                this.UpdateWithNotification("EndDate", value, ref this.endDate);
            }
        }
    }
}