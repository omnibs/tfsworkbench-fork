// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutOfDateVersionStatus.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   The out of date version status.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Models
{
    using TfsWorkbench.VersionCheck.Properties;

    /// <summary>
    /// The out of date version status.
    /// </summary>
    internal class OutOfDateVersionStatus : VersionStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutOfDateVersionStatus"/> class.
        /// </summary>
        public OutOfDateVersionStatus()
        {
            this.Status = VersionStatusOption.OutDated;
            this.DisplayMessage = Resources.String005;
        }
    }
}