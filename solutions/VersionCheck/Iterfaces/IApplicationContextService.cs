// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApplicationContextService.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the IDispatcherProxy type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Iterfaces
{
    using System;

    /// <summary>
    /// The application context interface.
    /// </summary>
    public interface IApplicationContextService
    {
        /// <summary>
        /// Gets the user decision on wether to open the download page.
        /// </summary>
        /// <param name="callBack">The call back.</param>
        void DoesUserWantToOpenDownloadPage(Action<bool, bool> callBack);

        /// <summary>
        /// Sends the application message.
        /// </summary>
        /// <param name="message">The message.</param>
        void SendApplicationMessage(string message);

        /// <summary>
        /// Sends the applciation error.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void SendApplciationError(Exception exception);

        /// <summary>
        /// Sends the system shell command.
        /// </summary>
        /// <param name="shellCommand">The shell command.</param>
        void SendSystemShellCommand(string shellCommand);

        /// <summary>
        /// Suggests a command requery.
        /// </summary>
        void SuggestCommandRequery();
    }
}
