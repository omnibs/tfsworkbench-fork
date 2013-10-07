// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumberToStringConverter.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the NumberToStringConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// The number to string converyter class.
    /// </summary>
    public class NumberToStringConverter : IValueConverter
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
            return value == null ? null : value.ToString();
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
            if (value == null)
            {
                return string.Empty;
            }

            double doubleOutput;
            if (double.TryParse(value.ToString(), out doubleOutput))
            {
                return doubleOutput;
            }

            int intOutput;
            if (int.TryParse(value.ToString(), out intOutput))
            {
                return intOutput;
            }

            return value;
        }
    }
}
