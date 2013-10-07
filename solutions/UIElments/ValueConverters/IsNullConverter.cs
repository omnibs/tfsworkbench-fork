// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsNullConverter.cs" company="None">
//   None
// </copyright>
// <summary>
//   The is null converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// The is null converter.
    /// </summary>
    public class IsNullConverter : IValueConverter
    {
        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>A boolean indication of null status.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        /// <summary>
        /// The convert back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>Nothing, it's not implemented.</returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}