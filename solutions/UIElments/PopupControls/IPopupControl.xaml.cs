// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPopupControl.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   The popup control interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.PopupControls
{
    using System.Windows;

    /// <summary>
    /// The popup control interface.
    /// </summary>
    public interface IPopupControl
    {
        /// <summary>
        /// Called when [handle mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnHandleMouseDown(object sender, RoutedEventArgs e);
    }
}