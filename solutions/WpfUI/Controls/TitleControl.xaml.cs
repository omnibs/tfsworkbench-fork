// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TitleControl.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for TitleControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.Controls
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Core.Interfaces;

    using UIElements;

    /// <summary>
    /// Interaction logic for TitleControl.xaml
    /// </summary>
    public partial class TitleControl
    {
        /// <summary>
        /// The project data property.
        /// </summary>
        private static readonly DependencyProperty projectDataProperty = DependencyProperty.Register(
            "ProjectData",
            typeof(IProjectData),
            typeof(TitleControl),
            new PropertyMetadata(null, OnProjectDataChanged));

        /// <summary>
        /// The display elements collection.
        /// </summary>
        private static readonly DependencyProperty activeDisplayModeProperty =
            DependencyProperty.Register(
                "ActiveDisplayMode",
                typeof(IDisplayMode),
                typeof(TitleControl),
                new PropertyMetadata(null, OnActiveDisplayModeChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleControl"/> class.
        /// </summary>
        public TitleControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        /// <summary>
        /// Gets the project data property.
        /// </summary>
        /// <value>The project data property.</value>
        public static DependencyProperty ProjectDataProperty
        {
            get { return projectDataProperty; }
        }

        /// <summary>
        /// Gets the active display element property.
        /// </summary>
        /// <value>The active display element property.</value>
        public static DependencyProperty ActiveDisplayModeProperty
        {
            get { return activeDisplayModeProperty; }
        }

        /// <summary>
        /// Gets or sets the project data.
        /// </summary>
        /// <value>The project data.</value>
        public IProjectData ProjectData
        {
            get { return (IProjectData)this.GetValue(ProjectDataProperty); } 
            set { this.SetValue(ProjectDataProperty, value); }
        }

        /// <summary>
        /// Gets or sets the active display element.
        /// </summary>
        /// <value>The active display element.</value>
        public IDisplayMode ActiveDisplayMode
        {
            get { return (IDisplayMode)this.GetValue(ActiveDisplayModeProperty); }
            set { this.SetValue(ActiveDisplayModeProperty, value); }
        }

        /// <summary>
        /// Adds the display element.
        /// </summary>
        /// <param name="displayMode">The display element.</param>
        public void AddDisplayMode(IDisplayMode displayMode)
        {
            this.PART_ItemsControl.Items.Add(new DisplayModeTitle { DisplayMode = displayMode });
        }

        /// <summary>
        /// Called when [active display element changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnActiveDisplayModeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as TitleControl;
            if (control == null)
            {
                return;
            }

            foreach (var element in control.PART_ItemsControl.Items.OfType<DisplayModeTitle>())
            {
                element.IsActive = Equals(element.DisplayMode, e.NewValue);
            }
        }

        /// <summary>
        /// Called when [project data changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnProjectDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TitleControl;
            if (control == null)
            {
                return;
            }

            control.OnPropertyChanged(new DependencyPropertyChangedEventArgs(ProjectDataProperty, null, control.ProjectData));
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ItemsControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnItemsControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var displayModeTitle = PART_ItemsControl.SelectedItem as DisplayModeTitle;

            if (displayModeTitle == null || Equals(this.ActiveDisplayMode, displayModeTitle.DisplayMode))
            {
                return;
            }

            CommandLibrary.ShowDisplayModeCommand.Execute(displayModeTitle.DisplayMode, this);
        }
    }
}