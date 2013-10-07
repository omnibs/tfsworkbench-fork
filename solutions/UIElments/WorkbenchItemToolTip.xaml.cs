// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkbenchItemToolTip.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for WorkbenchItemToolTip.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    using Core.Interfaces;

    using TfsWorkbench.Core.Services;

    /// <summary>
    /// Interaction logic for WorkbenchItemToolTip.xaml
    /// </summary>
    public partial class WorkbenchItemToolTip
    {
        private readonly IProjectDataService projectDataService;

        /// <summary>
        /// The workbecnh item property.
        /// </summary>
        private static readonly DependencyProperty workbenchItemProperty = DependencyProperty.Register(
            "WorkbenchItem",
            typeof(IWorkbenchItem),
            typeof(WorkbenchItemToolTip));

        /// <summary>
        /// The contorl item collection property.
        /// </summary>
        private static readonly DependencyProperty controlItemGroupProperty = DependencyProperty.Register(
            "ControlItemGroup", 
            typeof(IControlItemGroup), 
            typeof(WorkbenchItemToolTip));

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchItemToolTip"/> class.
        /// </summary>
        public WorkbenchItemToolTip()
            : this(ServiceManager.Instance.GetService<IProjectDataService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchItemToolTip"/> class.
        /// </summary>
        /// <param name="projectDataService">The project data service.</param>
        public WorkbenchItemToolTip(IProjectDataService projectDataService)
        {
            if (projectDataService == null)
            {
                throw new ArgumentNullException("projectDataService");
            }

            this.InitializeComponent();

            this.projectDataService = projectDataService;
        }

        /// <summary>
        /// Gets the control item collection property.
        /// </summary>
        /// <value>The control item collection property.</value>
        public static DependencyProperty ControlItemGroupProperty
        {
            get { return controlItemGroupProperty; }
        }

        /// <summary>
        /// Gets the workbench item property.
        /// </summary>
        /// <value>The workbench item property.</value>
        public static DependencyProperty WorkbenchItemProperty
        {
            get { return workbenchItemProperty; }
        }

        /// <summary>
        /// Gets or sets the control items.
        /// </summary>
        /// <value>The control items.</value>
        public IControlItemGroup ControlItemGroup
        {
            get { return (IControlItemGroup)this.GetValue(ControlItemGroupProperty); }
            set { this.SetValue(ControlItemGroupProperty, value); }
        }

        /// <summary>
        /// Gets or sets the workbench item.
        /// </summary>
        /// <value>The workbench item.</value>
        public IWorkbenchItem WorkbenchItem
        {
            get { return (IWorkbenchItem)this.GetValue(WorkbenchItemProperty); }
            set { this.SetValue(WorkbenchItemProperty, value); }
        }

        /// <summary>
        /// Called when [tool tip closed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnToolTipClosed(object sender, RoutedEventArgs e)
        {
            if (this.ControlItemGroup == null)
            {
                return;
            }

            this.ControlItemGroup.WorkbenchItem = null;
            this.ControlItemGroup.ControlItems.Clear();
            this.ControlItemGroup = null;
        }

        /// <summary>
        /// Called when [tool tip opened].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnToolTipOpened(object sender, RoutedEventArgs e)
        {
            if (this.WorkbenchItem == null)
            {
                return;
            }

            DependencyObject visualParent = this.PlacementTarget;

            ToolTipService.SetShowDuration(visualParent, 3600000);

            this.ControlItemGroup =
                this.projectDataService.CurrentDataProvider.GetControlItemGroup(this.WorkbenchItem);

            var dataBinding = this.PART_ValueList.GetBindingExpression(ItemsControl.ItemsSourceProperty);
            if (dataBinding != null)
            {
                dataBinding.UpdateTarget();
            }
        }
    }
}