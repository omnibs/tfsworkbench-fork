// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiSelectControlItem.cs" company="None">
//   None
// </copyright>
// <summary>
//   The multi select control item class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ItemListUI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using Core.Interfaces;

    /// <summary>
    /// The multi select control item class.
    /// </summary>
    public class MultiSelectControlItem : IControlItem
    {
        /// <summary>
        /// The base control item.
        /// </summary>
        private IControlItem baseControlItem;

        /// <summary>
        /// The is selected flag.
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiSelectControlItem"/> class.
        /// </summary>
        /// <param name="baseControlItem">The base control item.</param>
        public MultiSelectControlItem(IControlItem baseControlItem)
        {
            if (baseControlItem == null)
            {
                throw new ArgumentNullException("baseControlItem");
            }

            this.baseControlItem = baseControlItem;
            this.baseControlItem.PropertyChanged += this.OnPropertyChanged;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets a value indicating whether this instance is value update source.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is value update source; otherwise, <c>false</c>.
        /// </value>
        public bool IsValueUpdateSource { get; private set; }

        /// <summary>
        /// Gets or sets the help text.
        /// </summary>
        /// <value>The help text.</value>
        public string HelpText
        {
            get { return this.baseControlItem.HelpText; }
            set { this.baseControlItem.HelpText = value; }
        }

        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        /// <value>The display text.</value>
        public string DisplayText
        {
            get { return this.baseControlItem.DisplayText; }
            set { this.baseControlItem.DisplayText = value; }
        }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>The name of the field.</value>
        public string FieldName
        {
            get { return this.baseControlItem.FieldName; }
            set { this.baseControlItem.FieldName = value; }
        }

        /// <summary>
        /// Gets or sets the type of the control.
        /// </summary>
        /// <value>The type of the control.</value>
        public string ControlType
        {
            get { return this.baseControlItem.ControlType; }
            set { this.baseControlItem.ControlType = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get { return this.baseControlItem.IsReadOnly; }
            set { this.baseControlItem.IsReadOnly = value; }
        }

        /// <summary>
        /// Gets or sets the task board item.
        /// </summary>
        /// <value>The task board item.</value>
        public IWorkbenchItem WorkbenchItem
        {
            get { return this.baseControlItem.WorkbenchItem; }
            set { this.baseControlItem.WorkbenchItem = value; }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get
            {
                return this.baseControlItem.Value;
            }

            set
            {
                this.IsValueUpdateSource = true;
                this.baseControlItem.Value = value;
                this.IsValueUpdateSource = false;
            }
        }

        /// <summary>
        /// Gets the allowed values.
        /// </summary>
        /// <value>The allowed values.</value>
        public IEnumerable<object> AllowedValues
        {
            get { return this.baseControlItem.AllowedValues; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has allowed values.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has allowed values; otherwise, <c>false</c>.
        /// </value>
        public bool HasAllowedValues
        {
            get { return this.baseControlItem.HasAllowedValues; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is limited to allowed values.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is limited to allowed values; otherwise, <c>false</c>.
        /// </value>
        public bool IsLimitedToAllowedValues
        {
            get { return this.baseControlItem.IsLimitedToAllowedValues; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (this.isSelected == value)
                {
                    return;
                }

                this.isSelected = value;

                this.OnPropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            this.baseControlItem.PropertyChanged -= this.OnPropertyChanged;
            this.baseControlItem = null;
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged == null)
            {
                return;
            }

            this.PropertyChanged(this, e);
        }
    }
}