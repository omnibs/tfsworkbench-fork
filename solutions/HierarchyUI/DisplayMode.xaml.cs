// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayMode.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for DisplayMode.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using TfsWorkbench.UIElements.DragHelpers;
using TfsWorkbench.UIElements.FilterObjects;

namespace TfsWorkbench.HierarchyUI
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Core.DataObjects;
    using Core.EventArgObjects;
    using Core.Helpers;
    using Core.Interfaces;
    using Helpers;

    using HierarchyObjects;

    using Properties;

    using TfsWorkbench.Core.Services;

    using UIElements;

    /// <summary>
    /// Interaction logic for DisplayMode.xaml
    /// </summary>
    [Export(typeof(IDisplayMode))]
    public partial class DisplayMode : IDisplayMode
    {
        /// <summary>
        /// The project data service instance.
        /// </summary>
        private readonly IProjectDataService projectDataService;

        /// <summary>
        /// The link manager serice.
        /// </summary>
        private readonly ILinkManagerService linkManagerService;

        /// <summary>
        /// The orientation property.
        /// </summary>
        private static readonly DependencyProperty orientationProperty = DependencyProperty.Register(
            "Orientation",
            typeof(Orientation),
            typeof(DisplayMode),
            new PropertyMetadata(Orientation.Vertical, OnFiltersChanged));

        /// <summary>
        /// The filter property.
        /// </summary>
        private static readonly DependencyProperty filterProperty = DependencyProperty.Register(
            "Filter",
            typeof(FilterCollection),
            typeof(DisplayMode));

        /// <summary>
        /// The selected views property.
        /// </summary>
        private static readonly DependencyProperty selectedViewsProperty = DependencyProperty.Register(
            "SelectedViews",
            typeof(ObservableCollection<SelectedValue>),
            typeof(DisplayMode));

        /// <summary>
        /// The selected states property.
        /// </summary>
        private static readonly DependencyProperty selectedStatesProperty = DependencyProperty.Register(
            "SelectedStates",
            typeof(ObservableCollection<SelectedValue>),
            typeof(DisplayMode));

        /// <summary>
        /// The is excluding empty views property.
        /// </summary>
        private static readonly DependencyProperty isExcludingEmptyViewsProperty = DependencyProperty.Register(
            "IsExcludingEmptyViews",
            typeof(bool),
            typeof(DisplayMode),
            new PropertyMetadata(false, OnFiltersChanged));

        /// <summary>
        /// The subscribed view map dicationary;
        /// </summary>
        private readonly IList<ViewMap> subscribedViewMaps = new List<ViewMap>();

        /// <summary>
        /// The has queued updated flag;
        /// </summary>
        private bool hasQueuedUpdate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayMode"/> class.
        /// </summary>
        public DisplayMode()
            : this(ServiceManager.Instance.GetService<IProjectDataService>(), ServiceManager.Instance.GetService<ILinkManagerService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayMode"/> class.
        /// </summary>
        /// <param name="projectDataService">The project data service.</param>
        /// <param name="linkManagerService">The link manager service.</param>
        public DisplayMode(IProjectDataService projectDataService, ILinkManagerService linkManagerService)
        {
            if (projectDataService == null)
            {
                throw new ArgumentNullException("projectDataService");
            }

            if (linkManagerService == null)
            {
                throw new ArgumentNullException("linkManagerService");
            }

            InitializeComponent();

            this.projectDataService = projectDataService;
            this.linkManagerService = linkManagerService;
            this.MenuControl = new MenuControl { DisplayMode = this };
            this.Title = Settings.Default.ModeName;
            this.DisplayPriority = Settings.Default.DisplayPriority;
            this.Description = Settings.Default.ModeDescription;

            ElementDragHelper.RegisterScrollViewer(this.PART_LayoutScroller);

            this.SetUpCommandBindings();

            this.Filter = new FilterCollection(this.Dispatcher);
            this.Filter.SelectionChanged += this.OnFilterSelectionChanged;

            this.SelectedViews = new ObservableCollection<SelectedValue>();
            this.SelectedStates = new ObservableCollection<SelectedValue>();

            var dpd = DependencyPropertyDescriptor.FromProperty(VisibilityProperty, this.GetType());
            if (dpd != null)
            {
                dpd.AddValueChanged(this, delegate { this.ResetLayout(); });
            }

            this.projectDataService.ProjectDataChanged += this.OnProjectDataChanged;
        }

        /// <summary>
        /// Gets the orientation property.
        /// </summary>
        /// <value>The orientation property.</value>
        public static DependencyProperty OrientationProperty
        {
            get { return orientationProperty; }
        }

        /// <summary>
        /// Gets the filter property.
        /// </summary>
        /// <value>The filter property.</value>
        public static DependencyProperty FilterProperty
        {
            get { return filterProperty; }
        }

        /// <summary>
        /// Gets the selected views property.
        /// </summary>
        /// <value>The selected views property.</value>
        public static DependencyProperty SelectedViewsProperty
        {
            get { return selectedViewsProperty; }
        }

        /// <summary>
        /// Gets the selected states property.
        /// </summary>
        /// <value>The selected states property.</value>
        public static DependencyProperty SelectedStatesProperty
        {
            get { return selectedStatesProperty; }
        }

        /// <summary>
        /// Gets the is excluding empty views property.
        /// </summary>
        /// <value>The is excluding empty views property.</value>
        public static DependencyProperty IsExcludingEmptyViewsProperty
        {
            get { return isExcludingEmptyViewsProperty; }
        }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        public FilterCollection Filter
        {
            get { return (FilterCollection)this.GetValue(FilterProperty); }
            set { this.SetValue(FilterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public Orientation Orientation
        {
            get { return (Orientation)this.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selected views.
        /// </summary>
        /// <value>The selected views.</value>
        public ObservableCollection<SelectedValue> SelectedViews
        {
            get { return (ObservableCollection<SelectedValue>)this.GetValue(SelectedViewsProperty); }
            set { this.SetValue(SelectedViewsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selected states.
        /// </summary>
        /// <value>The selected states.</value>
        public ObservableCollection<SelectedValue> SelectedStates
        {
            get { return (ObservableCollection<SelectedValue>)this.GetValue(SelectedStatesProperty); }
            set { this.SetValue(SelectedStatesProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is excluding empty views.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is excluding empty views; otherwise, <c>false</c>.
        /// </value>
        public bool IsExcludingEmptyViews
        {
            get { return (bool)this.GetValue(IsExcludingEmptyViewsProperty); }
            set { this.SetValue(IsExcludingEmptyViewsProperty, value); }
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
        /// Shows the specified workbech item.
        /// </summary>
        /// <param name="workbenchItem">The workbech item.</param>
        public void Highlight(IWorkbenchItem workbenchItem)
        {
            CommandLibrary.ShowDisplayModeCommand.Execute(this, this);

            IEnumerable<HierarchyItemNode> existingVisuals;
            if (!LayoutHelper.TryGetAssociatedVisuals(this.PART_LayoutCanvas, workbenchItem, out existingVisuals))
            {
                LocalCommandLibrary.FilterToItemCommand.Execute(workbenchItem, this);
                this.Dispatcher.BeginInvoke(
                    DispatcherPriority.SystemIdle,
                    (SendOrPostCallback)delegate { this.Highlight(workbenchItem); },
                    null);
                return;
            }

            var isInView = false;

            foreach (var hierarchyItemNode in existingVisuals)
            {
                HighlightHelper.Highlight(hierarchyItemNode);

                if (isInView)
                {
                    continue;
                }

                hierarchyItemNode.BringIntoView();
                isInView = true;
            }
        }

        /// <summary>
        /// Called when [orientation changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFiltersChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as DisplayMode;
            if (control == null)
            {
                return;
            }

            control.EnqueueLayoutUpdate();
        }

        /// <summary>
        /// Called when [project data changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.ProjectDataChangedEventArgs"/> instance containing the event data.</param>
        private void OnProjectDataChanged(object sender, ProjectDataChangedEventArgs e)
        {
            this.PART_FiltersControl.SelectedItemCount = this.Filter.SelectedWorkbenchItemCount;

            if (e.OldValue != null)
            {
                LayoutHelper.ClearCanvas(this.PART_LayoutCanvas);
                this.AttachListeners(e.OldValue, false);
            }

            this.Filter.Initialise(e.NewValue);

            this.InitialiseSelectedViews();
            this.InitialiseSelectedStates(e.NewValue);

            if (e.NewValue != null)
            {
                this.AttachListeners(e.NewValue);
            }
        }

        /// <summary>
        /// Sets up command bindings.
        /// </summary>
        private void SetUpCommandBindings()
        {
            this.CommandBindings.Add(
                new CommandBinding(
                    LocalCommandLibrary.FilterToItemCommand,
                    this.OnFilterToItem));

            this.CommandBindings.Add(
                new CommandBinding(
                    LocalCommandLibrary.FilterToViewCommand,
                    this.OnFilterToView));

            this.CommandBindings.Add(
                new CommandBinding(
                    LocalCommandLibrary.EditViewCommand,
                    this.OnEditView));

            this.CommandBindings.Add(
                new CommandBinding(
                    LocalCommandLibrary.AddChildItemCommand,
                    this.OnAddChildItem));

            this.CommandBindings.Add(
                new CommandBinding(
                    LocalCommandLibrary.LinkItemCommand,
                    this.OnLinkItemCommand));

            this.CommandBindings.Add(
                new CommandBinding(
                    LocalCommandLibrary.UnlinkItemCommand,
                    this.OnUnlinkItemCommand));
        }

        /// <summary>
        /// Initialises the selected states.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        private void InitialiseSelectedStates(IProjectData projectData)
        {
            this.SelectedStates.Clear();

            if (projectData == null)
            {
                return;
            }

            foreach (var state in projectData.ItemTypes.SelectMany(t => t.States).Distinct().OrderBy(s => s))
            {
                this.SelectedStates.Add(new SelectedValue { IsSelected = true, Text = state });
            }
        }

        /// <summary>
        /// Attaches the listeners.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="attach">if set to <c>true</c> [attach].</param>
        private void AttachListeners(IProjectData projectData, bool attach = true)
        {
            if (projectData == null)
            {
                return;
            }

            if (attach)
            {
                projectData.WorkbenchItems.CollectionChanged += this.OnCollectionChanged;
                projectData.ViewMaps.CollectionChanged += this.OnViewMapsCollectionChanged;
                projectData.WorkbenchItems.LinkChanged += this.OnLinkChanged;
            }
            else
            {
                projectData.WorkbenchItems.CollectionChanged -= this.OnCollectionChanged;
                projectData.ViewMaps.CollectionChanged -= this.OnViewMapsCollectionChanged;
                projectData.WorkbenchItems.LinkChanged -= this.OnLinkChanged;
            }
        }

        /// <summary>
        /// Called when [collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.RepositoryChangedEventArgs&lt;IWorkbenchItem&gt;"/> instance containing the event data.</param>
        private void OnCollectionChanged(object sender, RepositoryChangedEventArgs<IWorkbenchItem> e)
        {
            Action execute = () =>
                {
                    switch (e.Action)
                    {
                        case ChangeActionOption.Remove:
                            foreach (var workbenchItem in e.Context)
                            {
                                LayoutHelper.RemoveAssociatedVisual(this.PART_LayoutCanvas, workbenchItem);
                            }

                            break;
                        case ChangeActionOption.Clear:
                            LayoutHelper.ClearCanvas(this.PART_LayoutCanvas);

                            break;
                        case ChangeActionOption.Refresh:
                            this.EnqueueLayoutUpdate();

                            break;
                        default:
                            return;
                    }
                };

            if (Thread.CurrentThread.Equals(this.Dispatcher.Thread))
            {
                execute();
            }
            else
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, execute);
            }
        }

        /// <summary>
        /// Called when [link changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.ItemLinkChangeEventArgs"/> instance containing the event data.</param>
        private void OnLinkChanged(object sender, ItemLinkChangeEventArgs e)
        {
            Func<ILinkItem, bool> hasEffect = l =>
                {
                    if (l == null)
                    {
                        return false;
                    }

                    // Link parent is rendered and link view is selected.
                    IEnumerable<HierarchyItemNode> parentVisuals;
                    return 
                        LayoutHelper.TryGetAssociatedVisuals(this.PART_LayoutCanvas, l.Parent, out parentVisuals) 
                        && this.SelectedViews.Any(v => v.IsSelected && Equals(v.Text, l.LinkName));
                };

            if (hasEffect(e.OldValue) || hasEffect(e.NewValue))
            {
                this.EnqueueLayoutUpdate();
            }
        }

        /// <summary>
        /// Called when [swimlane collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnViewMapsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.InitialiseSelectedViews();
        }

        /// <summary>
        /// Called when [link item command].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void OnLinkItemCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var parameter = e.Parameter as Tuple<HierarchyView, IWorkbenchItem>;

            if (parameter == null)
            {
                return;
            }

            var parentItem = parameter.Item1.ChildCreationParameters.Parent;
            var linkToAdd = Factory.BuildLinkItem(parameter.Item1.ChildCreationParameters.LinkTypeName, parentItem, parameter.Item2);
            this.linkManagerService.AddLink(linkToAdd);

            this.EnqueueLayoutUpdate();
        }

        /// <summary>
        /// Called when [unlink item command].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void OnUnlinkItemCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var parameter = e.Parameter as Tuple<HierarchyView, IWorkbenchItem>;

            if (parameter == null)
            {
                return;
            }

            var creationParameters = parameter.Item1.ChildCreationParameters;
            var linkToRemove = parameter.Item2.ParentLinks.FirstOrDefault(l => Equals(creationParameters.LinkTypeName, l.LinkName) && Equals(creationParameters.Parent, l.Parent));
            this.linkManagerService.RemoveLink(linkToRemove);

            this.EnqueueLayoutUpdate();
        }

        /// <summary>
        /// Called when [filter to view].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void OnFilterToView(object sender, ExecutedRoutedEventArgs e)
        {
            var viewTitle = e.Parameter as string;

            if (string.IsNullOrEmpty(viewTitle))
            {
                return;
            }

            foreach (var selectedValue in this.SelectedViews)
            {
                selectedValue.IsSelected = selectedValue.Text.Equals(viewTitle);
            }

            this.EnqueueLayoutUpdate();
        }

        /// <summary>
        /// Initialises the selected views.
        /// </summary>
        private void InitialiseSelectedViews()
        {
            this.SelectedViews.Clear();

            foreach (var viewMap in this.subscribedViewMaps)
            {
                viewMap.PropertyChanged -= this.OnViewMapPropertyChanged;
            }

            this.subscribedViewMaps.Clear();

            var projectData = this.projectDataService.CurrentProjectData;

            if (projectData == null)
            {
                return;
            }

            foreach (var viewMap in projectData.ViewMaps.OrderBy(vm => vm.DisplayOrder))
            {
                var selectedValue = new SelectedValue { IsSelected = true, Text = viewMap.Title };
                this.subscribedViewMaps.Add(viewMap);
                this.SelectedViews.Add(selectedValue);
                viewMap.PropertyChanged += this.OnViewMapPropertyChanged;
            }
        }

        /// <summary>
        /// Called when [view map property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnViewMapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Title"))
            {
                this.InitialiseSelectedViews();
            }
        }

        /// <summary>
        /// Called when [filter selection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnFilterSelectionChanged(object sender, EventArgs e)
        {
            this.EnqueueLayoutUpdate();
        }

        /// <summary>
        /// Enqueues the layout render.
        /// </summary>
        private void EnqueueLayoutUpdate()
        {
            if (this.hasQueuedUpdate)
            {
                return;
            }

            this.hasQueuedUpdate = true;

            if (this.IsVisible)
            {
                this.ResetLayout();
            }
        }

        /// <summary>
        /// Called when [edit view].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void OnEditView(object sender, ExecutedRoutedEventArgs e)
        {
            var viewMap = e.Parameter as ViewMap;

            if (viewMap == null)
            {
                return;
            }

            CommandLibrary.EditViewCommand.Execute(
                this.projectDataService.CurrentProjectData.ViewMaps.FirstOrDefault(vm => Equals(vm, viewMap)), this);
        }

        /// <summary>
        /// Called when [filter to command].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void OnFilterToItem(object sender, ExecutedRoutedEventArgs e)
        {
            var workbenchItem = e.Parameter as IWorkbenchItem;

            if (workbenchItem == null || this.projectDataService.CurrentProjectData == null)
            {
                return;
            }

            this.Filter.FilterToItem(workbenchItem);

            this.EnqueueLayoutUpdate();
        }

        /// <summary>
        /// Handles the PreviewMouseWheel event of the ScrollViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseWheelEventArgs"/> instance containing the event data.</param>
        private void ScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Shift)
            {
                return;
            }

            if (e.Delta < 0)
            {
                this.PART_ScaleSlider.Value -= 0.05;
            }
            else
            {
                this.PART_ScaleSlider.Value += 0.05;
            }
        }

        /// <summary>
        /// Handles the Click event of the ResetButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ResetButtonClick(object sender, RoutedEventArgs e)
        {
            this.EnqueueLayoutUpdate();
        }

        /// <summary>
        /// Resets the layout.
        /// </summary>
        private void ResetLayout()
        {
            if (!this.IsVisible || !this.hasQueuedUpdate)
            {
                return;
            }

            LayoutHelper.ClearCanvas(this.PART_LayoutCanvas);
            if (this.projectDataService.CurrentProjectData == null)
            {
                return;
            }

            Action message = () =>
                {
                    CommandLibrary.ApplicationMessageCommand.Execute("Updating hierachy mode layout...", this);
                    CommandLibrary.DisableUserInputCommand.Execute(true, this);
                };

            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, message);

            Action callback = () =>
            {
                LayoutHelper.RenderView(
                    this.projectDataService.CurrentProjectData,
                    this.PART_LayoutCanvas,
                    this.Orientation,
                    this.Filter.IsMatch,
                    this.SelectedViews,
                    this.IsExcludingEmptyViews,
                    this.SelectedStates);

                CommandLibrary.DisableUserInputCommand.Execute(false, this);
                CommandLibrary.ApplicationMessageCommand.Execute("Hierachy mode layout updated.", this);

                this.hasQueuedUpdate = false;
            };

            this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, callback);
        }

        /// <summary>
        /// Called when [add child item command].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void OnAddChildItem(object sender, ExecutedRoutedEventArgs e)
        {
            var creationParameters = e.Parameter as IChildCreationParameters;

            if (creationParameters == null || this.projectDataService.CurrentProjectData == null)
            {
                return;
            }

            CommandLibrary.CreateChildCommand.Execute(creationParameters, this);

            this.EnqueueLayoutUpdate();
        }

        /// <summary>
        /// Called when [selected views changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnFilterSelectionsChanged(object sender, EventArgs e)
        {
            this.EnqueueLayoutUpdate();
        }
    }
}
