// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HierarchyViewNode.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for WorkbenchItemNode.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.HierarchyUI.HierarchyObjects
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for WorkbenchItemNode.xaml
    /// </summary>
    public partial class HierarchyViewNode
    {
        /// <summary>
        /// The hierarchy item property.
        /// </summary>
        private static readonly DependencyProperty hierarchyViewProperty = DependencyProperty.Register(
            "HierarchyView",
            typeof(HierarchyView),
            typeof(HierarchyViewNode));

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyViewNode"/> class.
        /// </summary>
        public HierarchyViewNode()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the hierarchy view property.
        /// </summary>
        /// <value>The hierarchy view property.</value>
        public static DependencyProperty HierarchyViewProperty
        {
            get { return hierarchyViewProperty; }
        }

        /// <summary>
        /// Gets or sets the hierarchy view.
        /// </summary>
        /// <value>The hierarchy view.</value>
        internal HierarchyView HierarchyView
        {
            get { return (HierarchyView)this.GetValue(HierarchyViewProperty); }
            set { this.SetValue(HierarchyViewProperty, value); }
        }
    }
}
