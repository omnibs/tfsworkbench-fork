// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectSelectorView.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for ProjectLoadControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.ProjectSelector
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;

    using TfsWorkbench.UIElements.PopupControls;

    /// <summary>
    /// Interaction logic for ProjectLoadControl.xaml
    /// </summary>
    public partial class ProjectSelectorView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectSelectorView"/> class.
        /// </summary>
        public ProjectSelectorView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Called when [path selector got keyboard focus].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyboardFocusChangedEventArgs"/> instance containing the event data.</param>
        private void OnPathSelectorGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.EnsureProjectNodesLoaded(sender);
        }

        /// <summary>
        /// Called when [path selector mouse left button down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnPathSelectorMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.EnsureProjectNodesLoaded(sender);
        }

        /// <summary>
        /// Ensures the project nodes loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void EnsureProjectNodesLoaded(object sender)
        {
            var viewModel = this.DataContext as IProjectSelectorViewModel;

            if (viewModel == null)
            {
                return;
            }

            viewModel.EnsureProjectNodesLoadedCommand.Execute(sender);
        }

        /// <summary>
        /// Sets the opacity.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SetFullOpacity(object sender, EventArgs e)
        {
            this.PART_StatusMessageBackground.BeginAnimation(OpacityProperty, null);
            this.PART_StatusMessageBackground.Opacity = 1;
        }

        /// <summary>
        /// Removes the opacity.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RemoveOpacity(object sender, EventArgs e)
        {
            this.PART_StatusMessageBackground.BeginAnimation(OpacityProperty, null);
            this.PART_StatusMessageBackground.Opacity = 0;
        }
    }
}