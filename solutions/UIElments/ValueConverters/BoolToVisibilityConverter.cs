// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BoolToVisibilityConverter.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the BoolToVisibilityConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Initializes instance of BoolToVisibilityConverter
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolToVisibilityConverter"/> class.
        /// </summary>
        public BoolToVisibilityConverter()
        {
            this.TrueVisibility = Visibility.Visible;
            this.FalseVisibility = Visibility.Hidden;
        }

        /// <summary>
        /// Gets or sets the true visibility.
        /// </summary>
        /// <value>The true visibility.</value>
        public Visibility TrueVisibility
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the false visibility.
        /// </summary>
        /// <value>The false visibility.</value>
        public Visibility FalseVisibility
        {
            get; set;
        }

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
            if (!(value is bool))
            {
                return Visibility.Hidden;
            }

            var valueAsBool = (bool)value;

            return valueAsBool ? this.TrueVisibility : this.FalseVisibility;
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
            if (!(value is Visibility))
            {
                return false;
            }

            var visibility = (Visibility)value;

            return visibility == this.TrueVisibility;
        }
    }
}