// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for App.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Threading;

    using Microsoft.Win32;

    using Properties;

    using TfsWorkbench.WpfUI.Controllers;

    using UIElements;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// The application controller intance.
        /// </summary>
        private ApplicationController applicationController;

        /// <summary>
        /// The activation status flag.
        /// </summary>
        private bool isActivated;

        /// <summary>
        /// The is showing error message flag.
        /// </summary>
        private bool isShowingErrorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">More than one instance of the <see cref="T:System.Windows.Application"/> class is created per <see cref="T:System.AppDomain"/>.</exception>
        public App()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        /// <summary>
        /// Gets a value indicating whether this instance has unhandled exception.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has unhandled exception; otherwise, <c>false</c>.
        /// </value>
        internal bool HasUnhandledException { get; private set; }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Activated"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnActivated(EventArgs e)
        {
            if (this.isActivated)
            {
                return;
            }

            this.isActivated = true;

            this.InitialiseApp();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Exit"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.ExitEventArgs"/> that contains the event data.</param>
        protected override void OnExit(ExitEventArgs e)
        {
            if (this.applicationController == null)
            {
                return;
            }

            this.applicationController.SaveProjectLayout();
        }

        /// <summary>
        /// Initialises the app.
        /// </summary>
        private void InitialiseApp()
        {
            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.PreviewMouseWheelEvent, new RoutedEventHandler(MouseWheelScrollBubbler.HandleMouseWheel), true);

            this.applicationController = new ApplicationController(Current.MainWindow as MainAppWindow);
        }

        /// <summary>
        /// Handles the DispatcherUnhandledException event of the Application.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Threading.DispatcherUnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void OnAppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (this.isShowingErrorMessage)
            {
                return;
            }

            this.isShowingErrorMessage = true;

            var sb = new StringBuilder();
            var ex = e.Exception;

            sb.AppendLine("An unhandled exception has occured and the application needs to close. Exception details:");
            sb.AppendLine();

            while (ex != null)
            {
                sb.AppendLine(ex.Message);
                ex = ex.InnerException;
            }

            sb.AppendLine();
            sb.AppendLine("Save exception details to file?");

            if (MessageBox.Show(
                sb.ToString(), Settings.Default.ApplicationTitle, MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
            {
                if (!string.IsNullOrEmpty(e.Exception.StackTrace))
                {
                    sb.AppendLine();
                    sb.AppendLine(e.Exception.StackTrace);
                }

                var fileSaveDialog = new SaveFileDialog
                    {
                        Filter = "Text File|*.txt", 
                        Title = "Save exception to file:"
                    };

                if ((bool)fileSaveDialog.ShowDialog(Current.MainWindow))
                {
                    using (var sr = new StreamWriter(fileSaveDialog.FileName, false))
                    {
                        sr.Write(sb.ToString());
                        sr.Flush();
                        sr.Close();
                    }
                }
            }

            e.Handled = true;

            this.HasUnhandledException = true;

            this.Shutdown();
        }
    }
}