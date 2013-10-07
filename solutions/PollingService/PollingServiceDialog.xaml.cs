// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PollingServiceDialog.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for EditItemControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.PollingService
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    using TfsWorkbench.PollingService.Properties;
    using TfsWorkbench.UIElements;

    /// <summary>
    /// Interaction logic for EditItemControl.xaml
    /// </summary>
    public partial class PollingServiceDialog
    {
        /// <summary>
        /// The changePoller dependency property.
        /// </summary>
        private static readonly DependencyProperty changePollerProperty = DependencyProperty.Register(
            "ChangePoller",
            typeof(ChangePoller),
            typeof(PollingServiceDialog));

        /// <summary>
        /// The dispatch timer.
        /// </summary>
        private readonly DispatcherTimer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PollingServiceDialog"/> class.
        /// </summary>
        public PollingServiceDialog()
        {
            this.InitializeComponent();

            this.timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };

            this.timer.Tick += (s, e) =>
                {
                    var binding = this.PART_NextPoll.GetBindingExpression(TextBox.TextProperty);
                    if (binding != null)
                    {
                        binding.UpdateTarget();
                    }
                };

            this.timer.Start();
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
        /// Handles the Click event of the CloseButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            CommandLibrary.CloseDialogCommand.Execute(this, this);
            this.timer.Stop();
        }

        /// <summary>
        /// Handles the Click event of the ServiceStatusButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnServiceStatusButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.ChangePoller == null || !this.ChangePoller.IsValid)
            {
                return;
            }

            if (this.ChangePoller.IsRunning)
            {
                this.ChangePoller.Stop();
                Settings.Default.ChangePollingEnabled = false;
            }
            else
            {
                this.ChangePoller.Start();
                Settings.Default.ChangePollingEnabled = true;
            }
        }
    }
}