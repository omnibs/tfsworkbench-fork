// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStatisticsController.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IStatisticsController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.StatisticsViewer
{
    using System.Collections.Generic;
    using System.Windows.Input;

    using TfsWorkbench.StatisticsViewer.StatisticsGroups;

    /// <summary>
    /// The statistics controller interface.
    /// </summary>
    public interface IStatisticsController
    {
        /// <summary>
        /// Gets the statistic pages.
        /// </summary>
        /// <value>The statistic pages.</value>
        IEnumerable<IStatisticsPage> StatisticPages { get; }

        /// <summary>
        /// Called when [show statistics].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        void OnShowStatistics(object sender, ExecutedRoutedEventArgs e);

        /// <summary>
        /// Called when [can execute].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        void OnCanExecute(object sender, CanExecuteRoutedEventArgs e);
    }
}