// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewGroup.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the WorkspaceStatistics type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.StatisticsViewer.StatisticsGroups
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TfsWorkbench.Core.DataObjects;
    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.StatisticsViewer.Properties;

    /// <summary>
    /// The workspace statistics item.
    /// </summary>
    internal class ViewGroup : StatisticsGroupBase
    {
        /// <summary>
        /// The type header.
        /// </summary>
        private readonly string header;

        /// <summary>
        /// The description.
        /// </summary>
        private readonly string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewGroup"/> class.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="viewMap">The view map data.</param>
        public ViewGroup(IProjectData projectData, ViewMap viewMap)
            : base(new[] { Resources.String002, Resources.String003 })
        {
            if (projectData == null)
            {
                throw new ArgumentNullException("projectData");
            }

            if (viewMap == null)
            {
                throw new ArgumentNullException("viewMap");
            }

            this.header = viewMap.Title;
            this.description = viewMap.Description;
            this.BuildStatisticLines(projectData, viewMap);
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
                return this.description;
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
        /// <param name="viewMap">The view map data.</param>
        private void BuildStatisticLines(IProjectData projectData, ViewMap viewMap)
        {
            var filteredItems = projectData.WorkbenchItems.ToArray();
            var allItems = projectData.WorkbenchItems.UnfilteredList.ToArray();

            Func<string, bool, IEnumerable<IWorkbenchItem>> getInstances = 
                (typeName, isFiltered) =>
                    (isFiltered ? filteredItems : allItems).Where(w => Equals(w.GetTypeName(), typeName));

            Func<string, bool, IEnumerable<IWorkbenchItem>> getChildrenWithParents =
                (typeName, isFiltered) =>
                    (isFiltered ? filteredItems : allItems)
                        .Where(w =>
                            Equals(w.GetTypeName(), typeName)
                            && w.ParentLinks.Any(pl => Equals(pl.LinkName, viewMap.LinkName))
                            && viewMap.SwimLaneStates.Union(viewMap.BucketStates).Any(s => Equals(s, w.GetState())));

            Func<string, bool, IEnumerable<IWorkbenchItem>> getOrphans =
                (typeName, isFiltered) => (isFiltered ? filteredItems : allItems)
                    .Where(
                        w =>
                            Equals(w.GetTypeName(), typeName)
                            && !w.ParentLinks.Any(pl => Equals(pl.LinkName, viewMap.LinkName)));


            var totalFilteredInstance = 0;
            var totalAllInstances = 0;

            // Add a line for each parent type.
            foreach (var parentType in viewMap.ParentTypes)
            {
                this.AddLine(
                    string.Concat(parentType, Resources.String014),
                    projectData.ItemTypes[parentType],
                    getInstances,
                    ref totalFilteredInstance,
                    ref totalAllInstances);
            }

            // Add a line for the child type.
            this.AddLine(
                string.Concat(viewMap.ChildType, Resources.String016), 
                projectData.ItemTypes[viewMap.ChildType],
                getChildrenWithParents, 
                ref totalFilteredInstance,
                ref totalAllInstances);

            // Add a line for the view orphans.
            this.AddLine(
                string.Concat(viewMap.ChildType, Resources.String017),
                projectData.ItemTypes[viewMap.ChildType],
                getOrphans,
                ref totalFilteredInstance,
                ref totalAllInstances);

            // Add the view total line.
            this.AddLine(
                new DetailLine(
                    string.Empty, 
                    new[] { ConcatFilteredAndAllValues(totalFilteredInstance, totalAllInstances), Resources.String004 }, 
                    TemplateNames.ThreeColumnLineBold));
        }

        /// <summary>
        /// Adds the line.
        /// </summary>
        /// <param name="lineHeader">The line header.</param>
        /// <param name="itemTypeData">The item type data.</param>
        /// <param name="getInstances">The get all instances.</param>
        /// <param name="filteredInstanceCount">The filtered instance count.</param>
        /// <param name="allInstanceCount">All instance count.</param>
        private void AddLine(
            string lineHeader, 
            ItemTypeData itemTypeData,
            Func<string, bool, IEnumerable<IWorkbenchItem>> getInstances,
            ref int filteredInstanceCount,
            ref int allInstanceCount)
        {
            if (itemTypeData == null)
            {
                return;
            }

            var filteredInstances = getInstances(itemTypeData.TypeName, true).ToArray();
            var filteredMetricSum = GetMetricSum(itemTypeData, filteredInstances);
            filteredInstanceCount += filteredInstances.Count();

            var allInstances = getInstances(itemTypeData.TypeName, false).ToArray();
            var allMetricSum = GetMetricSum(itemTypeData, allInstances);
            allInstanceCount += allInstances.Count();

            this.AddLine(
                new DetailLine(
                    lineHeader,
                    new[] { ConcatFilteredAndAllValues(filteredInstances.Count(), allInstances.Count()), ConcatFilteredAndAllValues(filteredMetricSum, allMetricSum) }, 
                    TemplateNames.ThreeColumnLine));
        }
    }
}