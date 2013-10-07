// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusGroup.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the WorkspaceStatistics type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.StatisticsViewer.StatisticsGroups
{
    using System;
    using System.Linq;

    using TfsWorkbench.Core.DataObjects;
    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.StatisticsViewer.Properties;

    /// <summary>
    /// The workspace statistics item.
    /// </summary>
    internal class StatusGroup : StatisticsGroupBase
    {
        /// <summary>
        /// The type header.
        /// </summary>
        private readonly string header;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusGroup"/> class.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="itemTypeData">The item type data.</param>
        public StatusGroup(IProjectData projectData, ItemTypeData itemTypeData)
            : base(new[] { Resources.String002, GetMetricTitle(itemTypeData) })
        {
            if (projectData == null)
            {
                throw new ArgumentNullException("projectData");
            }

            if (itemTypeData == null)
            {
                throw new ArgumentNullException("itemTypeData");
            }

            this.header = itemTypeData.TypeName;
            this.BuildStatisticLines(projectData, itemTypeData);
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>The header.</value>
        public override string Header
        {
            get
            {
                return this.header;
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
        /// Builds the header.
        /// </summary>
        /// <param name="itemTypeData">The item type data.</param>
        /// <returns>The group header text.</returns>
        private static string GetMetricTitle(ItemTypeData itemTypeData)
        {
            var metricField = itemTypeData.Fields.FirstOrDefault(f => Equals(f.ReferenceName, itemTypeData.NumericField));

            var metricText = metricField != null
                                 ? metricField.DisplayName
                                 : Resources.String011;

            return metricText;
        }

        /// <summary>
        /// Builds the statistic lines.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="itemTypeData">The item type data.</param>
        private void BuildStatisticLines(IProjectData projectData, ItemTypeData itemTypeData)
        {
            var allInstances = projectData.WorkbenchItems.UnfilteredList.Where(w => Equals(w.GetTypeName(), itemTypeData.TypeName)).ToArray();
            var filteredInstances = projectData.WorkbenchItems.Where(w => Equals(w.GetTypeName(), itemTypeData.TypeName)).ToArray();
            var allTotalMetricSum = 0d;
            var filteredTotalMetricSum = 0d;

            foreach (var state in itemTypeData.States.OrderBy(s => s))
            {
                var localState = state;
                var filteredStateInstances = filteredInstances.Where(w => Equals(localState, w.GetState()));
                var filteredCount = filteredStateInstances.Count().ToString();
                var filteredMetricSum = GetMetricSum(itemTypeData, filteredStateInstances);

                var allStateInstances = allInstances.Where(w => Equals(localState, w.GetState()));
                var allCount = allStateInstances.Count().ToString();
                var allMetricSum = GetMetricSum(itemTypeData, allStateInstances);

                double metricDouble;
                if (double.TryParse(filteredMetricSum, out metricDouble))
                {
                    filteredTotalMetricSum += metricDouble;
                }

                if (double.TryParse(allMetricSum, out metricDouble))
                {
                    allTotalMetricSum += metricDouble;
                }

                this.AddLine(
                    new DetailLine(
                            localState, 
                            new[] { ConcatFilteredAndAllValues(filteredCount, allCount), ConcatFilteredAndAllValues(filteredMetricSum, allMetricSum) },
                            TemplateNames.ThreeColumnLine));
            }

            this.AddLine(
                new DetailLine(
                        string.Empty, 
                        new[] { ConcatFilteredAndAllValues(filteredInstances.Count(), allInstances.Count()), ConcatFilteredAndAllValues(filteredTotalMetricSum, allTotalMetricSum) }, 
                        TemplateNames.ThreeColumnLineBold));
        }
    }
}