// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueSelectorControl.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for ValueSelectorControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.PopupControls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;

    using TfsWorkbench.Core.DataObjects;

    /// <summary>
    /// Interaction logic for ValueSelectorControl.xaml
    /// </summary>
    public partial class ValueSelectorControl : IPopupControl
    {
        /// <summary>
        /// The value selections property.
        /// </summary>
        private static readonly DependencyProperty valueSelectionsProperty = DependencyProperty.Register(
            "ValueSelections", typeof(Collection<SelectedValue>), typeof(ValueSelectorControl));

        /// <summary>
        /// The display text property.
        /// </summary>
        private static readonly DependencyProperty displayTextProperty = DependencyProperty.Register(
            "DisplayText", typeof(string), typeof(ValueSelectorControl));

        /// <summary>
        /// The show select all property.
        /// </summary>
        private static readonly DependencyProperty showSelectAllProperty = DependencyProperty.Register(
            "ShowSelectAll", typeof(bool), typeof(ValueSelectorControl));

        /// <summary>
        /// The ignore check change flag.
        /// </summary>
        private bool ignoreCheckChange;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueSelectorControl"/> class.
        /// </summary>
        public ValueSelectorControl()
        {
            this.InitializeComponent();

            PopupControlHelper.Instance.RegisterPopupControl(this);
        }

        /// <summary>
        /// Occurs when the [value selection changed].
        /// </summary>
        public event EventHandler ValueSelectionChanged;

        /// <summary>
        /// Gets the value selections property.
        /// </summary>
        /// <value>The value selections property.</value>
        public static DependencyProperty ValueSelectionsProperty
        {
            get
            {
                return valueSelectionsProperty;
            }
        }

        /// <summary>
        /// Gets the display text property.
        /// </summary>
        /// <value>The display text property.</value>
        public static DependencyProperty DisplayTextProperty
        {
            get
            {
                return displayTextProperty;
            }
        }

        /// <summary>
        /// Gets the show select all property.
        /// </summary>
        /// <value>The show select all property.</value>
        public static DependencyProperty ShowSelectAllProperty
        {
            get { return showSelectAllProperty; }
        }

        /// <summary>
        /// Gets or sets the value selections.
        /// </summary>
        /// <value>The value selections.</value>
        public Collection<SelectedValue> ValueSelections
        {
            get { return (Collection<SelectedValue>)this.GetValue(ValueSelectionsProperty); }
            set { this.SetValue(ValueSelectionsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        /// <value>The display text.</value>
        public string DisplayText
        {
            get { return (string)this.GetValue(DisplayTextProperty); }
            set { this.SetValue(DisplayTextProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show select all].
        /// </summary>
        /// <value><c>true</c> if [show select all]; otherwise, <c>false</c>.</value>
        public bool ShowSelectAll
        {
            get { return (bool)this.GetValue(ShowSelectAllProperty); }
            set { this.SetValue(ShowSelectAllProperty, value); }
        }

        /// <summary>
        /// Called when [handle mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void OnHandleMouseDown(object sender, RoutedEventArgs e)
        {
            if (!this.PART_Popup.IsOpen)
            {
                return;
            }

            var source = e.OriginalSource as DependencyObject;

            if (this.IsIgnoredSource(source))
            {
                return;
            }

            this.PART_Popup.IsOpen = false;
        }

        /// <summary>
        /// Called when the selected values change.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCheckChange(object sender, RoutedEventArgs e)
        {
            if (!this.ignoreCheckChange && this.ValueSelectionChanged != null)
            {
                this.ValueSelectionChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Determines whether [the specified source] [is ignored].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        /// <c>true</c> if [the specified source] [is ignored]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsIgnoredSource(DependencyObject source)
        {
            return
                source.IsInstanceOrChildOf(this.PART_SelectAllButton)
                || source.IsInstanceOrChildOf(this.PART_DeselectAllButton)
                || source.IsInstanceOrChildOf(this.PART_ToggleButton)
                || source.IsInstanceOrChildOf(this.PART_PopupListView);
        }

        /// <summary>
        /// Called when [select all_ click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnSelectAllClick(object sender, RoutedEventArgs e)
        {
            this.ignoreCheckChange = true;

            foreach (var valueSelection in this.ValueSelections.Where(valueSelection => !valueSelection.IsSelected).ToArray())
            {
                valueSelection.IsSelected = true;
            }

            this.ignoreCheckChange = false;

            this.OnCheckChange(sender, e);

            e.Handled = true;
        }

        /// <summary>
        /// Called when [deselect all_ click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnDeselectAllClick(object sender, RoutedEventArgs e)
        {
            this.ignoreCheckChange = true;

            foreach (var valueSelection in this.ValueSelections.Where(valueSelection => valueSelection.IsSelected).ToArray())
            {
                valueSelection.IsSelected = false;
            }

            this.ignoreCheckChange = false;

            this.OnCheckChange(sender, e);

            e.Handled = true;
        }
    }
}