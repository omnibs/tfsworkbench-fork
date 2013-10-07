// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSpanToValueConverter.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the TimeSpanToValueConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// The time span to value converter.
    /// </summary>
    public class TimeSpanToValueConverter : IValueConverter
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
            var timeSpan = value as TimeSpan?;
            return timeSpan.HasValue ? timeSpan.Value.TotalSeconds : 0;
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
            var seconds = value as double?;
            return new TimeSpan(0, 0, seconds.HasValue ? (int)Math.Ceiling(seconds.Value) : 0);
        }
    }
}
