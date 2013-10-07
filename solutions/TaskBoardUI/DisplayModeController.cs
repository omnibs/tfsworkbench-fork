// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayModeController.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the DisplayModeController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.TaskBoardUI
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;

    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.TaskBoardUI.DataObjects;
    using TfsWorkbench.TaskBoardUI.Helpers;
    using TfsWorkbench.TaskBoardUI.Properties;
    using TfsWorkbench.UIElements;

    /// <summary>
    /// The display mode contrtoller.
    /// </summary>
    internal class DisplayModeController
    {
        /// <summary>
        /// the display mode.
        /// </summary>
        private readonly DisplayMode displayMode;

        /// <summary>
        /// The swimlane service.
        /// </summary>
        private readonly ISwimLaneService swimLaneService;

        /// <summary>
        /// The project data service.
        /// </summary>
        private readonly IProjectDataService projectDataService;

        /// <summary>
        /// The view windows collection.
        /// </summary>
        private readonly Collection<ViewWindow> viewWindows = new Collection<ViewWindow>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayModeController"/> class.
        /// </summary>
        /// <param name="displayMode">The display mode.</param>
        public DisplayModeController(DisplayMode displayMode)
            : this(displayMode, SwimLaneService.Instance, ServiceManager.Instance.GetService<IProjectDataService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayModeController"/> class.
        /// </summary>
        /// <param name="displayMode">The display mode.</param>
        /// <param name="swimLaneService">The swim lane service.</param>
        /// <param name="projectDataService">The project data service.</param>
        public DisplayModeController(DisplayMode displayMode, ISwimLaneService swimLaneService, IProjectDataService projectDataService)
        {
            if (displayMode == null)
            {
                throw new ArgumentNullException("displayMode");
            }

            if (swimLaneService == null)
            {
                throw new ArgumentNullException("swimLaneService");
            }

            if (projectDataService == null)
            {
                throw new ArgumentNullException("projectDataService");
            }

            this.displayMode = displayMode;
            this.swimLaneService = swimLaneService;
            this.projectDataService = projectDataService;

            this.swimLaneService.SwimLaneViews.CollectionChanged += this.OnSwimLaneCollectionChanged;
            this.projectDataService.ProjectDataChanged += this.OnProjectDataChanged;

            this.SetUpDisplayMode();
        }

        /// <summary>
        /// Highlights the specified workbench item.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        public void Highlight(IWorkbenchItem workbenchItem)
        {
            CommandLibrary.ShowDisplayModeCommand.Execute(this.displayMode, this.displayMode);

            if (this.projectDataService.CurrentProjectData == null)
            {
                return;
            }

            var highlighter = WorkbenchItemHighlighter.CreateInstance(this.displayMode.PART_MainTabControl);

            highlighter.HighlightItemsInViews(workbenchItem);
        }

        /// <summary>
        /// Sets up display mode.
        /// </summary>
        private void SetUpDisplayMode()
        {
            this.displayMode.MenuControl = new MenuControl { DisplayMode = this.displayMode };
            this.displayMode.Title = Settings.Default.ModeName;
            this.displayMode.Description = Resources.String022;
            this.displayMode.DisplayPriority = Settings.Default.DisplayPriority;

            this.displayMode.CommandBindings.Add(
                new CommandBinding(
                    LocalCommandLibrary.NewWindowCommand, 
                    this.OnExecuteNewWindowCommand, 
                    this.OnCanExecuteNewWindowCommand));
        }

        /// <summary>
        /// Called when [can execute new window command].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void OnCanExecuteNewWindowCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            var swimLaneView = e.Parameter as SwimLaneView;
            e.CanExecute = swimLaneView != null && swimLaneView.ViewMap != null && this.projectDataService.CurrentProjectData != null;
        }

        /// <summary>
        /// Called when [execute new window command].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void OnExecuteNewWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var swimLaneView = e.Parameter as SwimLaneView;
            if (swimLaneView == null || swimLaneView.ViewMap == null)
            {
                return;
            }

            var viewWindow = new ViewWindow(swimLaneView.ViewMap);

            viewWindow.Closed += this.OnViewWindowClosed;

            this.viewWindows.Add(viewWindow);

            viewWindow.Show();
        }

        /// <summary>
        /// Called when [view window closed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnViewWindowClosed(object sender, EventArgs e)
        {
            var viewWindow = sender as ViewWindow;
            if (viewWindow == null)
            {
                return;
            }

            this.viewWindows.Remove(viewWindow);

            viewWindow.Closed -= this.OnViewWindowClosed;
        }

        /// <summary>
        /// Called when [swim lane collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnSwimLaneCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var swimLaneView in e.NewItems.OfType<SwimLaneView>().Where(slv => slv.IncludeInTabs))
                    {
                        this.AddSwimLaneViewTab(swimLaneView);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var swimLaneView in e.OldItems.OfType<SwimLaneView>())
                    {
                        this.RemoveSwimLaneViewTab(swimLaneView);
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (var swimLaneView in this.swimLaneService.SwimLaneViews)
                    {
                        this.RemoveSwimLaneViewTab(swimLaneView);
                    }

                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Removes the swim lane view tab.
        /// </summary>
        /// <param name="swimLaneView">The swim lane view.</param>
        private void RemoveSwimLaneViewTab(SwimLaneView swimLaneView)
        {
            var tabItem = this.displayMode.PART_MainTabControl.Items.OfType<TabItem>()
                .FirstOrDefault(tc => tc.DataContext == swimLaneView);

            if (tabItem == null)
            {
                return;
            }

            BindingOperations.ClearBinding(tabItem, HeaderedContentControl.HeaderProperty);
            BindingOperations.ClearBinding(tabItem, FrameworkElement.ToolTipProperty);

            var viewControl = tabItem.Content as ViewControl;

            if (viewControl != null)
            {
                viewControl.ReleaseResources();
            }

            tabItem.Content = null;
            tabItem.Header = null;

            this.displayMode.PART_MainTabControl.Items.Remove(tabItem);
        }

        /// <summary>
        /// Builds the tabs.
        /// </summary>
        /// <param name="swimLaneView">The swim lane view.</param>
        private void AddSwimLaneViewTab(SwimLaneView swimLaneView)
        {
            var viewControl = new ViewControl { SwimLaneView = swimLaneView, ProjectData = this.projectDataService.CurrentProjectData };

            var tabItem = new TabItem { DataContext = swimLaneView, Content = viewControl };

            tabItem.SetBinding(HeaderedContentControl.HeaderProperty, "ViewMap.Title");
            tabItem.SetBinding(FrameworkElement.ToolTipProperty, "ViewMap.Description");

            var insertIndex =
                this.displayMode.PART_MainTabControl.Items.OfType<TabItem>()
                .Select(item => item.DataContext).OfType<SwimLaneView>()
                .TakeWhile(context => context.ViewMap.DisplayOrder <= swimLaneView.ViewMap.DisplayOrder)
                .Count();

            this.displayMode.PART_MainTabControl.Items.Insert(insertIndex, tabItem);
        }

        /// <summary>
        /// Releases the resources.
        /// </summary>
        private void ReleaseResources()
        {
            foreach (var swimLaneView in this.swimLaneService.SwimLaneViews)
            {
                this.RemoveSwimLaneViewTab(swimLaneView);
            }

            this.displayMode.PART_MainTabControl.Items.Clear();
        }

        /// <summary>
        /// Called when [project data changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.ProjectDataChangedEventArgs"/> instance containing the event data.</param>
        private void OnProjectDataChanged(object sender, ProjectDataChangedEventArgs e) 
        {
            this.ReleaseResources();

            this.swimLaneService.Initialise(e.NewValue);
            ((MenuControl)this.displayMode.MenuControl).ProjectData = e.NewValue;
        }
    }
}
