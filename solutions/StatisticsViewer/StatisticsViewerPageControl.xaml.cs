// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatisticsViewerPageControl.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for StatisticsViewerGroupControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.StatisticsViewer
{
    using System.Windows;

    using TfsWorkbench.StatisticsViewer.StatisticsGroups;

    /// <summary>
    /// Interaction logic for StatisticsViewerGroupControl.xaml
    /// </summary>
    public partial class StatisticsViewerPageControl
    {
        /// <summary>
        /// The statistics group property.
        /// </summary>
        private static readonly DependencyProperty statisticsPageProperty = DependencyProperty.Register(
            "StatisticsPage",
            typeof(IStatisticsPage),
            typeof(StatisticsViewerPageControl));

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsViewerPageControl"/> class.
        /// </summary>
        public StatisticsViewerPageControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the statistics group property.
        /// </summary>
        /// <value>The statistics group property.</value>
        public static DependencyProperty StatisticsPageProperty
        {
            get { return statisticsPageProperty; }
        }

        /// <summary>
        /// Gets or sets the statistics group.
        /// </summary>
        /// <value>The statistics group.</value>
        public IStatisticsPage StatisticsPage
        {
            get { return (IStatisticsPage)this.GetValue(StatisticsPageProperty); }
            set { this.SetValue(StatisticsPageProperty, value); }
        }
    }
}
