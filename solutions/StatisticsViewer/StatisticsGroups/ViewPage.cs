// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewPage.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the WorkspacePage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.StatisticsViewer.StatisticsGroups
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.StatisticsViewer.Properties;

    /// <summary>
    /// The workspace statistics page.
    /// </summary>
    internal class ViewPage : IStatisticsPage
    {
        /// <summary>
        /// The statistics groups collection.
        /// </summary>
        private readonly Collection<IStatisticsGroup> groups = new Collection<IStatisticsGroup>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewPage"/> class.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        public ViewPage(IProjectData projectData)
        {
            this.PageTitle = Resources.String012;
            this.PageDescription = Resources.String013;

            this.CalculateStatistics(projectData);
        }

        /// <summary>
        /// Gets the page title.
        /// </summary>
        /// <value>The page title.</value>
        public string PageTitle { get; private set; }

        /// <summary>
        /// Gets the page description.
        /// </summary>
        /// <value>The page description.</value>
        public string PageDescription { get; private set; }

        /// <summary>
        /// Gets the statistics groups.
        /// </summary>
        /// <value>The statistics groups.</value>
        public IEnumerable<IStatisticsGroup> Groups
        {
            get
            {
                return this.groups;
            }
        }

        /// <summary>
        /// Calculates the statistics.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        private void CalculateStatistics(IProjectData projectData)
        {
            this.groups.Clear();

            foreach (var viewMap in projectData.ViewMaps.OrderBy(vm => vm.DisplayOrder))
            {
                this.groups.Add(new ViewGroup(projectData, viewMap));
            }
        }
    }
}
