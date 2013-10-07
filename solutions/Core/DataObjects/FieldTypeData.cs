// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldTypeData.cs" company="None">
//   None
// </copyright>
// <summary>
//   The field type data class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.DataObjects
{
    using System;

    /// <summary>
    /// The field type data class.
    /// </summary>
    public class FieldTypeData
    {
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the name of the reference.
        /// </summary>
        /// <value>The name of the reference.</value>
        public string ReferenceName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is display field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is display field; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisplayField { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is double.
        /// </summary>
        /// <value><c>true</c> if this instance is double; otherwise, <c>false</c>.</value>
        public bool IsDouble { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is integer.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is integer; otherwise, <c>false</c>.
        /// </value>
        public bool IsInteger { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is date.
        /// </summary>
        /// <value><c>true</c> if this instance is date; otherwise, <c>false</c>.</value>
        public bool IsDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is editable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is editable; otherwise, <c>false</c>.
        /// </value>
        public bool IsEditable { get; set; }
    }
}
