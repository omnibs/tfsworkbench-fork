// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalControlTemplateSelector.cs" company="EMC Consulting">
//   EMC Consulting 2010
// </copyright>
// <summary>
//   Defines the LocalControlTemplateSelector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Emcc.ScrumMastersWorkbench.ItemListUI
{
    using System.Windows;

    using Core.Interfaces;

    using UIElements.ValueConverters;

    /// <summary>
    /// Initializes instance of LocalControlTemplateSelector
    /// </summary>
    public class LocalControlTemplateSelector : ControlTemplateSelector
    {
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
            var controlItem = item as IControlItem;
            var element = container as FrameworkElement;

            if (controlItem == null || element == null)
            {
                return null;
            }

            return controlItem.ControlType.Equals("HtmlFieldControl")
                ? element.TryFindResource("HtmlFieldControl_SingleLine") as DataTemplate
                : base.SelectTemplate(item, container);
        }
    }
}
