// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FailedToCheckVersionStatus.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the FailedToCheckVersionStatus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Models
{
    using TfsWorkbench.VersionCheck.Properties;

    /// <summary>
    /// The failed to check version status class.
    /// </summary>
    internal class FailedToCheckVersionStatus : VersionStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToCheckVersionStatus"/> class.
        /// </summary>
        /// <param name="failureMessage">The failure message.</param>
        public FailedToCheckVersionStatus(string failureMessage)
        {
            this.Status = VersionStatusOption.Unknown;
            this.DisplayMessage = string.Concat(Resources.String006, failureMessage);
        }
    }
}