// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterServiceView.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for FilterView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.FilterService
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    using TfsWorkbench.UIElements;

    /// <summary>
    /// Interaction logic for FilterView.xaml
    /// </summary>
    public partial class FilterServiceView : IFilterServiceView
    {
        /// <summary>
        /// The controller dependency property.
        /// </summary>
        private static readonly DependencyProperty controllerProperty = DependencyProperty.Register(
            "Controller",
            typeof(IFilterServiceController),
            typeof(FilterServiceView));

        /// <summary>
        /// The workbenchFilter dependency property.
        /// </summary>
        private static readonly DependencyProperty workbenchFilterProperty = DependencyProperty.Register(
            "WorkbenchFilter",
            typeof(WorkbenchFilter),
            typeof(FilterServiceView));

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterServiceView"/> class.
        /// </summary>
        public FilterServiceView()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterServiceView"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public FilterServiceView(IFilterServiceController controller)
        {
            this.Controller = controller;

            InitializeComponent();
        }

        /// <summary>
        /// Gets the WorkbenchFilter property.
        /// </summary>
        /// <value>The name property.</value>
        public static DependencyProperty WorkbenchFilterProperty
        {
            get { return workbenchFilterProperty; }
        }

        /// <summary>
        /// Gets the Controller property.
        /// </summary>
        /// <value>The name property.</value>
        public static DependencyProperty ControllerProperty
        {
            get { return controllerProperty; }
        }

        /// <summary>
        /// Gets or sets the instance Controller.
        /// </summary>
        /// <returns>The instance Controller.</returns>
        public IFilterServiceController Controller
        {
            get { return (IFilterServiceController)this.GetValue(ControllerProperty); }
            set { this.SetValue(ControllerProperty, value); }
        }

        /// <summary>
        /// Gets the value entry control.
        /// </summary>
        /// <value>The value entry control.</value>
        public ContentControl ValueCotnrol
        {
            get
            {
                return this.PART_Value;
            }
        }

        /// <summary>
        /// Gets or sets the instance WorkbenchFilter.
        /// </summary>
        /// <returns>The instance WorkbenchFilter.</returns>
        public WorkbenchFilter WorkbenchFilter
        {
            get { return (WorkbenchFilter)this.GetValue(WorkbenchFilterProperty); }
            set { this.SetValue(WorkbenchFilterProperty, value); }
        }

        /// <summary>
        /// Called when [close button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Controller.CloseFilterDialog();
        }

        /// <summary>
        /// Called when [selection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnFilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LocalCommandLibrary.SelectFilterCommand.Execute(
                e.AddedItems.OfType<WorkbenchFilter>().FirstOrDefault(), this);
        }
    }
}
