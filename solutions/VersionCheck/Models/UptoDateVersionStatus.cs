// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UptoDateVersionStatus.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   The up to date version status.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Models
{
    using TfsWorkbench.VersionCheck.Properties;

    /// <summary>
    /// The up to date version status.
    /// </summary>
    internal class UptoDateVersionStatus : VersionStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UptoDateVersionStatus"/> class.
        /// </summary>
        public UptoDateVersionStatus()
        {
            this.Status = VersionStatusOption.UpToDate;
            this.DisplayMessage = Resources.String004;
        }
    }
}