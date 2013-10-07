// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStatisticsLine.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IStatisticLine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.StatisticsViewer.StatisticsGroups
{
    using System.Collections.Generic;

    /// <summary>
    /// The statistic line interface.
    /// </summary>
    public interface IStatisticsLine
    {
        /// <summary>
        /// Gets the row header.
        /// </summary>
        /// <value>The row header.</value>
        string RowHeader { get; }

        /// <summary>
        /// Gets the name of the template.
        /// </summary>
        /// <value>The name of the template.</value>
        string TemplateName { get; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        ICollection<string> Values { get; }
    }
}