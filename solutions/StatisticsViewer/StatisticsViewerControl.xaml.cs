// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatisticsViewerControl.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for ReportViewerControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.StatisticsViewer
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for StatisticsViewerControl.xaml
    /// </summary>
    public partial class StatisticsViewerControl
    {
        /// <summary>
        /// The statistics controller.
        /// </summary>
        private readonly IStatisticsController controller;

        /// <summary>
        /// The has rendered flag.
        /// </summary>
        private bool hasRendered;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsViewerControl"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public StatisticsViewerControl(IStatisticsController controller)
        {
            this.InitializeComponent();
            this.controller = controller;
        }

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (!this.hasRendered)
            {
                this.hasRendered = true;
                this.BuildTabs();
            }

            base.OnRender(drawingContext);
        }

        /// <summary>
        /// Populates the tabs.
        /// </summary>
        private void BuildTabs()
        {
            foreach (var statisticGroup in this.controller.StatisticPages)
            {
                var groupControl = new StatisticsViewerPageControl { StatisticsPage = statisticGroup };

                var tabItem = new TabItem { DataContext = statisticGroup, Content = groupControl };

                tabItem.SetBinding(HeaderedContentControl.HeaderProperty, "PageTitle");

                this.PART_TabControl.Items.Add(tabItem);
            }

            if (this.PART_TabControl.Items.Count > 0)
            {
                this.PART_TabControl.SelectedItem = this.PART_TabControl.Items[0];
            }
        }

        /// <summary>
        /// Called when [reset button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnResetButtonClick(object sender, RoutedEventArgs e)
        {
            this.PART_TabControl.Items.Clear();
            this.BuildTabs();
        }
    }
}
