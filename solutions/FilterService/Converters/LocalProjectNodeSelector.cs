// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalProjectNodeSelector.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ProjectNodeSelector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.FilterService.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.UIElements.ValueConverters;

    /// <summary>
    /// Initializes instance of ProjectNodeSelector
    /// </summary>
    public class LocalProjectNodeSelector : ProjectNodeSelector
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fieldName = value as string;

            if (string.IsNullOrEmpty(fieldName))
            {
                return null;
            }

            var field = this.ProjectData.ItemTypes.SelectMany(it => it.Fields).FirstOrDefault(
                f => f.DisplayName == fieldName);

            IProjectNode path;
            if (field == null || !this.ProjectData.ProjectNodes.TryGetValue(field.ReferenceName, out path))
            {
                return null;
            }

            return path;
        }
    }
}