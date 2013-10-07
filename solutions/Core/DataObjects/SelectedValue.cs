// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectedValue.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the SelectedValue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.DataObjects
{
    using System.Xml.Serialization;

    /// <summary>
    /// Initializes instance of SelectedValue
    /// </summary>
    public class SelectedValue : NotifierBase
    {
        /// <summary>
        /// The value.
        /// </summary>
        private string text;

        /// <summary>
        /// The is selected flag.
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [XmlText]
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.UpdateWithNotification("Value", value, ref this.text);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute(AttributeName = "selected")]
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                this.UpdateWithNotification("IsSelected", value, ref this.isSelected);
            }
        }
    }
}
