// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditItemControl.xaml.cs" company="EMC Consulting">
//   EMC Consulting 2009
// </copyright>
// <summary>
//   Interaction logic for EditItemControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Emcc.TeamSystem.TaskBoard.UIElements
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Core.DataObjects;

    /// <summary>
    /// Interaction logic for EditItemControl.xaml
    /// </summary>
    public partial class EditItemControl : UserControl
    {
        public static DependencyProperty ControlItemCollectionProperty = DependencyProperty.Register(
            "ControlItemCollection", typeof(ControlItemCollection), typeof(EditItemControl));

        public static DependencyProperty ProjectDataProperty = DependencyProperty.Register(
            "ProjectData", typeof(ProjectData), typeof(EditItemControl));

        /// <summary>
        /// Initializes a new instance of the <see cref="EditItemControl"/> class.
        /// </summary>
        public EditItemControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        /// <summary>
        /// Gets or sets the control items.
        /// </summary>
        /// <value>The control items.</value>
        public ControlItemCollection ControlItemCollection
        {
            get { return (ControlItemCollection)this.GetValue(ControlItemCollectionProperty); }
            set { this.SetValue(ControlItemCollectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the control items.
        /// </summary>
        /// <value>The control items.</value>
        public ProjectData ProjectData
        {
            get { return (ProjectData)this.GetValue(ProjectDataProperty); }
            set { this.SetValue(ProjectDataProperty, value); }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.SizeChanged"/> event, using the specified information as part of the eventual event data.
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            var grid = this.EditGridView;

            var avilableWidth = this.ActualWidth - 60;

            grid.Columns[1].Width = avilableWidth * 0.6;
            grid.Columns[0].Width = avilableWidth * 0.4;
        }

        /// <summary>
        /// Handles the Click event of the CloseButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var validationErrors =
                this.ControlItemCollection.ControlItems[0].TaskBoardItem.ValueProvider.ValidationErrors;

            var message = string.Empty;

            if (validationErrors != null)
            {
                message = string.Concat(validationErrors.Select(v => string.Concat(v, Environment.NewLine)).ToArray());
            }

            this.ValidationErrors.Text = message;

            if (string.IsNullOrEmpty(message))
            {
                CommandLibrary.CloseEditPanel.Execute(null, this.CloseButton);
            }
        }

        /// <summary>
        /// Handles the Click event of the DiscardButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void DiscardButton_Click(object sender, RoutedEventArgs e)
        {
            this.ProjectData.Discard(this.ControlItemCollection.ControlItems[0].TaskBoardItem);

            CommandLibrary.CloseEditPanel.Execute(null, this.DiscardButton);
        }
    }
}