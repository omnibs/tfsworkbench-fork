// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMainViewModel.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   The main view model interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Iterfaces
{
    using System.ComponentModel;

    using TfsWorkbench.VersionCheck.ViewModels;

    /// <summary>
    /// The main view model interface.
    /// </summary>
    public interface IMainViewModel
    {
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether [check version on start up].
        /// </summary>
        /// <value>
        /// <c>true</c> if [check version on start up]; otherwise, <c>false</c>.
        /// </value>
        bool CheckVersionOnStartUp { get; set; }

        /// <summary>
        /// Gets the header text.
        /// </summary>
        /// <value>The header text.</value>
        string MenuHeaderText { get; }

        /// <summary>
        /// Gets the toggle start up check header text.
        /// </summary>
        /// <value>The toggle start up check header text.</value>
        string ToggleStartUpCheckHeaderText { get; }

        /// <summary>
        /// Gets the button tool tip.
        /// </summary>
        /// <value>The button tool tip.</value>
        string ButtonToolTip { get; }

        /// <summary>
        /// Gets the button content text.
        /// </summary>
        /// <value>The button content text.</value>
        string ButtonContentText { get; }

        /// <summary>
        /// Gets the version check command.
        /// </summary>
        /// <value>The version check command.</value>
        CommandViewModel VersionCheckCommand { get; }

        /// <summary>
        /// Gets the go to download page command.
        /// </summary>
        /// <value>The go to download page command.</value>
        CommandViewModel GoToDownloadPageCommand { get; }

        /// <summary>
        /// Executes the version check.
        /// </summary>
        void ExecuteVersionCheck();
    }
}