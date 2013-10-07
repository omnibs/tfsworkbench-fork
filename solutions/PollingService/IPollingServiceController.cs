// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPollingServiceController.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IChangePollerController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.PollingService
{
    using System;
    using System.Windows;

    using TfsWorkbench.Core.Interfaces;

    /// <summary>
    /// The change poller controller interface.
    /// </summary>
    public interface IPollingServiceController : IDisposable
    {
        /// <summary>
        /// Gets the change poller.
        /// </summary>
        /// <value>The change poller.</value>
        ChangePoller ChangePoller { get; }

        /// <summary>
        /// Gets a value indicating whether this instance can show dialog.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can show dialog; otherwise, <c>false</c>.
        /// </value>
        bool CanShowDialog { get; }

        /// <summary>
        /// Attaches the project data.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        void AttachProjectData(IProjectData projectData);

        /// <summary>
        /// Detaches the project data.
        /// </summary>
        void DetachProjectData();

        /// <summary>
        /// Starts the polling.
        /// </summary>
        void StartPolling();

        /// <summary>
        /// Stops the polling.
        /// </summary>
        void StopPolling();

        /// <summary>
        /// Shows the change poller control.
        /// </summary>
        /// <param name="sourceElement">The source element.</param>
        void ShowChangePollerControl(UIElement sourceElement);
    }
}