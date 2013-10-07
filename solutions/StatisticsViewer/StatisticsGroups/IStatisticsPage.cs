// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStatisticsPage.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IStatisticsPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.StatisticsViewer.StatisticsGroups
{
    using System.Collections.Generic;

    /// <summary>
    /// The statistics page interface.
    /// </summary>
    public interface IStatisticsPage
    {
        /// <summary>
        /// Gets the page title.
        /// </summary>
        /// <value>The page title.</value>
        string PageTitle { get;  }

        /// <summary>
        /// Gets the page description.
        /// </summary>
        /// <value>The page description.</value>
        string PageDescription { get; }

        /// <summary>
        /// Gets the statistics groups.
        /// </summary>
        /// <value>The statistics groups.</value>
        IEnumerable<IStatisticsGroup> Groups { get; }
    }
}
