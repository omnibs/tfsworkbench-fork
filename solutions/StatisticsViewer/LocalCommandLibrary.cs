// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalCommandLibrary.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the LocalCommandLibrary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.StatisticsViewer
{
    using System.Windows.Input;

    /// <summary>
    /// The local command library class.
    /// </summary>
    internal class LocalCommandLibrary
    {
        /// <summary>
        /// The sgow report command.
        /// </summary>
        private static readonly RoutedUICommand showStatisticsViewerCommand = new RoutedUICommand("Show Statistics", "showStatistics", typeof(LocalCommandLibrary));

        /// <summary>
        /// Gets the show statistics viewer command.
        /// </summary>
        /// <value>The show statistics viewer command.</value>
        public static RoutedUICommand ShowStatisticsViewerCommand
        {
            get { return showStatisticsViewerCommand; }
        }
    }
}
