// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdvancedSetupControl.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for AdvancedSetupControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using DataObjects;

    using Helpers;

    /// <summary>
    /// Interaction logic for AdvancedSetupControl.xaml
    /// </summary>
    public partial class AdvancedSetupControl
    {
        /// <summary>
        /// The project setup property.
        /// </summary>
        private static readonly DependencyProperty projectSetupProperty = DependencyProperty.Register(
            "ProjectSetup",
            typeof(ProjectSetup),
            typeof(AdvancedSetupControl), 
            new PropertyMetadata(null, OnProjectSetupChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedSetupControl"/> class.
        /// </summary>
        public AdvancedSetupControl()
        {
            this.InitializeComponent();

            this.DataContext = this;

            this.SetupCommandBindings();
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

            if (!ValidationHelper.IsValidDateRange(this.ProjectSetup.StartDate, this.ProjectSetup.EndDate))
            {
                this.AddErrorMessage("The specified project date range is not valid.");
            }

            if (this.ProjectSetup.Releases.Count() == 0)
            {
                this.AddErrorMessage("You must specfiy at least one valid release.");
            }
            else
            {
                if (!ValidationHelper.HasUniqueNames(this.ProjectSetup.Releases.Cast<INamedItem>()))
                {
                    this.AddErrorMessage("Release names must be unique.");
                }

                foreach (var release in this.ProjectSetup.Releases)
                {
                    var releaseStartDate = release.StartDate;
                    var releaseEndDate = release.EndDate;
                    if (!ValidationHelper.IsValidDateRange(releaseStartDate, releaseEndDate))
                    {
                        this.AddErrorMessage(
                            string.Format(CultureInfo.InvariantCulture, "'{0}' date range is not valid.", release.Name));
                    }
                }
            }

            if (this.ProjectSetup.WorkStreams.Count() == 0)
            {
                this.AddErrorMessage("You must specfiy at least one valid work stream.");
            }
            else
            {
                if (!ValidationHelper.HasUniqueNames(this.ProjectSetup.WorkStreams.Cast<INamedItem>()))
                {
                    this.AddErrorMessage("Work stream names must be unique.");
                }

                foreach (var workStream in this.ProjectSetup.WorkStreams)
                {
                    if (!ValidationHelper.IsValidName(workStream.Name))
                    {
                        this.AddErrorMessage(
                            string.Format(CultureInfo.InvariantCulture, "Work stream '{0}' name not valid.", workStream.Name));
                    }

                    if (workStream.Cadance <= 0)
                    {
                        this.AddErrorMessage(
                            string.Format(CultureInfo.InvariantCulture, "Work stream '{0}' has an invalid cadance.", workStream.Name));
                    }
                }
            }

            if (this.ProjectSetup.Teams.Count() == 0)
            {
                this.AddErrorMessage("You must specfiy at least one valid team.");
            }
            else
            {
                if (!ValidationHelper.HasUniqueNames(this.ProjectSetup.Teams.Cast<INamedItem>()))
                {
                    this.AddErrorMessage("Team names must be unique.");
                }

                foreach (var team in this.ProjectSetup.Teams)
                {
                    if (!ValidationHelper.IsValidName(team.Name))
                    {
                        this.AddErrorMessage(
                            string.Format(CultureInfo.InvariantCulture, "Team '{0}' name not valid.", team.Name));
                    }

                    if (!team.HasValidCapacity)
                    {
                        this.AddErrorMessage(
                            string.Format(CultureInfo.InvariantCulture, "Team '{0}' capacity value is not valid.", team.Name));
                    }

                    if (!ValidationHelper.IsValidWorkStream(team.WorkStream))
                    {
                        this.AddErrorMessage(
                            string.Format(CultureInfo.InvariantCulture, "Team '{0}' does not specfiy a work stream.", team.Name));
                    }
                }
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
            Release release;

            if (this.ProjectSetup.Releases == null || this.ProjectSetup.Releases.Count() == 0)
            {
                return;
            }

            if (e.PropertyName.Equals("StartDate"))
            {
                // Update the release object start date.
                release = this.ProjectSetup.Releases.First();
                release.StartDate = this.ProjectSetup.StartDate;
            }

            if (e.PropertyName.Equals("EndDate"))
            {
                // Update the release object end date.
                release = this.ProjectSetup.Releases.Last();
                release.EndDate = this.ProjectSetup.EndDate;
            }
        }

        /// <summary>
        /// Setups the command bindings.
        /// </summary>
        private void SetupCommandBindings()
        {
            CanExecuteRoutedEventHandler canExecute = (s, e) =>
            {
                var parameter = e.Parameter as INamedItem;
                e.CanExecute = parameter != null;
            };
            this.CommandBindings.Add(
                new CommandBinding(
                    LocalCommandLibrary.DeleteNamedItemCommand, this.DeleteNamedItem, canExecute));
        }

        /// <summary>
        /// Deletes the named item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void DeleteNamedItem(object sender, ExecutedRoutedEventArgs e)
        {
            var item = e.Parameter as INamedItem;
            if (item == null)
            {
                return;
            }

            if (item is Release)
            {
                var release = item as Release;
                if (this.ProjectSetup.Releases.Contains(release))
                {
                    this.ProjectSetup.Releases.Remove(release);
                }

                return;
            }

            if (item is WorkStream)
            {
                var workStream = item as WorkStream;
                if (this.ProjectSetup.WorkStreams.Contains(workStream))
                {
                    this.ProjectSetup.WorkStreams.Remove(workStream);
                }

                return;
            }

            if (item is Team)
            {
                var team = item as Team;
                if (this.ProjectSetup.Teams.Contains(team))
                {
                    this.ProjectSetup.Teams.Remove(team);
                }

                return;
            }
        }

        /// <summary>
        /// Handles the Click event of the Execute control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            this.ExecuteSetup();
        }

        /// <summary>
        /// Handles the Click event of the AddRlease control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddRlease_Click(object sender, RoutedEventArgs e)
        {
            this.ProjectSetup = SetupControllerHelper.AddRelease(this.ProjectSetup);
            this.PART_TabControl.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the Click event of the AddWorkStream control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddWorkStream_Click(object sender, RoutedEventArgs e)
        {
            this.ProjectSetup = SetupControllerHelper.AddWorkStream(this.ProjectSetup);
            this.PART_TabControl.SelectedIndex = 1;
        }

        /// <summary>
        /// Handles the Click event of the AddTeam control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddTeam_Click(object sender, RoutedEventArgs e)
        {
            this.ProjectSetup = SetupControllerHelper.AddTeam(this.ProjectSetup);
            this.PART_TabControl.SelectedIndex = 2;
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
