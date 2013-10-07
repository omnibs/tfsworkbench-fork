// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkbenchItemContextMenu.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the WorkbenchItemContextMenu type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using Core.DataObjects;
    using Core.Helpers;
    using Core.Interfaces;

    using TfsWorkbench.Core.Services;

    /// <summary>
    /// The work bench item context menu.
    /// </summary>
    public class WorkbenchItemContextMenu : ContextMenu
    {
        /// <summary>
        /// The project data service instance.
        /// </summary>
        private readonly IProjectDataService projectDataService;

        /// <summary>
        /// The dynamic item indicator.
        /// </summary>
        private const string IsDynamicItem = "isDynamicItem";

        /// <summary>
        /// The view in menu text.
        /// </summary>
        private const string HightlighIn = "Highlight In:";

        /// <summary>
        /// The workbench item property.
        /// </summary>
        private static readonly DependencyProperty workbenchItemProperty = DependencyProperty.Register(
            "WorkbenchItem",
            typeof(IWorkbenchItem),
            typeof(WorkbenchItemContextMenu));

        /// <summary>
        /// The has loaded resources flag.
        /// </summary>
        private bool hasLoadedResources;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchItemContextMenu"/> class.
        /// </summary>
        public WorkbenchItemContextMenu()
            : this(ServiceManager.Instance.GetService<IProjectDataService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchItemContextMenu"/> class.
        /// </summary>
        /// <param name="projectDataService">The project data service.</param>
        public WorkbenchItemContextMenu(IProjectDataService projectDataService)
        {
            if (projectDataService == null)
            {
                throw new ArgumentNullException("projectDataService");
            }

            this.projectDataService = projectDataService;
        }

        /// <summary>
        /// Gets the workbench item property.
        /// </summary>
        /// <value>The workbench item property.</value>
        public static DependencyProperty WorkbenchItemProperty
        {
            get { return workbenchItemProperty; }
        }

        /// <summary>
        /// Gets or sets the workbench item.
        /// </summary>
        /// <value>The workbench item.</value>
        public IWorkbenchItem WorkbenchItem
        {
            get { return (IWorkbenchItem)this.GetValue(WorkbenchItemProperty); }
            set { this.SetValue(WorkbenchItemProperty, value); }
        }

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.Items.OfType<MenuItem>().Any(mi => mi.Header.Equals(HightlighIn)))
            {
                return;
            }

            if (!this.hasLoadedResources)
            {
                this.LoadResources();
            }

            var solidColorBrush = (SolidColorBrush)this.Resources["ButtonNormalStroke"];
            var controlTemplate = (ControlTemplate)this.Resources["FindIconTemplate"];
            var style = (Style)this.Resources["MenuIconStyle"];

            var viewInMenuItem = new MenuItem
                {
                    Header = HightlighIn,
                    Icon =
                        new ContentControl
                            {
                                Foreground = solidColorBrush,
                                Template = controlTemplate,
                                Style = style
                            }
                };

            this.Items.Add(viewInMenuItem);
        }

        /// <summary>
        /// Called when the <see cref="E:System.Windows.Controls.ContextMenu.Opened"/> event occurs.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.Controls.ContextMenu.Opened"/> event.</param>
        protected override void OnOpened(RoutedEventArgs e)
        {
            // Clear the existing dynamic elements.
            RemoveDynamicItems(this);

            if (this.projectDataService.CurrentProjectData == null || this.WorkbenchItem == null)
            {
                return;
            }

            this.AddDynamicItems();

            base.OnOpened(e);
        }

        /// <summary>
        /// Removes the dynamic items.
        /// </summary>
        /// <param name="parentItemsControl">The parent items control.</param>
        private static void RemoveDynamicItems(ItemsControl parentItemsControl)
        {
            foreach (var item in parentItemsControl.Items.OfType<FrameworkElement>().ToArray())
            {
                var itemsControl = item as ItemsControl;

                if (itemsControl != null)
                {
                    RemoveDynamicItems(itemsControl);
                }

                if (Equals(item.Tag, IsDynamicItem))
                {
                    parentItemsControl.Items.Remove(item);
                }
            }
        }

        /// <summary>
        /// Adds the dynamic items.
        /// </summary>
        private void AddDynamicItems()
        {
            var viewInMenuItem = this.Items.OfType<MenuItem>().FirstOrDefault(mi => mi.Header.Equals(HightlighIn));

            if (viewInMenuItem != null)
            {
                foreach (var searchProvider in this.projectDataService.HighlightProviders)
                {
                    var searchMenuItem = new MenuItem
                        {
                            Header = searchProvider.Title,
                            Command = CommandLibrary.ShowItemInCommand,
                            CommandTarget = this.PlacementTarget,
                            CommandParameter =
                                new Tuple<IWorkbenchItem, IHighlightProvider>(this.WorkbenchItem, searchProvider),
                            IsCheckable = true,
                            IsChecked = searchProvider.IsActive,
                            Tag = IsDynamicItem
                        };

                    viewInMenuItem.Items.Add(searchMenuItem);
                }
            }

            // Find any context menu field names
            ItemTypeData itemTypeData;
            if (!this.projectDataService.CurrentProjectData.ItemTypes.TryGetValue(this.WorkbenchItem.GetTypeName(), out itemTypeData)
                || !itemTypeData.ContextFields.Any())
            {
                return;
            }

            this.Items.Add(new Separator { Tag = IsDynamicItem });

            foreach (var contextField in itemTypeData.ContextFields)
            {
                var displayName = this.WorkbenchItem.DisplayNames[contextField];

                if (displayName == null)
                {
                    continue;
                }

                var menuIcon = new ContentControl
                {
                    Foreground = (SolidColorBrush)this.Resources["ButtonNormalStroke"],
                    Template = (ControlTemplate)this.Resources["EditIconTemplate"],
                    Style = (Style)this.Resources["MenuIconStyle"]
                };

                var menuItem = new MenuItem
                    {
                        Header = displayName,
                        Icon = menuIcon,
                        Tag = IsDynamicItem
                    };

                this.RenderSubMenuItems(menuItem, contextField);

                this.Items.Add(menuItem);
            }
        }

        /// <summary>
        /// Renders the sub menu items.
        /// </summary>
        /// <param name="parentMenu">The parent menu.</param>
        /// <param name="contextField">The context field.</param>
        private void RenderSubMenuItems(ItemsControl parentMenu, string contextField)
        {
            RoutedEventHandler childMenuItemOnClick = (s, e) =>
            {
                var menuItem = s as MenuItem;
                if (menuItem == null || this.WorkbenchItem == null)
                {
                    return;
                }

                var field = menuItem.Tag as string;
                if (field == null)
                {
                    return;
                }

                this.WorkbenchItem[field] = menuItem.Header;
            };

            parentMenu.Items.Clear();

            var currentValue = this.WorkbenchItem[contextField];
            var currentValueAsString = currentValue == null ? string.Empty : currentValue.ToString();
            foreach (var childMenuItem in
                from allowedValue in (IEnumerable<object>)this.WorkbenchItem.AllowedValues[contextField]
                let isEqualToCurrentValue = Equals(currentValueAsString, allowedValue)
                select new MenuItem
                    {
                        Header = allowedValue, 
                        Tag = contextField, 
                        IsCheckable = true, 
                        IsChecked = isEqualToCurrentValue
                    })
            {
                childMenuItem.Click += childMenuItemOnClick;

                parentMenu.Items.Add(childMenuItem);
            }
        }

        /// <summary>
        /// Builds the menu icon if required.
        /// </summary>
        private void LoadResources()
        {
            this.hasLoadedResources = true;

            var resourceDictionary = new ResourceDictionary
                {
                    Source =
                        new Uri(
                        "/TfsWorkbench.UIElements;Component/Resources/CommandButtonStyles.xaml",
                        UriKind.Relative)
                };

            this.Resources.MergedDictionaries.Add(resourceDictionary);
        }
    }
}
