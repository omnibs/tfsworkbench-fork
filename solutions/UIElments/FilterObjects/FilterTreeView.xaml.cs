// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterTreeView.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for ComboBoxTreeView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Input;

namespace TfsWorkbench.UIElements.FilterObjects
{
    /// <summary>
    /// Interaction logic for ComboBoxTreeView.xaml
    /// </summary>
    public partial class FilterTreeView
    {
        /// <summary>
        /// The filter collection property.
        /// </summary>
        private static readonly DependencyProperty filterCollectionProperty = DependencyProperty.Register(
            "FilterCollection",
            typeof(FilterCollection),
            typeof(FilterTreeView),
            new PropertyMetadata(null, OnFilterCollectionChanged));

        /// <summary>
        /// The selected item count property.
        /// </summary>
        private static readonly DependencyProperty selectedItemCountProperty = DependencyProperty.Register(
            "SelectedItemCount",
            typeof(int),
            typeof(FilterTreeView));

        /// <summary>
        /// The handle mouse down delegate;
        /// </summary>
        private readonly RoutedEventHandler handleMouseDown;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterTreeView"/> class.
        /// </summary>
        public FilterTreeView()
        {
            this.handleMouseDown = this.OnHandleMouseDown;

            this.InitializeComponent();

            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.AddHandler(MouseDownEvent, this.handleMouseDown, true);
            }
        }

        /// <summary>
        /// Gets the Filters property.
        /// </summary>
        /// <value>The Filters property.</value>
        public static DependencyProperty FilterCollectionProperty
        {
            get { return filterCollectionProperty; }
        }

        /// <summary>
        /// Gets the selected item count property.
        /// </summary>
        /// <value>The selected item count property.</value>
        public static DependencyProperty SelectedItemCountProperty
        {
            get { return selectedItemCountProperty; }
        }

        /// <summary>
        /// Gets or sets the filters.
        /// </summary>
        /// <value>The filters.</value>
        public FilterCollection FilterCollection
        {
            get { return (FilterCollection)this.GetValue(FilterCollectionProperty); }
            set { this.SetValue(FilterCollectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selected item count.
        /// </summary>
        /// <value>The selected item count.</value>
        public int SelectedItemCount
        {
            get { return (int)this.GetValue(SelectedItemCountProperty); }
            set { this.SetValue(SelectedItemCountProperty, value); }
        }

        /// <summary>
        /// Expands this instance.
        /// </summary>
        public void Expand()
        {
            this.PART_TreeViewPopup.IsOpen = true;
        }

        /// <summary>
        /// Called when [filter collection changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFilterCollectionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as FilterTreeView;

            if (control == null)
            {
                return;
            }

            var oldValue = e.OldValue as FilterCollection;
            if (oldValue != null)
            {
                oldValue.SelectionChanged -= control.OnFilterSelectionChanged;
            }

            var newValue = e.NewValue as FilterCollection;
            if (newValue != null)
            {
                newValue.SelectionChanged += control.OnFilterSelectionChanged;
            }
        }

        /// <summary>
        /// Called when [filter selection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnFilterSelectionChanged(object sender, EventArgs e)
        {
            this.SelectedItemCount = this.FilterCollection.SelectedWorkbenchItemCount;
        }

        /// <summary>
        /// Called when [handle mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnHandleMouseDown(object sender, RoutedEventArgs e)
        {
            if (!this.PART_TreeViewPopup.IsOpen)
            {
                return;
            }

            var source = e.OriginalSource as DependencyObject;

            try
            {
                if (source.IsInstanceOrChildOf(this.PART_ToggleButton) || source.IsInstanceOrChildOf(this.PART_MainTreeView))
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                if (CommandLibrary.ApplicationExceptionCommand.CanExecute(ex, this))
                {
                    CommandLibrary.ApplicationExceptionCommand.Execute(ex, this);
                }
                else
                {
                    throw;
                }
            }

            this.PART_TreeViewPopup.IsOpen = false;
        }

        /// <summary>
        /// Called when [show popup].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnShowPopup(object sender, RoutedEventArgs e)
        {
            this.PART_MainTreeView.Focus();
        }

        /// <summary>
        /// Handles the KeyDown event of the TreeView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void OnTreeViewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.PART_TreeViewPopup.IsOpen = false;
            }
        }
    }
}