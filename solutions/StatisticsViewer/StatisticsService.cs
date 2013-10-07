// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatisticsService.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the StatisticsService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.StatisticsViewer
{
    using System.Collections.Generic;

    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.StatisticsViewer.StatisticsGroups;

    /// <summary>
    /// The statistics service class.
    /// </summary>
    internal class StatisticsService : ProjectDataServiceConsumer
    {
        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid
        {
            get
            {
                return this.ProjectDataService.CurrentProjectData != null;
            }
        }

        /// <summary>
        /// Gets the statistics.
        /// </summary>
        /// <returns>The current workspace statistics.</returns>
        public IEnumerable<IStatisticsPage> GetStatistics()
        {
            var statistics = new List<IStatisticsPage>();

            if (this.IsValid)
            {
                statistics.Add(new WorkspacePage(this.ProjectDataService.CurrentProjectData));
                statistics.Add(new StatusPage(this.ProjectDataService.CurrentProjectData));
                statistics.Add(new ViewPage(this.ProjectDataService.CurrentProjectData));
            }

            return statistics;
        }
    }
}