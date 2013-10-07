// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownVersionStatus.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   The unknown vesion status.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Models
{
    using TfsWorkbench.VersionCheck.Properties;

    /// <summary>
    /// The unknown vesion status.
    /// </summary>
    internal class UnknownVersionStatus : VersionStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownVersionStatus"/> class.
        /// </summary>
        public UnknownVersionStatus()
        {
            this.Status = VersionStatusOption.Unknown;
            this.DisplayMessage = Resources.String003;
        }
    }
}