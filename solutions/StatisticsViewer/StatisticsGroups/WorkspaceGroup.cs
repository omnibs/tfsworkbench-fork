// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceGroup.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the WorkspaceStatistics type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.StatisticsViewer.StatisticsGroups
{
    using System.Linq;

    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.StatisticsViewer.Properties;

    /// <summary>
    /// The workspace statistics item.
    /// </summary>
    internal class WorkspaceGroup : StatisticsGroupBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceGroup"/> class.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        public WorkspaceGroup(IProjectData projectData)
            : base(new[] { Resources.String002, Resources.String003 })
        {
            this.BuildStatisticLines(projectData);
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>The header.</value>
        public override string Header
        {
            get
            {
                return Resources.String008;
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the name of the header template.
        /// </summary>
        /// <value>The name of the header template.</value>
        public override string HeaderTemplateName
        {
            get
            {
                return TemplateNames.ThreeColumnHeader;
            }
        }

        /// <summary>
        /// Builds the statistic lines.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        private void BuildStatisticLines(IProjectData projectData)
        {
            if (projectData == null)
            {
                return;
            }

            var totalFilteredMetricSum = 0d;
            var totalAllMetricSum = 0d;

            var filterItems = projectData.WorkbenchItems.ToArray();
            var unfilteredItems = projectData.WorkbenchItems.UnfilteredList.ToArray();

            foreach (var itemType in projectData.ItemTypes.OrderBy(it => it.TypeName))
            {
                var typeName = itemType.TypeName;
                var filteredInstances = filterItems.Where(w => Equals(w.GetTypeName(), typeName)).ToArray();
                var allInstances = unfilteredItems.Where(w => Equals(w.GetTypeName(), typeName)).ToArray();

                var filteredCount = filteredInstances.Count().ToString();
                var filteredMetricSum = GetMetricSum(itemType, filteredInstances);

                double metricDouble;
                if (double.TryParse(filteredMetricSum, out metricDouble))
                {
                    totalFilteredMetricSum += metricDouble;
                }

                var allCount = allInstances.Count().ToString();
                var allMetricSum = GetMetricSum(itemType, allInstances);

                if (double.TryParse(allMetricSum, out metricDouble))
                {
                    totalAllMetricSum += metricDouble;
                }

                this.AddLine(
                    new DetailLine(
                        typeName, 
                        new[] { ConcatFilteredAndAllValues(filteredCount, allCount), ConcatFilteredAndAllValues(filteredMetricSum, allMetricSum) }, 
                        TemplateNames.ThreeColumnLine));
            }

            var lineStrings = new[]
                {
                    ConcatFilteredAndAllValues(filterItems.Count(), unfilteredItems.Count()),
                    ConcatFilteredAndAllValues(totalFilteredMetricSum, totalAllMetricSum)
                };

            this.AddLine(
                new DetailLine(
                    string.Empty,
                    lineStrings,
                    TemplateNames.ThreeColumnLineBold));
        }
    }
}