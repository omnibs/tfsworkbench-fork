// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BoolToStringConverter.cs" company="None">
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
    using System.Windows.Data;

    /// <summary>
    /// Initializes instance of BoolToVisibilityConverter
    /// </summary>
    public class BoolToStringConverter : IValueConverter
    {
        /// <summary>
        /// The default false text.
        /// </summary>
        private const string DefaultFalseText = "No";

        /// <summary>
        /// The default truee text.
        /// </summary>
        private const string DefaultTrueText = "Yes";

        /// <summary>
        /// Initializes a new instance of the <see cref="BoolToStringConverter"/> class. 
        /// </summary>
        public BoolToStringConverter()
        {
            this.TrueText = DefaultTrueText;
            this.FalseText = DefaultFalseText;
        }

        /// <summary>
        /// Gets or sets the true visibility.
        /// </summary>
        /// <value>The true visibility.</value>
        public string TrueText
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the false visibility.
        /// </summary>
        /// <value>The false visibility.</value>
        public string FalseText
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
                return DefaultFalseText;
            }

            var valueAsBool = (bool)value;

            return valueAsBool ? this.TrueText : this.FalseText;
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
            var stringValue = value as string;

            if (string.IsNullOrEmpty(stringValue))
            {
                return false;
            }

            return string.Compare(stringValue, this.TrueText, true);
        }
    }
}