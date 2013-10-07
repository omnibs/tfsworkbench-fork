// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayMode.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for DisplayMode.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TfsWorkbench.ItemListUI
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    using Core.Helpers;
    using Core.Interfaces;

    using Properties;

    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Services;

    using UIElements;

    /// <summary>
    /// Interaction logic for DisplayMode.xaml
    /// </summary>
    [Export(typeof(IDisplayMode))]
    public partial class DisplayMode : IDisplayMode
    {
        /// <summary>
        /// The project data service.
        /// </summary>
        private readonly IProjectDataService projectDataService;

        /// <summary>
        /// The item lists.
        /// </summary>
        private readonly ObservableCollection<TabItem> tabItems = new ObservableCollection<TabItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayMode"/> class.
        /// </summary>
        public DisplayMode()
            : this(ServiceManager.Instance.GetService<IProjectDataService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayMode"/> class.
        /// </summary>
        /// <param name="projectDataService">The project data service.</param>
        public DisplayMode(IProjectDataService projectDataService)
        {
            if (projectDataService == null)
            {
                throw new ArgumentNullException("projectDataService");
            }

            InitializeComponent();

            this.MenuControl = new MenuControl { DisplayMode = this };
            this.DisplayPriority = Settings.Default.DisplayPriority;
            this.Title = Settings.Default.ModeName;
            this.projectDataService = projectDataService;
            this.Description = Settings.Default.ModeDescription;

            this.projectDataService.ProjectDataChanged += this.OnProjectDataChanged;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is search provider.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is search provider; otherwise, <c>false</c>.
        /// </value>
        public bool IsHighlightProvider
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the tab items.
        /// </summary>
        /// <value>The tab items.</value>
        public ObservableCollection<TabItem> TabItems
        {
            get { return this.tabItems; }
        }

        /// <summary>
        /// Gets the display priority.
        /// </summary>
        /// <value>The display priority.</value>
        public int DisplayPriority { get; private set; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get
            {
                return this.Visibility == Visibility.Visible;
            }
        }

        /// <summary>
        /// Gets the menu control.
        /// </summary>
        /// <value>The menu control.</value>
        public MenuItem MenuControl { get; private set; }

        /// <summary>
        /// Shows the specified workbech item.
        /// </summary>
        /// <param name="workbenchItem">The workbech item.</param>
        public void Highlight(IWorkbenchItem workbenchItem)
        {
            // Bring this mode to top
            CommandLibrary.ShowDisplayModeCommand.Execute(this, this);

            var itemType = workbenchItem.GetTypeName();

            // Find the relevant tab
            var tab = this.TabItems.FirstOrDefault(t => t.Header.Equals(itemType));

            if (tab == null)
            {
                return;
            }

            // Select the tab.
            this.PART_TabControl.SelectedItem = tab;

            var itemList = tab.Content as ItemList;

            if (itemList == null)
            {
                return;
            }

            var itemListView = itemList.PART_MainListView;

            var callback = (SendOrPostCallback)delegate
                {
                    var listItems = itemListView.Items.OfType<IControlItemGroup>()
                        .Where(ic => Equals(ic.WorkbenchItem, workbenchItem));

                    if (!listItems.Any())
                    {
                        return;
                    }

                    foreach (var itemCollection in listItems)
                    {
                        itemListView.ScrollIntoView(itemCollection);

                        var workbenchItemUi =
                            itemListView.ItemContainerGenerator.ContainerFromItem(itemCollection) as FrameworkElement;

                        if (workbenchItemUi == null)
                        {
                            continue;
                        }

                        workbenchItemUi.BringIntoView();
                        HighlightHelper.Highlight(workbenchItemUi);
                    }
                };

            this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, callback, null);
        }

        /// <summary>
        /// Called when [project data changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ProjectDataChangedEventArgs"/> instance containing the event data.</param>
        private void OnProjectDataChanged(object sender, ProjectDataChangedEventArgs e)
        {
            this.ReleaseResources();
            this.BuildListViews();
        }

        /// <summary>
        /// Releases the resources.
        /// </summary>
        private void ReleaseResources()
        {
            foreach (var itemList in this.tabItems.Select(t => t.Content).OfType<ItemList>())
            {
                itemList.Dispose();
            }

            foreach (var tabItem in this.tabItems)
            {
                tabItem.Content = null;
            }

            this.tabItems.Clear();
            this.PART_TabControl.RenderedTabs.Clear();
        }

        /// <summary>
        /// Builds the list views.
        /// </summary>
        private void BuildListViews()
        {
            var projectData = this.projectDataService.CurrentProjectData;

            if (projectData == null || projectData.ItemTypes == null)
            {
                return;
            }

            foreach (var itemType in projectData.ItemTypes.OrderBy(it => it.TypeName))
            {
                var itemList = new ItemList
                    {
                        WorkbenchItemTypeName = itemType.TypeName,
                        DataProvider = this.projectDataService.CurrentDataProvider,
                        ProjectData = projectData
                    };

                this.tabItems.Add(new TabItem { Header = itemType.TypeName, Content = itemList });
            }
        }
    }
}
