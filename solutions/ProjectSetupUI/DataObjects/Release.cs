// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Release.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the Release type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI.DataObjects
{
    using System.Linq;

    using Helpers;

    /// <summary>
    /// Initializes instance of Release
    /// </summary>
    internal class Release : DurationStructureBase
    {
        /// <summary>
        /// The workStreams collection.
        /// </summary>
        private readonly WorkStreamCollection workStreams = new WorkStreamCollection();

        /// <summary>
        /// Gets the work streams.
        /// </summary>
        /// <value>The work streams.</value>
        public WorkStreamCollection WorkStreams
        {
            get
            {
                return this.workStreams;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has valid work streams.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has valid work streams; otherwise, <c>false</c>.
        /// </value>
        public bool HasValidWorkStreams
        {
            get
            {
                return this.WorkStreams.Count() != 0 
                    && this.WorkStreams.All(ValidationHelper.IsValidWorkStream);
            }
        }
    }
}
