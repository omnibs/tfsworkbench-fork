// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IControlItem.cs" company="None">
//   None
// </copyright>
// <summary>
//   The control item interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Interfaces
{
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// The control item interface
    /// </summary>
    public interface IControlItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the help text.
        /// </summary>
        /// <value>The help text.</value>
        string HelpText { get; set; }

        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        /// <value>The display text.</value>
        string DisplayText { get; set; }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>The name of the field.</value>
        string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the type of the control.
        /// </summary>
        /// <value>The type of the control.</value>
        string ControlType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the task board item.
        /// </summary>
        /// <value>The task board item.</value>
        IWorkbenchItem WorkbenchItem { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        object Value { get; set; }

        /// <summary>
        /// Gets the allowed values.
        /// </summary>
        /// <value>The allowed values.</value>
        IEnumerable<object> AllowedValues { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has allowed values.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has allowed values; otherwise, <c>false</c>.
        /// </value>
        bool HasAllowedValues { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is limited to allowed values.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is limited to allowed values; otherwise, <c>false</c>.
        /// </value>
        bool IsLimitedToAllowedValues { get; }
    }
}