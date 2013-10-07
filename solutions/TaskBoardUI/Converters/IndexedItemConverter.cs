// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexedItemConverter.cs" company="None">
//   None
// </copyright>
// <summary>
//   Initializes instance of SwimLaneRowColumnConverter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.TaskBoardUI.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;

    using TfsWorkbench.TaskBoardUI.DataObjects;
    using TfsWorkbench.UIElements;

    /// <summary>
    /// Initializes instance of SwimLaneRowColumnConverter
    /// </summary>
    internal class IndexedItemConverter : IValueConverter
    {
        /// <summary>
        /// The reflected "get element at" method.
        /// </summary>
        private readonly MethodInfo getElementAtMethodInfo = typeof(IndexedItemConverter).GetMethod("GetElementAt", BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the getElementAtMethodInfo returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string state;
            if (TryGetState(value, parameter, out state))
            {
                return state;
            }

            // If not state collection, then use reflection to find the required item.
            object output = null;

            try
            {
                var genericType = targetType == typeof(object) && value.GetType().IsGenericType
                    ? value.GetType().GetGenericArguments().ElementAt(0)
                    : targetType;

                var generic = this.getElementAtMethodInfo.MakeGenericMethod(genericType);

                output = generic.Invoke(this, new[] { value, parameter });
            }
            catch (Exception ex)
            {
                if (!CommandLibrary.ApplicationExceptionCommand.CanExecute(ex, Application.Current.MainWindow))
                {
                    throw;
                }

                CommandLibrary.ApplicationExceptionCommand.Execute(ex, Application.Current.MainWindow);
            }

            return output;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the getElementAtMethodInfo returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tries to get the state.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="state">The state collection.</param>
        /// <returns><c>True</c> if the state found; otherwise <c>false</c>.</returns>
        private static bool TryGetState(object value, object parameter, out string state)
        {
            var stateContainerCollection = value as IEnumerable<string>;
            var indexParam = parameter as int?;

            state = stateContainerCollection != null && indexParam.HasValue
                ? GetElementAt(stateContainerCollection, indexParam.Value)
                : null;

            return state != null;
        }

        /// <summary>
        /// Determines whether [this invocation] [has valid parameters].
        /// </summary>
        /// <typeparam name="T">The array type</typeparam>
        /// <param name="enumerable">The source collection.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// <c>Instance</c> at index if [this invocation] [has valid parameters]; otherwise, <c>null</c>.
        /// </returns>
        /// <remarks>Called through reflection.</remarks>
        private static T GetElementAt<T>(IEnumerable<T> enumerable, object parameter)
        {
            int index;
            var isParameterValidIndex = int.TryParse(parameter.ToString(), out index);

            return enumerable != null && isParameterValidIndex && enumerable.Count() > index
                       ? enumerable.ElementAt(index)
                       : default(T);
        }
    }
}
