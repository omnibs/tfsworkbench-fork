// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspacePage.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the WorkspacePage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.StatisticsViewer.StatisticsGroups
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.StatisticsViewer.Properties;

    /// <summary>
    /// The workspace statistics page.
    /// </summary>
    internal class WorkspacePage : IStatisticsPage
    {
        /// <summary>
        /// The statistics groups collection.
        /// </summary>
        private readonly Collection<IStatisticsGroup> groups = new Collection<IStatisticsGroup>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspacePage"/> class.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        public WorkspacePage(IProjectData projectData)
        {
            if (projectData == null)
            {
                throw new ArgumentNullException("projectData");
            }

            this.PageTitle = Resources.String001;
            this.PageDescription = Resources.String005;

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

            this.groups.Add(new WorkspaceGroup(projectData));
        }
    }
}
