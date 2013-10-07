// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStatisticsGroup.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IStatisticGroup type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.StatisticsViewer.StatisticsGroups
{
    using System.Collections.Generic;

    /// <summary>
    /// The statistics group interface.
    /// </summary>
    public interface IStatisticsGroup
    {
        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>The header.</value>
        string Header { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        string Description { get; }

        /// <summary>
        /// Gets the lines.
        /// </summary>
        /// <value>The lines.</value>
        IEnumerable<IStatisticsLine> Lines { get; }

        /// <summary>
        /// Gets the column headers.
        /// </summary>
        /// <value>The column headers.</value>
        ICollection<string> ColumnHeaders { get; }

        /// <summary>
        /// Gets the name of the header template.
        /// </summary>
        /// <value>The name of the header template.</value>
        string HeaderTemplateName { get; }
    }
}