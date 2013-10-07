// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApplicationController.cs" company="None">
//   None
// </copyright>
// <summary>
//   The application controller interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.Controllers
{
    using System.Collections.Generic;

    using TfsWorkbench.Core.Interfaces;

    /// <summary>
    /// The application controller interface.
    /// </summary>
    public interface IApplicationController
    {
        /// <summary>
        /// Gets the main window.
        /// </summary>
        /// <value>The main window.</value>
        MainAppWindow MainWindow { get; }

        /// <summary>
        /// Gets the data provider helper.
        /// </summary>
        /// <value>The data provider helper.</value>
        IDataProviderController DataProviderController { get; }

        /// <summary>
        /// Gets the dialog controller.
        /// </summary>
        /// <value>The dialog controller.</value>
        IDialogController DialogController { get; }

        /// <summary>
        /// Saves the project layout.
        /// </summary>
        void SaveProjectLayout();

        /// <summary>
        /// Workbenches the item save error.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="errors">The errors.</param>
        void WorkbenchItemSaveError(IWorkbenchItem workbenchItem, IEnumerable<string> errors);

        /// <summary>
        /// Sets the status message.
        /// </summary>
        /// <param name="message">The message.</param>
        void SetStatusMessage(string message);

        /// <summary>
        /// Closes the project.
        /// </summary>
        void CloseProject();

        /// <summary>
        /// Enables main window input.
        /// </summary>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        void EnableInput(bool enable);

        /// <summary>
        /// Discards the unsaved changes.
        /// </summary>
        /// <returns><c>True</c> if changes are to be discarded; otherwise <c>false</c></returns>
        bool DiscardAnyUnsavedChanges();

        /// <summary>
        /// Sets the active display activeDisplayMode.
        /// </summary>
        /// <param name="activeDisplayMode">The active display element.</param>
        void SetActiveDisplayMode(IDisplayMode activeDisplayMode);

        /// <summary>
        /// Applies the loaded data.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        void ApplyLoadedData(IProjectData projectData);
    }
}