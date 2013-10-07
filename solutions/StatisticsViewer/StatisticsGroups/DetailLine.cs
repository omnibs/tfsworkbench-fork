// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DetailLine.cs" company="None">
//   None
// </copyright>
// <summary>
//   The detail line class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.StatisticsViewer.StatisticsGroups
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// The detail line class.
    /// </summary>
    internal class DetailLine : IStatisticsLine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetailLine"/> class.
        /// </summary>
        /// <param name="rowHeader">The row header.</param>
        /// <param name="templateName">Name of the template.</param>
        public DetailLine(string rowHeader, string templateName)
            : this(rowHeader,  new string[] { }, templateName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailLine"/> class.
        /// </summary>
        /// <param name="rowHeader">The row header.</param>
        /// <param name="values">The values.</param>
        /// <param name="templateName">Name of the template.</param>
        public DetailLine(string rowHeader, IEnumerable<string> values, string templateName)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            this.RowHeader = rowHeader;
            this.Values = new Collection<string>(values.ToList());
            this.TemplateName = templateName;
        }

        /// <summary>
        /// Gets the row header.
        /// </summary>
        /// <value>The row header.</value>
        public string RowHeader { get; private set; }

        /// <summary>
        /// Gets the name of the template.
        /// </summary>
        /// <value>The name of the template.</value>
        public string TemplateName { get; private set; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        public ICollection<string> Values { get; private set; }
    }
}