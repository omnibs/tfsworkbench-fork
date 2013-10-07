// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDialogController.cs" company="None">
//   None
// </copyright>
// <summary>
//   The dialog controller interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.Controllers
{
    using System.Windows;

    /// <summary>
    /// The dialog controller interface.
    /// </summary>
    public interface IDialogController
    {
        /// <summary>
        /// Gets a value indicating whether this instance is displaying modal dialog.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is displaying modal dialog; otherwise, <c>false</c>.
        /// </value>
        bool IsDisplayingModalDialog { get; }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="dialogContent">Content of the dialog.</param>
        void ShowDialog(FrameworkElement dialogContent);

        /// <summary>
        /// Closes the dialog.
        /// </summary>
        /// <param name="dialogElement">The dialog element.</param>
        void CloseDialog(FrameworkElement dialogElement);

        /// <summary>
        /// Closes all dialogs.
        /// </summary>
        void CloseAllDialogs();
    }
}