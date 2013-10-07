// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionToStringConverter.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the OperatorToStringConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.FilterService.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using TfsWorkbench.FilterService.Properties;

    /// <summary>
    /// The filter operator to string value converter.
    /// </summary>
    public class ActionToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var output = Resources.String014;

            if (value is FilterActionOption)
            {
                output = (FilterActionOption)value == FilterActionOption.Include
                    ? Resources.String014
                    : Resources.String015;
            }

            return output;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var actionString = value as string;

            if (string.IsNullOrEmpty(actionString) || Resources.String014 == actionString)
            {
                return FilterActionOption.Include;
            }

            return FilterActionOption.Exclude;
        }
    }
}
