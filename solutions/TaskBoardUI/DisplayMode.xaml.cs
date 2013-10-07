// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayMode.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for ViewTabsControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.TaskBoardUI
{
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Controls;

    using TfsWorkbench.Core.Interfaces;

    /// <summary>
    /// Interaction logic for ViewTabsControl.xaml
    /// </summary>
    [Export(typeof(IDisplayMode))]
    public partial class DisplayMode : IDisplayMode
    {
        /// <summary>
        /// The display mode controller.
        /// </summary>
        private readonly DisplayModeController controller;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayMode"/> class.
        /// </summary>
        public DisplayMode()
        {
            this.InitializeComponent();

            this.controller = new DisplayModeController(this);
        }

        /// <summary>
        /// Gets the display priority.
        /// </summary>
        /// <value>The display priority.</value>
        public int DisplayPriority { get; internal set; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; internal set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; internal set; }

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
        public MenuItem MenuControl { get; internal set; }

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
                return true;
            }
        }

        /// <summary>
        /// Shows the specified workbech item.
        /// </summary>
        /// <param name="workbenchItem">The workbech item.</param>
        public void Highlight(IWorkbenchItem workbenchItem)
        {
            this.controller.Highlight(workbenchItem);
        }
    }
}