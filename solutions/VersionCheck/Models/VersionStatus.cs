// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionStatus.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the VersionStatus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Models
{
    /// <summary>
    /// The version status class.
    /// </summary>
    public abstract class VersionStatus
    {
        /// <summary>
        /// Gets or sets the display message.
        /// </summary>
        /// <value>The display message.</value>
        public string DisplayMessage { get; protected set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public VersionStatusOption Status { get; protected set; }
    }
}
