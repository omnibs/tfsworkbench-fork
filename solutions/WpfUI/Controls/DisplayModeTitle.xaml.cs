// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayModeTitle.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for DisplayModeTitle.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;

    using TfsWorkbench.Core.Interfaces;

    using UIElements;

    /// <summary>
    /// Interaction logic for DisplayModeTitle.xaml
    /// </summary>
    public partial class DisplayModeTitle : INotifyPropertyChanged
    {
        /// <summary>
        /// The display element property.
        /// </summary>
        private static readonly DependencyProperty displayModeProperty = DependencyProperty.Register(
            "DisplayMode",
            typeof(IDisplayMode),
            typeof(DisplayModeTitle));

        /// <summary>
        /// The is active flag.
        /// </summary>
        private bool isActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayModeTitle"/> class.
        /// </summary>
        public DisplayModeTitle()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
        public IDisplayMode DisplayMode
        {
            get { return (IDisplayMode)this.GetValue(DisplayModeProperty); }
            set { this.SetValue(DisplayModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get
            {
                return this.isActive;
            }

            set
            {
                if (this.isActive == value)
                {
                    return;
                }

                this.isActive = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("IsActive"));
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the Grid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CommandLibrary.ShowDisplayModeCommand.Execute(this.DisplayMode, this);
            e.Handled = true;
        }
    }
}
