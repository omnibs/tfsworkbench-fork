// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ValidationHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Core.Interfaces;

    using DataObjects;

    /// <summary>
    /// Initializes instance of ValidationHelper
    /// </summary>
    internal static class ValidationHelper
    {
        /// <summary>
        /// Determines whether [the specified name] [is valid].
        /// </summary>
        /// <param name="name">The name to validate.</param>
        /// <returns>
        /// <c>true</c> if [the specified name] [is valid]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidName(string name)
        {
            var regEx = new Regex(@"^\w+[\w ]*\w+$");

            return regEx.IsMatch(name);
        }

        /// <summary>
        /// Determines whether [the specified date range] [is valid].
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>
        /// <c>true</c> if [the specified date range] [is valid]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidDateRange(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                return false;
            }

            var duration = endDate.Value.Subtract(startDate.Value);

            return duration.Days > 0;
        }

        /// <summary>
        /// Determines whether [the specified work stream] [is valid].
        /// </summary>
        /// <param name="workStream">The work stream.</param>
        /// <returns>
        /// <c>true</c> if [the specified work stream] [is valid]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidWorkStream(WorkStream workStream)
        {
            return workStream != null && IsValidName(workStream.Name) && workStream.Cadance > 0;
        }

        /// <summary>
        /// Determines whether [the specified named items] [has unique names].
        /// </summary>
        /// <param name="namedItems">The named items.</param>
        /// <returns>
        /// <c>True</c> if [the specified named items] [has unique names]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasUniqueNames(IEnumerable<INamedItem> namedItems)
        {
            var totalCount = namedItems.Count();
            var distinctCount = namedItems.Select(n => n.Name).Distinct().Count();

            return totalCount.Equals(distinctCount);
        }

        /// <summary>
        /// Determines whether [the specified project nodes] [have unique names].
        /// </summary>
        /// <param name="projectNodes">The project nodes.</param>
        /// <returns>
        /// <c>true</c> if [the specified project nodes] [have unique names]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasUniqueNames(IEnumerable<IProjectNode> projectNodes)
        {
            var totalCount = projectNodes.Count();
            var distinctCount = projectNodes.Select(n => n.Name).Distinct().Count();

            return totalCount.Equals(distinctCount);
        }
    }
}
