// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuControl.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for MenuControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.TaskBoardUI
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Core.Interfaces;

    using Properties;

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
            "DisplayMode",
            typeof(IDisplayMode),
            typeof(MenuControl));

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuControl"/> class.
        /// </summary>
        public MenuControl()
        {
            InitializeComponent();

            this.SetSizeMenuCheckedState();
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
        public IDisplayMode DisplayMode
        {
            get { return (IDisplayMode)this.GetValue(DisplayModeProperty); }
            set { this.SetValue(DisplayModeProperty, value); }
        }

        /// <summary>
        /// Changes the size of the task card.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ChangeTaskCardSize(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;

            if (menuItem == null)
            {
                return;
            }

            if (menuItem == this.PART_ExtraLarge)
            {
                Settings.Default.CurrentCardSize = Settings.Default.ExtraLargeCardSize;
            }

            if (menuItem == this.PART_Large)
            {
                Settings.Default.CurrentCardSize = Settings.Default.LargeCardSize;
            }

            if (menuItem == this.PART_Medium)
            {
                Settings.Default.CurrentCardSize = Settings.Default.MediumCardSize;
            }

            if (menuItem == this.PART_Small)
            {
                Settings.Default.CurrentCardSize = Settings.Default.SmallCardSize;
            }

            Settings.Default.Save();

            this.SetSizeMenuCheckedState();
        }

        private void SetSizeMenuCheckedState()
        {
            var selectedMenuItem = default(MenuItem);

            if (Settings.Default.CurrentCardSize == Settings.Default.ExtraLargeCardSize)
            {
                selectedMenuItem = this.PART_ExtraLarge;
            }

            if (Settings.Default.CurrentCardSize == Settings.Default.LargeCardSize)
            {
                selectedMenuItem = this.PART_Large;
            }

            if (Settings.Default.CurrentCardSize == Settings.Default.MediumCardSize)
            {
                selectedMenuItem = this.PART_Medium;
            }

            if (Settings.Default.CurrentCardSize == Settings.Default.SmallCardSize)
            {
                selectedMenuItem = this.PART_Small;
            }

            foreach (var menuItem in this.PART_CardSizeSelector.Items.OfType<MenuItem>())
            {
                menuItem.IsChecked = menuItem == selectedMenuItem;
            }
        }
    }
}
