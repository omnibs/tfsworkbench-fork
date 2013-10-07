// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PollingServiceMenuItem.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for ReportViewerMenuItem.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.PollingService
{
    using System;
    using System.Windows;

    /// <summary>
    /// Interaction logic for ReportViewerMenuItem.xaml
    /// </summary>
    public partial class PollingServiceMenuItem
    {
        /// <summary>
        /// The controller property.
        /// </summary>
        private static readonly DependencyProperty controllerProperty = DependencyProperty.Register(
            "Controller", typeof(IPollingServiceController), typeof(PollingServiceMenuItem));

        /// <summary>
        /// Initializes a new instance of the <see cref="PollingServiceMenuItem"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public PollingServiceMenuItem(IPollingServiceController controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            InitializeComponent();

            this.Controller = controller;
        }

        /// <summary>
        /// Gets the controller property.
        /// </summary>
        /// <value>The controller property.</value>
        public static DependencyProperty ControllerProperty
        {
            get { return controllerProperty; }
        }

        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        /// <value>The controller.</value>
        public IPollingServiceController Controller
        {
            get { return (IPollingServiceController)this.GetValue(ControllerProperty); }
            set { this.SetValue(ControllerProperty, value); }
        }
    }
}
