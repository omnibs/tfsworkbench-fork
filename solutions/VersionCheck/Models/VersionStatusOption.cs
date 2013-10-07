// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionStatusOption.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   The version status options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Models
{
    /// <summary>
    /// The version status options.
    /// </summary>
    public enum VersionStatusOption
    {
        /// <summary>
        /// The version status is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The version is up to date.
        /// </summary>
        UpToDate = 1,

        /// <summary>
        /// The version is out dated.
        /// </summary>
        OutDated = 2
    }
}