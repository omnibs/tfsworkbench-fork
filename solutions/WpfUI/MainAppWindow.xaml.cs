// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainAppWindow.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for MainAppWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI
{
    using System.Windows;

    using TfsWorkbench.WpfUI.Controllers;
    using TfsWorkbench.WpfUI.Properties;

    /// <summary>
    /// Interaction logic for MainAppWindow.xaml
    /// </summary>
    public partial class MainAppWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainAppWindow"/> class.
        /// </summary>
        public MainAppWindow()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the event data.</param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            var hasUnhandledException = ((App)Application.Current).HasUnhandledException;

            if (!hasUnhandledException && !ApplicationController.Instance.DiscardAnyUnsavedChanges())
            {
                e.Cancel = true;
                return;
            }

            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }

            Settings.Default.Save();
        }
    }
}