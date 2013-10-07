// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuControl.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for MenuControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI
{
    using System.Windows;

    using Core.Interfaces;

    /// <summary>
    /// Interaction logic for MenuControl.xaml
    /// </summary>
    public partial class MenuControl
    {
        /// <summary>
        /// The project data property.
        /// </summary>
        private static readonly DependencyProperty projectDataProperty = DependencyProperty.Register(
            "ProjectData",
            typeof(IProjectData),
            typeof(MenuControl));

        /// <summary>
        /// The display element property.
        /// </summary>
        private static readonly DependencyProperty displayModeProperty = DependencyProperty.Register(
            "Control",
            typeof(DisplayMode),
            typeof(MenuControl));

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuControl"/> class.
        /// </summary>
        public MenuControl()
        {
            this.InitializeComponent();
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
        /// Gets the display element property.
        /// </summary>
        /// <value>The display element property.</value>
        public static DependencyProperty DisplayModeProperty
        {
            get { return displayModeProperty; }
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
        /// Gets or sets the display element.
        /// </summary>
        /// <value>The display element.</value>
        public DisplayMode Control
        {
            get { return (DisplayMode)this.GetValue(DisplayModeProperty); }
            set { this.SetValue(DisplayModeProperty, value); }
        }
    }
}