// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HierarchyView.cs" company="None">
//   None
// </copyright>
// <summary>
//   The Hierarchy View class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.HierarchyUI.HierarchyObjects
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Core.DataObjects;
    using Core.Interfaces;

    using Properties;

    /// <summary>
    /// The Hierarchy View class.
    /// </summary>
    public class HierarchyView : HierarchyElementBase
    {
        /// <summary>
        /// The parent hierarcht item.
        /// </summary>
        private HierarchyItem parentItem;

        /// <summary>
        /// The child creation parameters instance.
        /// </summary>
        private ChildCreationParameters childCreationParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyView"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public HierarchyView(HierarchyItem parent) : base(parent)
        {
            this.HierarchyItems = new Collection<HierarchyItem>();
            this.parentItem = parent;
        }

        /// <summary>
        /// Gets or sets the view map.
        /// </summary>
        /// <value>The view map.</value>
        public ViewMap ViewMap { get; set; }

        /// <summary>
        /// Gets the view map detail.
        /// </summary>
        /// <value>The view map detail.</value>
        public string ViewMapDetail 
        {
            get
            {
                var viewMapDetail = string.Empty;

                if (this.ViewMap != null)
                {
                    viewMapDetail = this.ViewMap.ParentTypes
                        .Aggregate(
                            string.Empty, 
                            (current, parentType) => 
                                string.Concat(current, string.IsNullOrEmpty(current) ? string.Empty : ", ", parentType));

                    viewMapDetail = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} - {1} - {2}",
                        viewMapDetail,
                        this.ViewMap.LinkName,
                        this.ViewMap.ChildType);
                }

                return viewMapDetail;
            }
        }

        /// <summary>
        /// Gets the hierarchy items.
        /// </summary>
        /// <value>The hierarchy items.</value>
        public ICollection<HierarchyItem> HierarchyItems { get; private set; }

        /// <summary>
        /// Gets the child creation paramters.
        /// </summary>
        /// <value>The child creation parameters.</value>
        public IChildCreationParameters ChildCreationParameters 
        {
            get
            {
                if (this.ViewMap == null || this.parentItem == null)
                {
                    return null;
                }

                return this.childCreationParameters = this.childCreationParameters 
                    ?? new ChildCreationParameters
                        {
                            Parent = this.parentItem.WorkbenchItem,
                            LinkTypeName = this.ViewMap.LinkName,
                            ChildTypeName = this.ViewMap.ChildType
                        };
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
                return this.HierarchyItems;
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
                return Settings.Default.ViewElementSize.Height;
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
                return Settings.Default.ViewElementSize.Width;
            }
        }

        /// <summary>
        /// Releases the resources.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public void ReleaseResources(Canvas canvas)
        {
            foreach (var hierarchyItem in this.HierarchyItems)
            {
                hierarchyItem.ReleaseResources(canvas);
            }

            if (this.childCreationParameters != null)
            {
                this.childCreationParameters.Parent = null;
                this.childCreationParameters = null;
            }

            this.RemoveVisuals(canvas);
            this.ViewMap = null;
            this.HierarchyItems.Clear();
            this.parentItem = null;
        }

        /// <summary>
        /// Gets the visual element.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        /// <returns>The visual element.</returns>
        protected override FrameworkElement CreateVisualElement(Orientation orientation)
        {
            return new HierarchyViewNode
                {
                    HierarchyView = this, 
                    Width = this.ElementWidth, 
                    Height = this.ElementHeight
                };
        }
    }
}