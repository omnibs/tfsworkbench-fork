// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuControl.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for MenuControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ItemListUI
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for MenuControl.xaml
    /// </summary>
    public partial class MenuControl
    {
        /// <summary>
        /// The display element property.
        /// </summary>
        private static readonly DependencyProperty displayModeProperty = DependencyProperty.Register(
            "DisplayMode",
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
        /// Gets the display element property.
        /// </summary>
        /// <value>The display element property.</value>
        public static DependencyProperty DisplayModeProperty
        {
            get { return displayModeProperty; }
        }

        /// <summary>
        /// Gets or sets the display element.
        /// </summary>
        /// <value>The display element.</value>
        public DisplayMode DisplayMode
        {
            get { return (DisplayMode)this.GetValue(DisplayModeProperty); }

            set { this.SetValue(DisplayModeProperty, value); }
        }
    }
}