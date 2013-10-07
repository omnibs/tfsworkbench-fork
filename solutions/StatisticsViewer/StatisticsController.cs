// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatisticsController.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the StatisticsController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.StatisticsViewer
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;

    using TfsWorkbench.StatisticsViewer.StatisticsGroups;
    using TfsWorkbench.UIElements;

    /// <summary>
    /// The Statistics controller class.
    /// </summary>
    internal class StatisticsController : IStatisticsController
    {
        /// <summary>
        /// The statistics service.
        /// </summary>
        private readonly StatisticsService service;

        /// <summary>
        /// The controller instance;
        /// </summary>
        private static IStatisticsController instance;

        /// <summary>
        /// Prevents a default instance of the <see cref="StatisticsController"/> class from being created.
        /// </summary>
        private StatisticsController()
        {
            this.service = new StatisticsService();
        }

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static IStatisticsController Instance
        {
            get
            {
                return instance = instance ?? new StatisticsController();
            }

            set
            {
                instance = value;
            }
        }

        /// <summary>
        /// Gets the statistic pages.
        /// </summary>
        /// <value>The statistic pages.</value>
        public IEnumerable<IStatisticsPage> StatisticPages
        {
            get
            {
                return this.service.GetStatistics();
            }
        }

        /// <summary>
        /// Called when [show statistics].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public void OnShowStatistics(object sender, ExecutedRoutedEventArgs e)
        {
            if (!this.service.IsValid)
            {
                return;
            }

            var statisticsViewerControl = new StatisticsViewerControl(this);
            CommandLibrary.ShowDialogCommand.Execute(statisticsViewerControl, sender as IInputElement);
        }

        /// <summary>
        /// Called when [can execute].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        public void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.service.IsValid;
        }
    }
}
