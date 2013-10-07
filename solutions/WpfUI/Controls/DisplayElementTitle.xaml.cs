// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayElementTitle.xaml.cs" company="EMC Consulting">
//   EMC Consulting 2010
// </copyright>
// <summary>
//   Interaction logic for DisplayElementTitle.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Emcc.ScrumMastersWorkbench.WpfUI.Controls
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;

    using UIElements;

    /// <summary>
    /// Interaction logic for DisplayElementTitle.xaml
    /// </summary>
    public partial class DisplayElementTitle : INotifyPropertyChanged
    {
        /// <summary>
        /// The display element property.
        /// </summary>
        private static readonly DependencyProperty displayElementProperty = DependencyProperty.Register(
            "DisplayElement",
            typeof(IDisplayElement),
            typeof(DisplayElementTitle));

        /// <summary>
        /// The is active flag.
        /// </summary>
        private bool isActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayElementTitle"/> class.
        /// </summary>
        public DisplayElementTitle()
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
        public static DependencyProperty DisplayElementProperty
        {
            get { return displayElementProperty; }
        }

        /// <summary>
        /// Gets or sets the display element.
        /// </summary>
        /// <value>The display element.</value>
        public IDisplayElement DisplayElement
        {
            get { return (IDisplayElement)this.GetValue(DisplayElementProperty); }
            set { this.SetValue(DisplayElementProperty, value); }
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
            CommandLibrary.ShowDisplayElement.Execute(this.DisplayElement, this);
            e.Handled = true;
        }
    }
}
