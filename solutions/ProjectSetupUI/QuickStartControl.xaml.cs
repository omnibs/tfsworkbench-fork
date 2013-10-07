// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuickStartControl.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for QuickStartControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;

    using DataObjects;

    using Helpers;

    /// <summary>
    /// Interaction logic for QuickStartControl.xaml
    /// </summary>
    public partial class QuickStartControl
    {
        /// <summary>
        /// The project setup property.
        /// </summary>
        private static readonly DependencyProperty projectSetupProperty = DependencyProperty.Register(
            "ProjectSetup",
            typeof(ProjectSetup),
            typeof(QuickStartControl),
            new PropertyMetadata(null, OnProjectSetupChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickStartControl"/> class.
        /// </summary>
        public QuickStartControl()
        {
            this.InitializeComponent();

            this.DataContext = this;
        }

        /// <summary>
        /// Gets the project setup property.
        /// </summary>
        /// <value>The project setup property.</value>
        public static DependencyProperty ProjectSetupProperty
        {
            get { return projectSetupProperty; }
        }

        /// <summary>
        /// Gets or sets the project setup.
        /// </summary>
        /// <value>The project setup.</value>
        internal override ProjectSetup ProjectSetup
        {
            get { return (ProjectSetup)this.GetValue(ProjectSetupProperty); }
            set { this.SetValue(ProjectSetupProperty, value); }
        }

        /// <summary>
        /// Determines whether this instance is valid.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsValid()
        {
            this.ValidationErrors.Text = string.Empty;

            var startDate = this.ProjectSetup.StartDate;
            var endDate = this.ProjectSetup.EndDate;

            if (!ValidationHelper.IsValidDateRange(startDate, endDate))
            {
                this.AddErrorMessage("The project dates are not valid.");
            }

            if (!ValidationHelper.IsValidName(this.ProjectSetup.Teams[0].Name))
            {
                this.AddErrorMessage("Team name is not valid.");
            }

            if (!this.ProjectSetup.Teams[0].HasValidCapacity)
            {
                this.AddErrorMessage("The team capacity is not valid.");
            }

            if (!ValidationHelper.IsValidWorkStream(this.ProjectSetup.Teams[0].WorkStream))
            {
                this.AddErrorMessage("The sprint length is not valid.");
            }

            return string.IsNullOrEmpty(this.ValidationErrors.Text);
        }

        /// <summary>
        /// Called when [setup property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void OnSetupPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var release = this.ProjectSetup.Releases.FirstOrDefault();

            if (release == null)
            {
                return;
            }

            if (e.PropertyName.Equals("StartDate"))
            {
                // Update the release object start date.
                release.StartDate = this.ProjectSetup.StartDate;
            }

            if (e.PropertyName.Equals("EndDate"))
            {
                // Update the release object end date.
                release.EndDate = this.ProjectSetup.EndDate;
            }
        }

        /// <summary>
        /// Handles the Click event of the ExecuteButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            this.ExecuteSetup();
        }

        /// <summary>
        /// Concats the error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        private void AddErrorMessage(string errorMessage)
        {
            this.ValidationErrors.Text = 
                string.Concat(
                    this.ValidationErrors.Text,
                    string.IsNullOrEmpty(this.ValidationErrors.Text) ? string.Empty : Environment.NewLine,
                    errorMessage);
        }
    }
}
