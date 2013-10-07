// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterServiceMenuItem.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for FilterServiceMenuItem.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.FilterService
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for FilterServiceMenuItem.xaml
    /// </summary>
    public partial class FilterServiceMenuItem
    {
        /// <summary>
        /// The controller dependency property.
        /// </summary>
        private static readonly DependencyProperty controllerProperty = DependencyProperty.Register(
            "Controller",
            typeof(IFilterServiceController),
            typeof(FilterServiceMenuItem));

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterServiceMenuItem"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public FilterServiceMenuItem(IFilterServiceController controller)
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
