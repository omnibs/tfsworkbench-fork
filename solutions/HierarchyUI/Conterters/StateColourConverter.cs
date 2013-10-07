// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateColourConverter.cs" company="None">
//   None
// </copyright>
// <summary>
//   The State colour converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.HierarchyUI.Conterters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Media;

    using Core.DataObjects;

    /// <summary>
    /// The State colour converter class.
    /// </summary>
    public class StateColourConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"/>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding"/>.<see cref="F:System.Windows.Data.Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> or the default value.
        /// </returns>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding"/> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count() != 2)
            {
                return Brushes.Transparent;
            }

            var state = values[0] as string;
            var viewMap = values[1] as ViewMap;

            if (string.IsNullOrEmpty(state) || viewMap == null)
            {
                return Brushes.Transparent;
            }

            var stateColour = viewMap.StateItemColours.FirstOrDefault(c => Equals(c.Value, state));

            return stateColour == null ? Brushes.Transparent : new SolidColorBrush(stateColour.Colour);
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        /// <param name="value">The value that the binding target produces.</param><param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
