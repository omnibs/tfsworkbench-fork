// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatisticsGroupBase.cs" company="None">
//   None
// </copyright>
// <summary>
//   The statistics group base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.StatisticsViewer.StatisticsGroups
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;

    using TfsWorkbench.Core.DataObjects;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.StatisticsViewer.Properties;

    /// <summary>
    /// The statistics group base class.
    /// </summary>
    internal abstract class StatisticsGroupBase : IStatisticsGroup
    {
        /// <summary>
        /// The statistic lines.
        /// </summary>
        private readonly Collection<IStatisticsLine> lines = new Collection<IStatisticsLine>();

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsGroupBase"/> class.
        /// </summary>
        /// <param name="columnHeaders">The column headers.</param>
        protected StatisticsGroupBase(IEnumerable<string> columnHeaders)
        {
            this.ColumnHeaders = new Collection<string>(columnHeaders.ToList());
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>The header.</value>
        public abstract string Header { get; }

        /// <summary>
        /// Gets the name of the header template.
        /// </summary>
        /// <value>The name of the header template.</value>
        public abstract string HeaderTemplateName { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public abstract string Description { get; }

        /// <summary>
        /// Gets the lines.
        /// </summary>
        /// <value>The lines.</value>
        public IEnumerable<IStatisticsLine> Lines
        {
            get
            {
                return this.lines;
            }
        }

        /// <summary>
        /// Gets the column headers.
        /// </summary>
        /// <value>The column headers.</value>
        public ICollection<string> ColumnHeaders { get; private set; }

        /// <summary>
        /// Gets the metric sum.
        /// </summary>
        /// <param name="itemType">Type of the item.</param>
        /// <param name="instances">The instances.</param>
        /// <returns>The metric sum.</returns>
        protected static string GetMetricSum(ItemTypeData itemType, IEnumerable<IWorkbenchItem> instances)
        {
            var output = Resources.String004;
            var hasMatric = !string.IsNullOrEmpty(itemType.NumericField);

            if (hasMatric)
            {
                var isMetricDouble = itemType.Fields.Any(fd =>
                        string.Equals(fd.ReferenceName, itemType.NumericField, StringComparison.OrdinalIgnoreCase)
                        && fd.IsDouble);

                var isMetricInteger = itemType.Fields.Any(fd =>
                        string.Equals(fd.ReferenceName, itemType.NumericField, StringComparison.OrdinalIgnoreCase)
                        && fd.IsInteger);

                if (isMetricDouble)
                {
                    output = instances.Sum(w => GetMetric<double>(w, itemType.NumericField)).ToString();
                }

                if (isMetricInteger)
                {
                    output = instances.Sum(w => GetMetric<int>(w, itemType.NumericField)).ToString();
                }
            }

            return output;
        }

        /// <summary>
        /// Concats the filtered and all values.
        /// </summary>
        /// <param name="filtered">The filtered.</param>
        /// <param name="all">All items.</param>
        /// <returns>A concatentated string of the values.</returns>
        protected static string ConcatFilteredAndAllValues(object filtered, object all)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} / ({1})", filtered, all);
        }

        /// <summary>
        /// Adds the line.
        /// </summary>
        /// <param name="line">The statistic line.</param>
        protected void AddLine(IStatisticsLine line)
        {
            this.lines.Add(line);
        }

        /// <summary>
        /// Gets the metric.
        /// </summary>
        /// <typeparam name="T">The metric type.</typeparam>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="metricField">The metric field.</param>
        /// <returns>The metric value.</returns>
        private static T GetMetric<T>(IWorkbenchItem workbenchItem, string metricField)
        {
            var metricValue = workbenchItem[metricField];

            return metricValue == null || !typeof(T).IsAssignableFrom(metricValue.GetType())
                ? default(T)
                : (T)metricValue;
        }
    }
}