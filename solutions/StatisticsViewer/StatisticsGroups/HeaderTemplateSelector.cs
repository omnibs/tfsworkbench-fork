// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderTemplateSelector.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the LineTemplatePicker type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.StatisticsViewer.StatisticsGroups
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The line template selector class.
    /// </summary>
    public class HeaderTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Selects the template.
        /// </summary>
        /// <param name="item">The item candidate.</param>
        /// <param name="container">The container.</param>
        /// <returns>The data tempalte for the specfied statistics group.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate output = null;

            var statisticGroup = item as IStatisticsGroup;
            var resourceProvider = container as FrameworkElement;
            if (statisticGroup != null && resourceProvider != null)
            {
                output = resourceProvider.TryFindResource(statisticGroup.HeaderTemplateName) as DataTemplate;
            }

            return output;
        }
    }
}
