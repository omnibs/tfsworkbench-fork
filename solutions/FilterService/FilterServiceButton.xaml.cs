// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterServiceButton.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for FilterServiceButton.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.FilterService
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for FilterServiceButton.xaml
    /// </summary>
    public partial class FilterServiceButton
    {

        /// <summary>
        /// The controller dependency property.
        /// </summary>
        private static readonly DependencyProperty controllerProperty = DependencyProperty.Register(
            "Controller",
            typeof(IFilterServiceController),
            typeof(FilterServiceButton));

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterServiceButton"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public FilterServiceButton(IFilterServiceController controller)
        {
            InitializeComponent();
            this.Controller = controller;
        }

        /// <summary>
        /// Gets the Controller property.
        /// </summary>
        /// <value>The name property.</value>
        public static DependencyProperty ControllerProperty
        {
            get { return controllerProperty; }
        }

        /// <summary>
        /// Gets or sets the instance Controller.
        /// </summary>
        /// <returns>The instance Controller.</returns>
        public IFilterServiceController Controller
        {
            get { return (IFilterServiceController)this.GetValue(ControllerProperty); }
            set { this.SetValue(ControllerProperty, value); }
        }
    }
}
