// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopupTextBoxControl.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for ValueSelectorControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.PopupControls
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ValueSelectorControl.xaml
    /// </summary>
    public partial class PopUpTextBoxControl : IPopupControl
    {
        /// <summary>
        /// The value selections property.
        /// </summary>
        private static readonly DependencyProperty valueProperty = DependencyProperty.Register(
            "Value", typeof(string), typeof(PopUpTextBoxControl));

        /// <summary>
        /// The is readonly property.
        /// </summary>
        private static readonly DependencyProperty isReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly", typeof(bool), typeof(PopUpTextBoxControl));

        /// <summary>
        /// Initializes a new instance of the <see cref="PopUpTextBoxControl"/> class.
        /// </summary>
        public PopUpTextBoxControl()
        {
            this.InitializeComponent();

            PopupControlHelper.Instance.RegisterPopupControl(this);
        }

        /// <summary>
        /// Gets the value selections property.
        /// </summary>
        /// <value>The value selections property.</value>
        public static DependencyProperty ValueProperty
        {
            get
            {
                return valueProperty;
            }
        }

        /// <summary>
        /// Gets the is read only property.
        /// </summary>
        /// <value>The is read only property.</value>
        public static DependencyProperty IsReadOnlyProperty
        {
            get
            {
                return isReadOnlyProperty;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get { return (string)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get { return (bool)this.GetValue(IsReadOnlyProperty); }
            set { this.SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Called when [handle mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void OnHandleMouseDown(object sender, RoutedEventArgs e)
        {
            if (!this.Popup.IsOpen)
            {
                return;
            }

            var source = e.OriginalSource as DependencyObject;

            if (source.IsInstanceOrChildOf(this.ToggleButton) || source.IsInstanceOrChildOf(this.PopupScroller))
            {
                return;
            }

            this.Popup.IsOpen = false;
        }
    }
}