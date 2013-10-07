// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainMenuControl.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for MainMenuControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    using Core.Interfaces;

    /// <summary>
    /// Interaction logic for MainMenuControl.xaml
    /// </summary>
    public partial class MainMenuControl
    {
        /// <summary>
        /// The project data property.
        /// </summary>
        private static readonly DependencyProperty projectDataProperty = DependencyProperty.Register(
            "ProjectData",
            typeof(IProjectData),
            typeof(MainMenuControl),
            new PropertyMetadata(null, OnProjectDataChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenuControl"/> class.
        /// </summary>
        public MainMenuControl()
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
        /// Gets or sets the project data.
        /// </summary>
        /// <value>The project data.</value>
        public IProjectData ProjectData
        {
            get { return (IProjectData)this.GetValue(ProjectDataProperty); }
            set { this.SetValue(ProjectDataProperty, value); }
        }

        /// <summary>
        /// Called when [project data changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnProjectDataChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as MainMenuControl;

            if (control == null)
            {
                return;
            }

            var binding = control.PART_ViewDeleteMenu.GetBindingExpression(ItemsControl.ItemsSourceProperty);
            if (binding != null)
            {
                binding.UpdateTarget();
            }

            binding = control.PART_ViewEditMenu.GetBindingExpression(ItemsControl.ItemsSourceProperty);
            if (binding != null)
            {
                binding.UpdateTarget();
            }
        }

        /// <summary>
        /// Call garbage collections.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Gc(object sender, RoutedEventArgs e)
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
        }
    }
}