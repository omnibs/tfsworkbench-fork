// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HierarchyItemNode.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for WorkbenchItemNode.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.HierarchyUI.HierarchyObjects
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using UIElements;

    /// <summary>
    /// Interaction logic for WorkbenchItemNode.xaml
    /// </summary>
    public partial class HierarchyItemNode : UserControl 
    {
        /// <summary>
        /// The hierarchy item property.
        /// </summary>
        private static readonly DependencyProperty hierarchyItemProperty = DependencyProperty.Register(
            "HierarchItem",
            typeof(HierarchyItem),
            typeof(HierarchyItemNode));

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyItemNode"/> class.
        /// </summary>
        public HierarchyItemNode()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the hierarchy property.
        /// </summary>
        /// <value>The hierarchy property.</value>
        public static DependencyProperty HierarchyItemProperty
        {
            get { return hierarchyItemProperty; }
        }

        /// <summary>
        /// Gets or sets the hierarchy item.
        /// </summary>
        /// <value>The hierarchy item.</value>
        internal HierarchyItem HierarchyItem
        {
            get { return (HierarchyItem)this.GetValue(HierarchyItemProperty); }
            set { this.SetValue(HierarchyItemProperty, value); }
        }

        /// <summary>
        /// Releases the resources.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public void ReleaseResources(Canvas canvas)
        {
            if (this.HierarchyItem == null)
            {
                return;
            }

            this.HierarchyItem.ReleaseResources(canvas);

            foreach (var hierarchyView in this.HierarchyItem.HierarchyViews)
            {
                hierarchyView.ReleaseResources(canvas);
            }

            this.HierarchyItem = null;
        }

        /// <summary>
        /// Handles the OnMouseDoubleClick event of the HierarchyItemNode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void HierarchyItemNode_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.HierarchyItem == null || this.HierarchyItem.WorkbenchItem == null)
            {
                return;
            }

            CommandLibrary.EditItemCommand.Execute(this.HierarchyItem.WorkbenchItem, this);
        }
    }
}
