// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayFieldConverterBase.cs" company="None">
//   None
// </copyright>
// <summary>
//   The display field converter base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace TfsWorkbench.UIElements.ValueConverters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    using Core.DataObjects;
    using Core.Helpers;
    using Core.Interfaces;

    using TfsWorkbench.Core.Services;

    /// <summary>
    /// The display field converter base.
    /// </summary>
    public abstract class DisplayFieldConverterBase : IMultiValueConverter
    {
        /// <summary>
        /// Gets the value getter.
        /// </summary>
        /// <value>The value getter.</value>
        protected abstract Func<ItemTypeData, string> GetDisplayFieldName { get; }

        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"/>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding"/>.<see cref="F:System.Windows.Data.Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> or the default value.
        /// </returns>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding"/> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (this.GetDisplayFieldName == null)
            {
                throw new NullReferenceException("GetDisplayFieldName");
            }

            IWorkbenchItem workbechItem;
            ItemTypeData itemTypeData;

            string output = null;

            var regEx = new Regex(@"<[^>]*>");

            if (TryGetTypeData(values, out workbechItem, out itemTypeData))
            {
                var displayFieldName = this.GetDisplayFieldName(itemTypeData);

                var value = workbechItem[displayFieldName];

                if (value != null)
                {
                    output = regEx.Replace(value.ToString(), string.Empty);                    
                }
            }

            return output;
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

        /// <summary>
        /// Tries to get item type data.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="itemTypeData">The item type data.</param>
        /// <returns>
        /// <c>True</c> if the item type data is found; otherwise <c>false</c>.
        /// </returns>
        private static bool TryGetTypeData(IList<object> values, out IWorkbenchItem workbenchItem, out ItemTypeData itemTypeData)
        {
            var projectData = ServiceManager.Instance.GetService<IProjectDataService>().CurrentProjectData;
            
            workbenchItem = values != null && values.Count == 2 ? values[1] as IWorkbenchItem : null;

            var typeName = workbenchItem == null || workbenchItem.ValueProvider == null ? null : workbenchItem.GetTypeName();

            itemTypeData = projectData == null || workbenchItem == null 
                               ? null 
                               : projectData.ItemTypes.FirstOrDefault(t => Equals(t.TypeName, typeName));

            return workbenchItem != null && itemTypeData != null;
        }
    }
}