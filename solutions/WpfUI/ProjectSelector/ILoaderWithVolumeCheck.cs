// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILoaderWithVolumeCheck.cs" company="None">
//   None.
// </copyright>
// <summary>
//   Defines the IProjectSelectorLoader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.ProjectSelector
{
    using System;

    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.UIElements;

    /// <summary>
    /// The project selector loader interface.
    /// </summary>
    public interface ILoaderWithVolumeCheck : IDisposable
    {
        /// <summary>
        /// Occurs when [aborted].
        /// </summary>
        event EventHandler Aborted;

        /// <summary>
        /// Occurs when process complete.
        /// </summary>
        event EventHandler Complete;

        /// <summary>
        /// Occurs when [volume warning].
        /// </summary>
        event EventHandler<ContextEventArgs<DecisionControl>> VolumeWarning;

        /// <summary>
        /// Occurs when [close current project] is reached in the load process.
        /// </summary>
        event EventHandler<CancelEventArgs> ConfirmLoadData;

        /// <summary>
        /// Gets a value indicating whether to [ignore future volume warnings].
        /// </summary>
        /// <value>
        /// <c>true</c> if [ignore future volume warnings]; otherwise, <c>false</c>.
        /// </value>
        bool IgnoreFutureVolumeWarnings { get; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start();
    }
}