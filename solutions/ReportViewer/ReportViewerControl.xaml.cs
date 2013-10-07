// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportViewerControl.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for ReportViewerControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ReportViewer
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for ReportViewerControl.xaml
    /// </summary>
    public partial class ReportViewerControl
    {
        /// <summary>
        /// The report controller.
        /// </summary>
        private readonly IReportController controller;

        /// <summary>
        /// The report root property.
        /// </summary>
        private static readonly DependencyProperty reportRootProperty = DependencyProperty.Register(
            "ReportRoot",
            typeof(IReportNode),
            typeof(ReportViewerControl));

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportViewerControl"/> class.
        /// </summary>
        public ReportViewerControl()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportViewerControl"/> class.
        /// </summary>
        /// <param name="controller">The contorller.</param>
        public ReportViewerControl(IReportController controller)
        {
            InitializeComponent();

            if (controller == null)
            {
                return;
            }

            this.controller = controller;
            this.CommandBindings.Add(new CommandBinding(LocalCommandLibrary.ShowReportCommand, this.OnShowReport, this.CanShowReportViewer));
        }

        /// <summary>
        /// Gets the report root property.
        /// </summary>
        /// <value>The report root property.</value>
        public static DependencyProperty ReportRootProperty
        {
            get
            {
                return reportRootProperty;
            }
        }

        /// <summary>
        /// Gets or sets the report root.
        /// </summary>
        /// <value>The report root.</value>
        public IReportNode ReportRoot
        {
            get { return (IReportNode)this.GetValue(ReportRootProperty); }
            set { this.SetValue(ReportRootProperty, value); }
        }

        /// <summary>
        /// Determines whether this instance [can show report viewer] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanShowReportViewer(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.controller.HasLoadedReportList;
        }

        /// <summary>
        /// Called when [show report].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void OnShowReport(object sender, ExecutedRoutedEventArgs e)
        {
            var catalogItem = e.Parameter as CatalogItemBase;
            if (catalogItem == null)
            {
                return;
            }

            this.controller.ShowReport(this, catalogItem);
        }

        /// <summary>
        /// Called when [text block mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnTextBlockMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2)
            {
                return;
            }

            var source = sender as TextBlock;
            if (source == null)
            {
                return;
            }

            var context = source.DataContext as IReportNode;
            if (context == null || context.CatalogItem == null || !context.CatalogItem.IsReport)
            {
                return;
            }

            this.controller.ShowReport(this, context.CatalogItem);
        }
    }
}
