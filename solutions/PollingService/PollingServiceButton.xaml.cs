// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PollingServiceButton.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for ChangePollerStatusButton.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.PollingService
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ChangePollerStatusButton.xaml
    /// </summary>
    public partial class PollingServiceButton
    {
        /// <summary>
        /// The changePoller dependency property.
        /// </summary>
        private static readonly DependencyProperty changePollerProperty = DependencyProperty.Register(
            "ChangePoller",
            typeof(ChangePoller),
            typeof(PollingServiceButton),
            new PropertyMetadata(null, OnChangePollerChange));

        /// <summary>
        /// Initializes a new instance of the <see cref="PollingServiceButton"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public PollingServiceButton(IPollingServiceController controller)
        {
            InitializeComponent();

            this.ChangePoller = controller.ChangePoller;
        }

        /// <summary>
        /// Gets the ChangePoller property.
        /// </summary>
        /// <value>The name property.</value>
        public static DependencyProperty ChangePollerProperty
        {
            get { return changePollerProperty; }
        }

        /// <summary>
        /// Gets or sets the change poller.
        /// </summary>
        /// <value>The change poller.</value>
        /// <summary>
        /// Gets or sets the instance ChangePoller.
        /// </summary>
        /// <returns>The instance ChangePoller.</returns>
        public ChangePoller ChangePoller
        {
            get { return (ChangePoller)this.GetValue(ChangePollerProperty); }
            set { this.SetValue(ChangePollerProperty, value); }
        }

        /// <summary>
        /// Called when [change poller change].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnChangePollerChange(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as PollingServiceButton;
            if (control == null)
            {
                return;
            }

            control.Visibility = control.ChangePoller == null ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
