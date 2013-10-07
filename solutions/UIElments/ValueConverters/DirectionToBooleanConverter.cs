// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectionToBooleanConverter.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the DirectionToBooleanConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Core.Helpers;

    /// <summary>
    /// Initializes instance of DirectionToBooleanConverter
    /// </summary>
    public class DirectionToBooleanConverter : IValueConverter 
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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is SortDirection))
            {
                return true;
            }

            return (SortDirection)value == SortDirection.Ascending;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueAsBool = value as bool?;

            if (valueAsBool == null)
            {
                return SortDirection.Ascending;
            }

            return valueAsBool.Value ? SortDirection.Ascending : SortDirection.Descending;
        }
    }
}