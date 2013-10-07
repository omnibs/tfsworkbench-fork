// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperatorToStringConverter.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the OperatorToStringConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.FilterService.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    using TfsWorkbench.FilterService.Properties;

    /// <summary>
    /// The filter operator to string value converter.
    /// </summary>
    public class OperatorToStringConverter : IValueConverter
    {
        /// <summary>
        /// The option to string map.
        /// </summary>
        private static readonly Dictionary<FilterOperatorOption, string> optionMap;

        /// <summary>
        /// Initializes static members of the <see cref="OperatorToStringConverter"/> class.
        /// </summary>
        static OperatorToStringConverter()
        {
            optionMap = new Dictionary<FilterOperatorOption, string>
                {
                    { FilterOperatorOption.IsEqualTo, Resources.String003 },
                    { FilterOperatorOption.IsNotEqualTo, Resources.String004 },
                    { FilterOperatorOption.IsGreaterThan, Resources.String005 },
                    { FilterOperatorOption.IsLessThan, Resources.String006 },
                    { FilterOperatorOption.IsGreaterThanEqualTo, Resources.String007 },
                    { FilterOperatorOption.IsLessThanEqualTo, Resources.String008 },
                    { FilterOperatorOption.StartsWith, Resources.String009 },
                    { FilterOperatorOption.EndsWith, Resources.String010 },
                    { FilterOperatorOption.Contains, Resources.String011 },
                    { FilterOperatorOption.DoesNotStartWith, Resources.String043 },
                    { FilterOperatorOption.DoesNotEndWith, Resources.String044 },
                    { FilterOperatorOption.DoesNotContain, Resources.String045 },
                };
        }

        /// <summary>
        /// Gets the operators as strings.
        /// </summary>
        /// <value>The operators as strings.</value>
        public static IEnumerable<string> OperatorsAsStrings
        {
            get
            {
                return optionMap.Values;
            }
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var output = string.Empty;

            if ((value is FilterOperatorOption)
                && !optionMap.TryGetValue((FilterOperatorOption)value, out output))
            {
                output = string.Empty;
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
            return GetKey(value as string);
        }

        /// <summary>
        /// Tries to get the key.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The correspondiong operation option.</returns>
        private static FilterOperatorOption GetKey(string value)
        {
            return 
                optionMap.Any(kvp => Equals(kvp.Value, value)) 
                ? optionMap.First(kvp => Equals(kvp.Value, value)).Key 
                : FilterOperatorOption.IsEqualTo;
        }
    }
}
