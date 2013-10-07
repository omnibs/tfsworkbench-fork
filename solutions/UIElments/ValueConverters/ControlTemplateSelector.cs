// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlTemplateSelector.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ControlTemplateSelector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.ValueConverters
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    using Core.Interfaces;

    /// <summary>
    /// Initializes instance of ControlTemplateSelector
    /// </summary>
    public class ControlTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// The control data template cache.
        /// </summary>
        private readonly IDictionary<string, DataTemplate> controlTemplates = new Dictionary<string, DataTemplate>();

        /// <summary>
        /// The default control template name.
        /// </summary>
        private const string DefaultControlTemplate = "TextBlockControl";

        /// <summary>
        /// The field control type name.
        /// </summary>
        private const string FieldControlType = "FieldControl";

        /// <summary>
        /// Field control text box template name.
        /// </summary>
        private const string FieldControlTextBoxTemplate = "FieldControlTextBox";

        /// <summary>
        /// Field control drop down template name.
        /// </summary>
        private const string FieldControlDropDown = "FieldControlDropDown";

        /// <summary>
        /// Field control combo box template name.
        /// </summary>
        private const string FieldControlComboBox = "FieldControlComboBox";

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

            string templateName;
            DataTemplate resource;

            if (controlItem.ControlType == FieldControlType)
            {
                if (!controlItem.HasAllowedValues)
                {
                    templateName = FieldControlTextBoxTemplate;
                }
                else if (controlItem.IsLimitedToAllowedValues)
                {
                    templateName = FieldControlDropDown;
                }
                else
                {
                    templateName = FieldControlComboBox;
                }
            }
            else
            {
                templateName = controlItem.ControlType;
            }

            if (!this.controlTemplates.TryGetValue(templateName, out resource))
            {
                resource = element.TryFindResource(templateName) as DataTemplate ??
                           (DataTemplate)element.FindResource(DefaultControlTemplate);

                this.controlTemplates.Add(templateName, resource);
            }

            return resource;
        }
    }
}