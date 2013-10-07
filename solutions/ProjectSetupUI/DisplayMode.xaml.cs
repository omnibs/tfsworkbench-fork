// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayMode.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for MainControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI
{
    using System;
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Controls;

    using Core.Interfaces;

    using Properties;

    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    [Export(typeof(IDisplayMode))]
    public partial class DisplayMode : IDisplayMode 
    {
        /// <summary>
        /// The project data property.
        /// </summary>
        private static readonly DependencyProperty projectDataProperty = DependencyProperty.Register(
            "ProjectData",
            typeof(IProjectData),
            typeof(DisplayMode));

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayMode"/> class.
        /// </summary>
        public DisplayMode()
        {
            this.InitializeComponent();

            this.MenuControl = new MenuControl { Control = this };
            this.MainController = new MainController(this);
            this.Title = Settings.Default.ModeName;
            this.DisplayPriority = Settings.Default.DisplayPriority;
            this.Description = Properties.Resources.String001;
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
        /// Gets the display priority.
        /// </summary>
        /// <value>The display priority.</value>
        public int DisplayPriority { get; private set; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get
            {
                return this.Visibility == Visibility.Visible;
            }
        }

        /// <summary>
        /// Gets the menu control.
        /// </summary>
        /// <value>The menu control.</value>
        public MenuItem MenuControl
        {
            get; private set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is search provider.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is search provider; otherwise, <c>false</c>.
        /// </value>
        public bool IsHighlightProvider
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the setup controller.
        /// </summary>
        /// <value>The setup controller.</value>
        internal MainController MainController { get; private set; }

        /// <summary>
        /// Shows the specified workbech item.
        /// </summary>
        /// <param name="workbenchItem">The workbech item.</param>
        public void Highlight(IWorkbenchItem workbenchItem)
        {
            throw new NotImplementedException();
        }
    }
}
