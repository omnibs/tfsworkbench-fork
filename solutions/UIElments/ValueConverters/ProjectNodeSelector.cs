// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectNodeSelector.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ProjectNodeSelector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Core.Interfaces;

    using TfsWorkbench.Core.DataObjects;
    using TfsWorkbench.Core.Helpers;

    /// <summary>
    /// Initializes instance of ProjectNodeSelector
    /// </summary>
    public class ProjectNodeSelector : ProjectDataServiceConsumer, IValueConverter
    {
        /// <summary>
        /// Gets the project data.
        /// </summary>
        /// <value>The project data.</value>
        protected IProjectData ProjectData
        {
            get
            {
                return this.ProjectDataService.CurrentProjectData;
            }
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fieldName = value as string;

            if (string.IsNullOrEmpty(fieldName))
            {
                return null;
            }

            return this.ProjectData.ProjectNodes[fieldName];
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}