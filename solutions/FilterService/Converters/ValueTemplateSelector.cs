// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueTemplateSelector.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ValueTemplateSelector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.FilterService.Converters
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.FilterService.Properties;

    /// <summary>
    /// The value template selector
    /// </summary>
    public class ValueTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// The project data service instance.
        /// </summary>
        private readonly IProjectDataService projectDataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTemplateSelector"/> class.
        /// </summary>
        public ValueTemplateSelector()
            : this(ServiceManager.Instance.GetService<IProjectDataService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTemplateSelector"/> class.
        /// </summary>
        /// <param name="projectDataService">The project data service.</param>
        public ValueTemplateSelector(IProjectDataService projectDataService)
        {
            if (projectDataService == null)
            {
                throw new ArgumentNullException("projectDataService");
            }

            this.projectDataService = projectDataService;
        }

        /// <summary>
        /// When overridden in a derived class, returns a <see cref="T:System.Windows.DataTemplate"/> based on custom logic.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>
        /// Returns a <see cref="T:System.Windows.DataTemplate"/> or null. The default value is null.
        /// </returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var filter = item as WorkbenchFilter;
            var element = container as FrameworkElement;

            if (filter == null || element == null)
            {
                return null;
            }

            var projectData = this.projectDataService.CurrentProjectData;

            if (projectData != null && !string.IsNullOrEmpty(filter.FieldName))
            {
                if (filter.FieldName == Resources.String030)
                {
                    return element.TryFindResource("StateSelectorTemplate") as DataTemplate;
                }

                var field = projectData.ItemTypes.SelectMany(it => it.Fields).FirstOrDefault(f => f.DisplayName == filter.FieldName);

                if (field != null)
                {
                    if (field.IsDate)
                    {
                        return element.TryFindResource("DateValueTemplate") as DataTemplate;
                    }

                    if (field.ReferenceName == Core.Properties.Settings.Default.IterationPathFieldName
                        || field.ReferenceName == Core.Properties.Settings.Default.AreaPathFieldName)
                    {
                        return element.TryFindResource("ProjectPathValueTemplate") as DataTemplate;
                    }
                }
            }

            return element.TryFindResource("DefaultValueTemplate") as DataTemplate;
        }
    }
}
