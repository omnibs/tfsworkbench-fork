// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalCommandLibrary.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the LocalCommandLibrary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ReportViewer
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
        private static readonly RoutedUICommand showReportCommand = new RoutedUICommand("Show Report", "showReport", typeof(LocalCommandLibrary));

        /// <summary>
        /// The show report viewer command.
        /// </summary>
        private static readonly RoutedUICommand showReportViewerCommand = new RoutedUICommand("Show Report Viewer", "showReportViewer", typeof(LocalCommandLibrary));

        /// <summary>
        /// Gets the show report command.
        /// </summary>
        /// <value>The show report command.</value>
        public static RoutedUICommand ShowReportCommand
        {
            get { return showReportCommand; }
        }

        /// <summary>
        /// Gets the show report viewer command.
        /// </summary>
        /// <value>The show report viewer command.</value>
        public static RoutedUICommand ShowReportViewerCommand
        {
            get { return showReportViewerCommand; }
        }
    }
}
