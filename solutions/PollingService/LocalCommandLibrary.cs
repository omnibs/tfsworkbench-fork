// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalCommandLibrary.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the LocalCommandLibrary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.PollingService
{
    using System.Windows.Input;

    /// <summary>
    /// The local command library class.
    /// </summary>
    internal class LocalCommandLibrary
    {
        /// <summary>
        /// The show report viewer command.
        /// </summary>
        private static readonly RoutedUICommand showPollerDialogCommand = 
            new RoutedUICommand("Show Poller Dialog", "showPollerDialog", typeof(LocalCommandLibrary));

        /// <summary>
        /// Gets the show report viewer command.
        /// </summary>
        /// <value>The show report viewer command.</value>
        public static RoutedUICommand ShowPollerDialogCommand
        {
            get { return showPollerDialogCommand; }
        }
    }
}
