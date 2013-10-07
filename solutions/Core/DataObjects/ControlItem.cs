// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlItem.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ControlItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.Serialization;

    using TfsWorkbench.Core.Interfaces;

    /// <summary>
    /// Initializes instance of ControlItem
    /// </summary>
    [Serializable]
    [DesignerCategory(@"code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.workbench/ControlItems")]
    public class ControlItem : IControlItem
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the help text.
        /// </summary>
        /// <value>The help text.</value>
        public string HelpText { get; set; }

        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        /// <value>The display text.</value>
        [XmlAttribute(AttributeName = "displaytext")]
        public string DisplayText { get; set; }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>The name of the field.</value>
        [XmlAttribute(AttributeName = "fieldname")]
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the type of the control.
        /// </summary>
        /// <value>The type of the control.</value>
        [XmlAttribute(AttributeName = "controltype")]
        public string ControlType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute(AttributeName = "readonly")]
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the task board item.
        /// </summary>
        /// <value>The task board item.</value>
        [XmlIgnore]
        public IWorkbenchItem WorkbenchItem { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [XmlIgnore]
        public object Value
        {
            get { return this.WorkbenchItem[this.FieldName]; }
            set { this.WorkbenchItem[this.FieldName] = value; }
        }

        /// <summary>
        /// Gets the allowed values.
        /// </summary>
        /// <value>The allowed values.</value>
        public IEnumerable<object> AllowedValues
        {
            get { return (IEnumerable<object>)this.WorkbenchItem.AllowedValues[this.FieldName]; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has allowed values.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has allowed values; otherwise, <c>false</c>.
        /// </value>
        public bool HasAllowedValues
        {
            get { return this.WorkbenchItem.ValueProvider.HasAllowedValues(this.FieldName); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is limited to allowed values.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is limited to allowed values; otherwise, <c>false</c>.
        /// </value>
        public bool IsLimitedToAllowedValues
        {
            get { return this.WorkbenchItem.ValueProvider.IsLimitedToAllowedValues(this.FieldName); }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A clone of the current instance.</returns>
        public ControlItem Clone()
        {
            return new ControlItem
                {
                    ControlType = this.ControlType,
                    DisplayText = this.DisplayText,
                    FieldName = this.FieldName,
                    HelpText = this.HelpText,
                    IsReadOnly = this.IsReadOnly
                };
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        public void OnPropertyChanged()
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }
    }
}