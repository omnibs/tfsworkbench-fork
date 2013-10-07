// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HierarchyViewContextMenu.cs" company="None">
//   None
// </copyright>
// <summary>
//   The hierarchy view context mneu class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.HierarchyUI.HierarchyObjects
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using Core.Helpers;
    using Core.Interfaces;

    using TfsWorkbench.Core.Services;

    /// <summary>
    /// The hierarchy view context mneu class.
    /// </summary>
    public class HierarchyViewContextMenu : ContextMenu
    {
        /// <summary>
        /// The project data service instance.
        /// </summary>
        private readonly IProjectDataService projectDataService;

        /// <summary>
        /// The hierarchy view Property.
        /// </summary>
        private static readonly DependencyProperty hierarchyViewProperty = DependencyProperty.Register(
            "HierarchyView",
            typeof(HierarchyView),
            typeof(HierarchyViewContextMenu));

        /// <summary>
        /// The remove child menu.
        /// </summary>
        private MenuItem removeChildMenu;

        /// <summary>
        /// The orphans menu.
        /// </summary>
        private MenuItem orphansMenu;

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyViewContextMenu"/> class.
        /// </summary>
        public HierarchyViewContextMenu()
            : this(ServiceManager.Instance.GetService<IProjectDataService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyViewContextMenu"/> class.
        /// </summary>
        /// <param name="projectDataService">The project data service.</param>
        public HierarchyViewContextMenu(IProjectDataService projectDataService)
        {
            if (projectDataService == null)
            {
                throw new ArgumentNullException("projectDataService");
            }

            this.projectDataService = projectDataService;
        }

        /// <summary>
        /// Gets the view map property.
        /// </summary>
        /// <value>The view map property.</value>
        public static DependencyProperty HierarchyViewProperty
        {
            get { return hierarchyViewProperty; }
        }

        /// <summary>
        /// Gets the project data.
        /// </summary>
        /// <value>The project data.</value>
        public IProjectData ProjectData
        {
            get
            {
                return this.projectDataService.CurrentProjectData;
            }
        }

        /// <summary>
        /// Gets or sets the hierarchy view.
        /// </summary>
        /// <value>The hierarchy view.</value>
        public HierarchyView HierarchyView
        {
            get { return (HierarchyView)this.GetValue(HierarchyViewProperty); }
            set { this.SetValue(HierarchyViewProperty, value); }
        }

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.orphansMenu != null)
            {
                return;
            }

            var resourceDictionary = new ResourceDictionary
            {
                Source =
                    new Uri(
                    "/TfsWorkbench.UIElements;Component/Resources/CommandButtonStyles.xaml",
                    UriKind.Relative)
            };

            var orpahnMenuIcon = new ContentControl
                {
                    Foreground = (SolidColorBrush)resourceDictionary["ButtonNormalStroke"],
                    Template = (ControlTemplate)resourceDictionary["AddIconTemplate"],
                    Style = (Style)resourceDictionary["MenuIconStyle"]
                };

            this.orphansMenu = new MenuItem
                {
                    Header = "Add Orphan Item",
                    Icon = orpahnMenuIcon
                };

            this.Items.Add(this.orphansMenu);

            var removeChildMenuIcon = new ContentControl
            {
                Foreground = (SolidColorBrush)resourceDictionary["ButtonNormalStroke"],
                Template = (ControlTemplate)resourceDictionary["DeleteIconTemplate"],
                Style = (Style)resourceDictionary["MenuIconStyle"]
            };

            this.removeChildMenu = new MenuItem
            {
                Header = "Remove Child Link",
                Icon = removeChildMenuIcon
            };

            this.Items.Add(this.removeChildMenu);
        }

        /// <summary>
        /// Called when the <see cref="E:System.Windows.Controls.ContextMenu.Opened"/> event occurs.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.Controls.ContextMenu.Opened"/> event.</param>
        protected override void OnOpened(RoutedEventArgs e)
        {
            if (this.ProjectData == null || this.HierarchyView == null || this.orphansMenu == null)
            {
                return;
            }

            this.orphansMenu.Items.Clear();
            this.removeChildMenu.Items.Clear();

            var childType = this.HierarchyView.ViewMap.ChildType;
            var parent = this.HierarchyView.ChildCreationParameters.Parent;

            Func<IWorkbenchItem, bool> isOrphan =
                w => !Equals(w, parent)
                        && !w.ParentLinks.Any(this.HierarchyView.ViewMap.IsViewLink);

            Func<IWorkbenchItem, bool> hasViewParent =
                w => w.ParentLinks.Any(l => this.HierarchyView.ViewMap.IsViewLink(l) && Equals(l.Parent, parent));

            var viewChildren = this.ProjectData.WorkbenchItems.Where(w => w.GetTypeName().Equals(childType)).ToArray();

            var hasOrphans = viewChildren.Any(isOrphan);
            var hasChildren = viewChildren.Any(hasViewParent);

            if (!hasOrphans)
            {
                this.orphansMenu.IsEnabled = false;
            }
            else
            {
                foreach (var orphanMenu in viewChildren.Where(isOrphan).OrderBy(w => w.GetId()).Select(this.CreateOrphanMenuItem))
                {
                    this.orphansMenu.Items.Add(orphanMenu);
                }
            }

            if (!hasChildren)
            {
                this.removeChildMenu.IsEnabled = false;
            }
            else
            {
                foreach (var childMenu in viewChildren.Where(hasViewParent).OrderBy(w => w.GetId()).Select(this.CreateRemoveChildMenuItem))
                {
                    this.removeChildMenu.Items.Add(childMenu);
                }
            }
        }

        /// <summary>
        /// Creates the orphan menu item.
        /// </summary>
        /// <param name="orphan">The orphan.</param>
        /// <returns>A menu item based on the specified orphan.</returns>
        private MenuItem CreateOrphanMenuItem(IWorkbenchItem orphan)
        {
            var header = string.Format(
                CultureInfo.InvariantCulture,
                "({0}) {1} - ({2})",
                orphan.GetId(), 
                orphan.GetCaption(),
                orphan.GetState());

            var menuItem = new MenuItem
                {
                    Header = header,
                    Command = LocalCommandLibrary.LinkItemCommand,
                    CommandParameter = new Tuple<HierarchyView, IWorkbenchItem>(this.HierarchyView, orphan),
                    CommandTarget = this.PlacementTarget
                };

            return menuItem;
        }

        /// <summary>
        /// Creates the remove child menu item.
        /// </summary>
        /// <param name="child">The child.</param>
        /// <returns>A mewnu item to remove the child.</returns>
        private MenuItem CreateRemoveChildMenuItem(IWorkbenchItem child)
        {
            var header = string.Format(
                CultureInfo.InvariantCulture,
                "({0}) {1} - ({2})",
                child.GetId(),
                child.GetCaption(),
                child.GetState());

            var menuItem = new MenuItem
            {
                Header = header,
                Command = LocalCommandLibrary.UnlinkItemCommand,
                CommandParameter = new Tuple<HierarchyView, IWorkbenchItem>(this.HierarchyView, child),
                CommandTarget = this.PlacementTarget
            };

            return menuItem;
        }
    }
}
