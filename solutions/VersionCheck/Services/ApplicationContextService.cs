// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationContextService.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   The default dispatcher proxy class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Services
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    using TfsWorkbench.UIElements;
    using TfsWorkbench.VersionCheck.Iterfaces;
    using TfsWorkbench.VersionCheck.Properties;

    /// <summary>
    /// The application context service class.
    /// </summary>
    internal class ApplicationContextService : IApplicationContextService
    {
        /// <summary>
        /// The command root element.
        /// </summary>
        private readonly UIElement rootElement;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationContextService"/> class.
        /// </summary>
        public ApplicationContextService()
            : this(Application.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationContextService"/> class.
        /// </summary>
        /// <param name="application">The current application.</param>
        public ApplicationContextService(Application application)
        {
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            if (application.MainWindow == null)
            {
                throw new ArgumentException(Resources.String015);
            }

            this.rootElement = application.MainWindow;
        }

        /// <summary>
        /// Gets the user decision on wether to open the download page.
        /// </summary>
        /// <param name="callBack">The call back.</param>
        public void DoesUserWantToOpenDownloadPage(Action<bool, bool> callBack)
        {
            var decisionControl = new DecisionControl
                {
                    Caption = Resources.String011,
                    Message = Resources.String010,
                    DoNotShowAgainText = Resources.String013,
                    DoNotShowAgain = Settings.Default.CheckOnStartUp
                };

            decisionControl.DecisionMade += (s, e) => callBack(decisionControl.IsYes, decisionControl.DoNotShowAgain);

            CommandLibrary.ShowDialogCommand.Execute(decisionControl, this.rootElement);
        }

        /// <summary>
        /// Sends the application message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void SendApplicationMessage(string message)
        {
            CommandLibrary.ApplicationMessageCommand.Execute(message, this.rootElement);
        }

        /// <summary>
        /// Sends the applciation error.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void SendApplciationError(Exception exception)
        {
            CommandLibrary.ApplicationExceptionCommand.Execute(exception, this.rootElement);
        }

        /// <summary>
        /// Sends the system shell command.
        /// </summary>
        /// <param name="shellCommand">The shell command.</param>
        public void SendSystemShellCommand(string shellCommand)
        {
            CommandLibrary.SystemShellCommand.Execute(shellCommand, this.rootElement);
        }

        /// <summary>
        /// Suggests a command requery.
        /// </summary>
        public void SuggestCommandRequery()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}