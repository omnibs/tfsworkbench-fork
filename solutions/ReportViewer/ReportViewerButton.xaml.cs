// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportViewerButton.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for ReportViewerButton.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ReportViewer
{
    using System;
    using System.Windows;

    /// <summary>
    /// Interaction logic for ReportViewerButton.xaml
    /// </summary>
    public partial class ReportViewerButton
    {
        /// <summary>
        /// The controller property.
        /// </summary>
        private static readonly DependencyProperty controllerProperty = DependencyProperty.Register(
            "Controller", typeof(IReportController), typeof(ReportViewerButton));

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportViewerButton"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public ReportViewerButton(IReportController controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            this.Controller = controller;
            InitializeComponent();
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
        public IReportController Controller
        {
            get { return (IReportController)this.GetValue(ControllerProperty); }
            set { this.SetValue(ControllerProperty, value); }
        }
    }
}
