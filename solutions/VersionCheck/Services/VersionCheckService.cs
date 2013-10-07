// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionCheckService.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the VersionStatusOption type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Services
{
    using System;
    using System.ComponentModel;

    using TfsWorkbench.Core.Services;
    using TfsWorkbench.VersionCheck.Iterfaces;
    using TfsWorkbench.VersionCheck.Models;

    /// <summary>
    /// The verison checkign service class.
    /// </summary>
    internal class VersionCheckService : IVersionCheckService
    {
        /// <summary>
        /// The web request reader factory instance.
        /// </summary>
        private readonly IWebRequestReaderFactory webRequestReaderFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionCheckService"/> class.
        /// </summary>
        public VersionCheckService()
            : this(ServiceManager.Instance.GetService<IWebRequestReaderFactory>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionCheckService"/> class.
        /// </summary>
        /// <param name="webRequestReaderFactory">The web request reader factory.</param>
        public VersionCheckService(IWebRequestReaderFactory webRequestReaderFactory)
        {
            if (webRequestReaderFactory == null)
            {
                throw new ArgumentNullException("webRequestReaderFactory");
            }

            this.webRequestReaderFactory = webRequestReaderFactory;
        }

        /// <summary>
        /// Begins the async version status check.
        /// </summary>
        /// <param name="callBack">The call back method.</param>
        public void BeginAsyncGetVersionStatus(Action<VersionStatus, Exception> callBack)
        {
            using (var bgw = new BackgroundWorker())
            {
                bgw.DoWork += (s, e) => e.Result = this.GetVersionStatus();

                bgw.RunWorkerCompleted += (s, e) => callBack(e.Result as VersionStatus, e.Error);

                bgw.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Compare the version strings.
        /// </summary>
        /// <param name="localVersion">The local version.</param>
        /// <returns>The version status.</returns>
        private static VersionStatus CompareVersions(string localVersion)
        {
            return IsCurrentVersion(localVersion)
                       ? (VersionStatus)new UptoDateVersionStatus()
                       : new OutOfDateVersionStatus();
        }

        /// <summary>
        /// Determines whether [the specified version] [is current version].
        /// </summary>
        /// <param name="localVersion">The local version as a string.</param>
        /// <returns>
        /// <c>true</c> if [the specified version] [is current version]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsCurrentVersion(string localVersion)
        {
            return Helpers.GetLocalCoreVersion().Equals(localVersion);
        }

        /// <summary>
        /// Gets the version status.
        /// </summary>
        /// <returns>An instance of the version status class.</returns>
        private VersionStatus GetVersionStatus()
        {
            string response;
            return !this.TryGetCurrentVersionFromRemoteServer(out response) 
                       ? new FailedToCheckVersionStatus(response)
                       : CompareVersions(response);
        }

        /// <summary>
        /// Tries to get the current application version from the remote server.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns><c>True</c> if a web response is received; otherwise <c>false</c>.</returns>
        private bool TryGetCurrentVersionFromRemoteServer(out string response)
        {
            return this.CreateWebRequestReader().TryReadFirstLine(out response);
        }

        /// <summary>
        /// Creates the web request reader.
        /// </summary>
        /// <returns>A new instance of the web request reader class.</returns>
        private IWebRequestReader CreateWebRequestReader()
        {
            return this.webRequestReaderFactory.Create();
        }
    }
}
