// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IVersionCheckService.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the IVersionCheckService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Iterfaces
{
    using System;

    using TfsWorkbench.VersionCheck.Models;

    /// <summary>
    /// The version check service interface.
    /// </summary>
    public interface IVersionCheckService
    {
        /// <summary>
        /// Begins the async version status check.
        /// </summary>
        /// <param name="callBack">The call back.</param>
        void BeginAsyncGetVersionStatus(Action<VersionStatus, Exception> callBack);
    }
}