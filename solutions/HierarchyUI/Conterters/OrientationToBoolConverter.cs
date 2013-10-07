// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrientationToBoolConverter.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the OrientationToBoolConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.HierarchyUI.Conterters
{
    using System;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// The orientation to bool converter class.
    /// </summary>
    public class OrientationToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the true orientation.
        /// </summary>
        /// <value>The true orientation.</value>
        public Orientation TrueOrientation { get; set; }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var orientation = value as Orientation?;

            return !orientation.HasValue || orientation.Value.Equals(this.TrueOrientation);
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
            var boolValue = value as bool?;

            return !boolValue.HasValue || boolValue.Value
                       ? this.TrueOrientation
                       : this.TrueOrientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
        }
    }
}
