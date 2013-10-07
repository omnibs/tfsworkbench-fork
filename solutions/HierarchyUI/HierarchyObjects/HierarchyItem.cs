// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HierarchyItem.cs" company="None">
//   None
// </copyright>
// <summary>
//   The hierarchy item class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.HierarchyUI.HierarchyObjects
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    using Core.DataObjects;
    using Core.Interfaces;

    using Properties;

    /// <summary>
    /// The hierarchy item class.
    /// </summary>
    public class HierarchyItem : HierarchyElementBase
    {
        /// <summary>
        /// The hiearchy view parent.
        /// </summary>
        private readonly HierarchyView parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyItem"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public HierarchyItem(HierarchyView parent) : base(parent)
        {
            this.parent = parent;
            this.HierarchyViews = new Collection<HierarchyView>();
        }

        /// <summary>
        /// Gets or sets the project data.
        /// </summary>
        /// <value>The project data.</value>
        public IProjectData ProjectData { get; set; }

        /// <summary>
        /// Gets or sets the workbench item.
        /// </summary>
        /// <value>The workbench item.</value>
        public IWorkbenchItem WorkbenchItem { get; set; }

        /// <summary>
        /// Gets the hierarchy views.
        /// </summary>
        /// <value>The hierarchy views.</value>
        public ICollection<HierarchyView> HierarchyViews { get; private set; }

        /// <summary>
        /// Gets the parent view map.
        /// </summary>
        /// <value>The parent view map.</value>
        public ViewMap ParentViewMap
        {
            get
            {
                return this.parent == null ? null : this.parent.ViewMap;
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public override IEnumerable<HierarchyElementBase> Children 
        {
            get
            {
                return this.HierarchyViews;
            }
        }

        /// <summary>
        /// Gets the height of the element.
        /// </summary>
        /// <value>The height of the element.</value>
        protected override double ElementHeight
        {
            get
            {
                return Settings.Default.WorkItemElementSize.Height;
            }
        }

        /// <summary>
        /// Gets the width of the element.
        /// </summary>
        /// <value>The width of the element.</value>
        protected override double ElementWidth
        {
            get
            {
                return Settings.Default.WorkItemElementSize.Width;
            }
        }

        /// <summary>
        /// Releases the resources.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public void ReleaseResources(Canvas canvas)
        {
            foreach (var hierarchyView in this.HierarchyViews)
            {
                hierarchyView.ReleaseResources(canvas);
            }

            this.RemoveVisuals(canvas);
            this.WorkbenchItem = null;
            this.HierarchyViews.Clear();
            this.ProjectData = null;
        }

        /// <summary>
        /// Gets the visual element.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        /// <returns>The visual element.</returns>
        protected override FrameworkElement CreateVisualElement(Orientation orientation)
        {
            return new HierarchyItemNode
                {
                    HierarchyItem = this, 
                    Width = this.ElementWidth, 
                    Height = this.ElementHeight
                };
        }
    }
}