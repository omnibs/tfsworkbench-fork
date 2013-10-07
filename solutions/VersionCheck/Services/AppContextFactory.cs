// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppContextFactory.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the LocalFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Service
{
    using System;
    using System.Windows;

    using TfsWorkbench.UIElements;
    using TfsWorkbench.VersionCheck.Iterfaces;
    using TfsWorkbench.VersionCheck.Properties;

    /// <summary>
    /// The application context factory.
    /// </summary>
    internal static class AppContextFactory
    {
        /// <summary>
        /// Creates the default app context.
        /// </summary>
        /// <returns>A new instance of the default application context class.</returns>
        public static IAppContext CreateDefaultAppContext()
        {
            return new DefaultAppContext(Application.Current.MainWindow);
        }

        /// <summary>
        /// The default dispatcher proxy class.
        /// </summary>
        private class DefaultAppContext : IAppContext
        {
            /// <summary>
            /// The command root element.
            /// </summary>
            private readonly UIElement commandRootElement;

            /// <summary>
            /// Initializes a new instance of the <see cref="AppContextFactory.DefaultAppContext"/> class.
            /// </summary>
            /// <param name="commandRootElement">The command root element.</param>
            public DefaultAppContext(UIElement commandRootElement)
            {
                if (commandRootElement == null)
                {
                    throw new ArgumentNullException("commandRootElement");
                }

                this.commandRootElement = commandRootElement;
            }

            /// <summary>
            /// Gets the user decision on wether to open the download page.
            /// </summary>
            /// <returns><c>True</c> if users wants to open download page; otherwise <c>false</c>.</returns>
            public bool UserWantsToOpenDownloadPage()
            {
                var messageBoxResult = MessageBox.Show(
                    Resources.String010,
                    Resources.String011,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                return messageBoxResult == MessageBoxResult.Yes;
            }

            /// <summary>
            /// Sends the application message.
            /// </summary>
            /// <param name="message">The message.</param>
            public void SendApplicationMessage(string message)
            {
                CommandLibrary.ApplicationMessageCommand.Execute(message, this.commandRootElement);
            }

            /// <summary>
            /// Sends the applciation error.
            /// </summary>
            /// <param name="exception">The exception.</param>
            public void SendApplciationError(Exception exception)
            {
                CommandLibrary.ApplicationExceptionCommand.Execute(exception, this.commandRootElement);
            }

            /// <summary>
            /// Sends the system shell command.
            /// </summary>
            /// <param name="shellCommand">The shell command.</param>
            public void SendSystemShellCommand(string shellCommand)
            {
                CommandLibrary.SystemShellCommand.Execute(shellCommand, this.commandRootElement);
            }
        }
    }
}
