// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewSorter.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for ViewSorter.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.TaskBoardUI
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    using TfsWorkbench.TaskBoardUI.DataObjects;

    /// <summary>
    /// Interaction logic for ViewSorter.xaml
    /// </summary>
    public partial class ViewSorter
    {
        /// <summary>
        /// The fields collection instance.
        /// </summary>
        private readonly ObservableCollection<string> fields = new ObservableCollection<string>();

        /// <summary>
        /// The view dependency property.
        /// </summary>
        private static readonly DependencyProperty viewProperty = DependencyProperty.Register(
            "View",
            typeof(SwimLaneView), 
            typeof(ViewSorter),
            new PropertyMetadata(null, OnViewChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewSorter"/> class.
        /// </summary>
        public ViewSorter()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the view property.
        /// </summary>
        /// <value>The view property.</value>
        public static DependencyProperty ViewProperty
        {
            get { return viewProperty; }
        }

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>The view object.</value>
        public SwimLaneView View
        {
            get { return (SwimLaneView)this.GetValue(ViewProperty); }
            set { this.SetValue(ViewProperty, value); } 
        }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <value>The fields.</value>
        public ObservableCollection<string> Fields
        {
            get { return this.fields; }
        }

        /// <summary>
        /// Called when [view changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnViewChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var sorter = dependencyObject as ViewSorter;

            if (sorter == null || e.NewValue == null)
            {
                return;
            }

            sorter.View.ApplySort();
        }

        /// <summary>
        /// Called when [sort order changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnSortOrderChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.View != null)
            {
                this.View.ApplySort();
            }
        }

        /// <summary>
        /// Called when [direction click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnDirectionChanged(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            if (button == null || this.View == null)
            {
                return;
            }

            this.View.ApplySort();
        }
    }
}