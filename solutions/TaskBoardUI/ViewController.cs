// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewController.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ViewController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.TaskBoardUI
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Threading;

    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.TaskBoardUI.Constants;
    using TfsWorkbench.TaskBoardUI.Converters;
    using TfsWorkbench.TaskBoardUI.DataObjects;
    using TfsWorkbench.TaskBoardUI.Helpers;
    using TfsWorkbench.UIElements;
    using TfsWorkbench.UIElements.DragHelpers;

    /// <summary>
    /// The view controller class.
    /// </summary>
    internal class ViewController
    {
        /// <summary>
        /// The drag controller instance.
        /// </summary>
        private readonly ElementDragController<IWorkbenchItem> elementDragController;

        /// <summary>
        /// the controlled view control.
        /// </summary>
        private readonly ViewControl viewControl;

        /// <summary>
        /// The link manager service instance.
        /// </summary>
        private readonly ILinkManagerService linkManagerService;

        /// <summary>
        /// The drag target helper.
        /// </summary>
        private readonly DragTargetHelper dragTargetHelper;

        /// <summary>
        /// The drag event handler instance.
        /// </summary>
        private readonly DragDeltaEventHandler dragEventHandler = ThumbDragDelta;

        /// <summary>
        /// The clean Up Virtualized Item Event Handler instance.
        /// </summary>
        private readonly CleanUpVirtualizedItemEventHandler cleanUpVirtualizedItemEventHandler;

        /// <summary>
        /// The indexed item converter.
        /// </summary>
        private readonly IndexedItemConverter indexedItemConverter = new IndexedItemConverter();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewController"/> class.
        /// </summary>
        /// <param name="viewControl">The view control.</param>
        public ViewController(ViewControl viewControl)
            : this(viewControl, ServiceManager.Instance.GetService<ILinkManagerService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewController"/> class.
        /// </summary>
        /// <param name="viewControl">The view control.</param>
        /// <param name="linkManagerService">The link manager service.</param>
        public ViewController(ViewControl viewControl, ILinkManagerService linkManagerService)
        {
            if (viewControl == null)
            {
                throw new ArgumentNullException("viewControl");
            }

            if (linkManagerService == null)
            {
                throw new ArgumentNullException("linkManagerService");
            }

            this.viewControl = viewControl;
            this.linkManagerService = linkManagerService;

            this.cleanUpVirtualizedItemEventHandler = new CleanUpVirtualizedItemEventHandler(this.OnCleanUp);
            this.elementDragController = new ElementDragController<IWorkbenchItem>(this.viewControl);
            this.dragTargetHelper = new DragTargetHelper(this.elementDragController);

            this.AlterEventHandlers(this.viewControl);
            this.AlterEventHandlers(this.elementDragController);
        }

        /// <summary>
        /// Releases the references.
        /// </summary>
        public void ReleaseResources()
        {
            this.AlterEventHandlers(this.viewControl, true);
            this.AlterEventHandlers(this.elementDragController, true);

            var control = this.viewControl;

            foreach (var column in control.PART_MainGridView.Columns)
            {
                BindingOperations.ClearAllBindings(column);
            }

            this.dragTargetHelper.UnregisterAllCollections();

            control.PART_ViewSorter.View = null;
            control.PART_ViewSorter.Fields.Clear();
            control.PART_ViewSorter = null;

            var swimLaneView = control.SwimLaneView;
            if (swimLaneView != null)
            {
                swimLaneView.ReleaseResources();
                swimLaneView.LayoutChanged -= this.OnViewLayoutChanged;
                swimLaneView.SwimLaneRows.CollectionChanged -= this.OnSwimLaneRowCollectionChanged;
                control.SwimLaneView = null;
            }

            control.ProjectData = null;
        }

        /// <summary>
        /// Resizes the columns to fit.
        /// </summary>
        public void ResizeColumnsToFit()
        {
            // Resize swimlanes
            var avilableWidth = this.viewControl.PART_MainListView.ActualWidth - 30;

            var newWidth = avilableWidth / this.viewControl.PART_MainGridView.Columns.Count();

            newWidth = newWidth < Numbers.MinColumnWidth
                           ? Numbers.MinColumnWidth
                           : newWidth > Numbers.MaxColumnWidth
                                 ? Numbers.MaxColumnWidth
                                 : newWidth;

            foreach (var column in this.viewControl.PART_MainGridView.Columns)
            {
                column.Width = newWidth;
            }

            // Resize other item containers
            newWidth = avilableWidth / this.viewControl.PART_OtherItemsGridView.Columns.Count();

            newWidth = newWidth < Numbers.MinColumnWidth
                           ? Numbers.MinColumnWidth
                           : newWidth > Numbers.MaxColumnWidth
                                 ? Numbers.MaxColumnWidth
                                 : newWidth;

            foreach (var column in this.viewControl.PART_OtherItemsGridView.Columns)
            {
                column.Width = newWidth;
            }
        }

        /// <summary>
        /// Synchronises the sort fields.
        /// </summary>
        public void SetSortFields()
        {
            var control = this.viewControl;
            var projectData = control.ProjectData;
            var swimLaneView = control.SwimLaneView;

            if (projectData == null || swimLaneView == null)
            {
                return;
            }

            var parentFields = new List<string>();

            foreach (var parentTypeName in swimLaneView.ViewMap.ParentTypes)
            {
                var parentType = projectData.ItemTypes[parentTypeName];

                if (parentType == null)
                {
                    throw new NullReferenceException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Properties.Resources.String019,
                            parentTypeName));
                }

                parentFields.AddRange(parentType.Fields.Select(f => f.DisplayName).Where(f => !parentFields.Contains(f)));
            }

            var selectedSortField = swimLaneView.ViewMap.ParentSorter.FieldName;

            control.PART_ViewSorter.Fields.Clear();
            foreach (var parentField in parentFields.OrderBy(pf => pf))
            {
                control.PART_ViewSorter.Fields.Add(parentField);
            }

            control.PART_ViewSorter.PART_Selector.SelectedItem = selectedSortField;
        }

        /// <summary>
        /// Wires up swim lane view.
        /// </summary>
        public void WireUpSwimLaneView()
        {
            var control = this.viewControl;

            this.SetSortFields();
            control.SwimLaneView.LayoutChanged += this.OnViewLayoutChanged;
            control.SwimLaneView.SwimLaneRows.CollectionChanged += this.OnSwimLaneRowCollectionChanged;
            this.BuildGridView();
        }

        /// <summary>
        /// Handles the DragDelta event of the Thumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private static void ThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var senderAsThumb = e.OriginalSource as Thumb;

            if (senderAsThumb == null)
            {
                return;
            }

            var header = senderAsThumb.TemplatedParent as GridViewColumnHeader;

            if (header != null)
            {
                if (header.Column.ActualWidth < Numbers.MinColumnWidth)
                {
                    header.Column.Width = Numbers.MinColumnWidth;
                }

                if (header.Column.ActualWidth > Numbers.MaxColumnWidth)
                {
                    header.Column.Width = Numbers.MaxColumnWidth;
                }

                return;
            }

            var swimLaneRow = senderAsThumb.DataContext as SwimLaneRow;

            if (swimLaneRow == null || !Equals(senderAsThumb.Tag, "RowSizeGripper"))
            {
                return;
            }

            swimLaneRow.RowHeight += e.VerticalChange;

            if (swimLaneRow.RowHeight < Numbers.MinRowHeight)
            {
                swimLaneRow.RowHeight = Numbers.MinRowHeight;
            }

            if (swimLaneRow.RowHeight > Numbers.MaxRowHeight)
            {
                swimLaneRow.RowHeight = Numbers.MaxRowHeight;
            }
        }

        /// <summary>
        /// Called when [items dragged].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.UIElements.DragHelpers.DragDataEventArgs&lt;IWorkbenchItem&gt;"/> instance containing the event data.</param>
        private static void OnItemsDragged(object sender, DragDataEventArgs<IWorkbenchItem> e)
        {
            var sourceCollection = e.Source.ItemsSource as ICollection<IWorkbenchItem>;

            if (sourceCollection == null)
            {
                return;
            }

            foreach (var child in e.Items)
            {
                sourceCollection.Remove(child);
            }
        }

        /// <summary>
        /// Processes the changed items control.
        /// </summary>
        /// <param name="itemsControl">The list view.</param>
        private void ProcessChangedItemsControl(ItemsControl itemsControl)
        {
            var generator = itemsControl.ItemContainerGenerator;

            if (this.elementDragController == null || generator.Status != GeneratorStatus.ContainersGenerated)
            {
                return;
            }

            // Trawl the containers for unregistered drag targets
            this.dragTargetHelper.SynchroniseDragElements(itemsControl);
        }

        /// <summary>
        /// Wires up drag contoller.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="releaseHandlers">if set to <c>true</c> [release handlers].</param>
        private void AlterEventHandlers(IElementDragController<IWorkbenchItem> target, bool releaseHandlers = false)
        {
            if (releaseHandlers)
            {
                target.ItemsDragged -= OnItemsDragged;
                target.ItemsDropped -= this.OnItemsDropped;
                target.ItemDragFailed -= this.OnItemDragFailed;
            }
            else
            {
                target.ItemsDragged += OnItemsDragged;
                target.ItemsDropped += this.OnItemsDropped;
                target.ItemDragFailed += this.OnItemDragFailed;
            }
        }

        /// <summary>
        /// Sets up view control.
        /// </summary>
        /// <param name="target">The view control.</param>
        /// <param name="release">if set to <c>true</c> [release handlers].</param>
        private void AlterEventHandlers(ViewControl target, bool release = false)
        {
            var mainListView = target.PART_MainListView;
            var mainGridView = target.PART_MainGridView;
            var otherItemsListView = target.PART_OtherItemsListView;
            var scaleSlider = target.PART_ScaleSlider;

            if (release)
            {
                this.viewControl.CommandBindings.Clear();

                mainListView.ItemContainerGenerator.StatusChanged -= this.OnMainListViewStatusChanged;
                mainListView.MouseDoubleClick -= this.OnMainListViewMouseDoubleClick;
                mainListView.PreviewMouseWheel -= this.OnMainListViewPreviewMouseWheel;
                mainGridView.Columns.CollectionChanged -= this.OnSwimLaneColumnsChanged;
                otherItemsListView.ItemContainerGenerator.StatusChanged -= this.OnOtherItemsListViewStatusChanged;
                scaleSlider.ValueChanged -= this.OnScaleChanged;

                mainListView.RemoveHandler(Thumb.DragDeltaEvent, this.dragEventHandler);
                mainListView.RemoveHandler(VirtualizingStackPanel.CleanUpVirtualizedItemEvent, this.cleanUpVirtualizedItemEventHandler);
                otherItemsListView.RemoveHandler(Thumb.DragDeltaEvent, this.dragEventHandler);
            }
            else
            {
                this.viewControl.CommandBindings.Add(
                    new CommandBinding(LocalCommandLibrary.ResetCommand, this.OnExectuteResetCommand));

                mainListView.ItemContainerGenerator.StatusChanged += this.OnMainListViewStatusChanged;
                mainListView.MouseDoubleClick += this.OnMainListViewMouseDoubleClick;
                mainListView.PreviewMouseWheel += this.OnMainListViewPreviewMouseWheel;
                mainGridView.Columns.CollectionChanged += this.OnSwimLaneColumnsChanged;
                otherItemsListView.ItemContainerGenerator.StatusChanged += this.OnOtherItemsListViewStatusChanged;
                scaleSlider.ValueChanged += this.OnScaleChanged;

                mainListView.AddHandler(Thumb.DragDeltaEvent, this.dragEventHandler, true);
                mainListView.AddHandler(VirtualizingStackPanel.CleanUpVirtualizedItemEvent, this.cleanUpVirtualizedItemEventHandler);
                otherItemsListView.AddHandler(Thumb.DragDeltaEvent, this.dragEventHandler, true);
            }
        }

        /// <summary>
        /// Called when [items dropped].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.UIElements.DragHelpers.DragDataEventArgs&lt;IWorkbenchItem&gt;"/> instance containing the event data.</param>
        private void OnItemsDropped(object sender, DragDataEventArgs<IWorkbenchItem> e)
        {
            var targetCollection = e.Target.ItemsSource as ICollection<IWorkbenchItem>;

            if (targetCollection == null)
            {
                return;
            }

            this.HandleBucketTransition(e, true);

            var stateCollection = targetCollection as StateCollection;

            foreach (var item in e.Items)
            {
                if (stateCollection != null)
                {
                    stateCollection.Add(item);
                    continue;
                }

                targetCollection.Add(item);
            }

            this.HandleBucketTransition(e, false);

            this.InvalidateCommands();
        }

        /// <summary>
        /// Called when [item drag failed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.UIElements.DragHelpers.DragDataEventArgs&lt;IWorkbenchItem&gt;"/> instance containing the event data.</param>
        private void OnItemDragFailed(object sender, DragDataEventArgs<IWorkbenchItem> e)
        {
            var itemCollection = e.Source.ItemsSource as ICollection<IWorkbenchItem>;

            if (itemCollection == null)
            {
                return;
            }

            foreach (var item in e.Items)
            {
                itemCollection.Add(item);
            }

            this.InvalidateCommands();
        }

        /// <summary>
        /// Handles the OnMouseDoubleClick event of the MainListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnMainListViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var source = e.OriginalSource as FrameworkElement;

            if (source == null)
            {
                return;
            }

            var workbenchItem = source.DataContext as IWorkbenchItem;

            if (workbenchItem != null)
            {
                CommandLibrary.EditItemCommand.Execute(workbenchItem, this.viewControl);
                return;
            }

            var swimLaneRow = source.DataContext as SwimLaneRow;

            if (swimLaneRow == null)
            {
                return;
            }

            CommandLibrary.EditItemCommand.Execute(swimLaneRow.Parent, this.viewControl);
        }

        /// <summary>
        /// Mains the list view preview mouse wheel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseWheelEventArgs"/> instance containing the event data.</param>
        private void OnMainListViewPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Shift)
            {
                return;
            }

            if (e.Delta < 0)
            {
                this.viewControl.PART_ScaleSlider.Value -= 0.05;
            }
            else
            {
                this.viewControl.PART_ScaleSlider.Value += 0.05;
            }

            e.Handled = true;
        }

        /// <summary>
        /// Called when [clean up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.CleanUpVirtualizedItemEventArgs"/> instance containing the event data.</param>
        private void OnCleanUp(object sender, CleanUpVirtualizedItemEventArgs e)
        {
            var dragTargetCollection = e.UIElement as FrameworkElement;
            if (dragTargetCollection != null)
            {
                this.dragTargetHelper.UnregisterCollection(dragTargetCollection);
            }
        }

        /// <summary>
        /// Called when [main list view status changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnMainListViewStatusChanged(object sender, EventArgs e)
        {
            this.ProcessChangedItemsControl(this.viewControl.PART_MainListView);
        }

        /// <summary>
        /// Called when [other items list view status changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnOtherItemsListViewStatusChanged(object sender, EventArgs e)
        {
            this.ProcessChangedItemsControl(this.viewControl.PART_OtherItemsListView);
        }

        /// <summary>
        /// Called when [scale changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="double"/> instance containing the event data.</param>
        private void OnScaleChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.viewControl.PART_MainGridView != null)
            {
                this.ResizeColumnsToFit();
            }
        }

        /// <summary>
        /// Called when [swim lane columns changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnSwimLaneColumnsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Move)
            {
                return;
            }

            var swimLaneView = this.viewControl.SwimLaneView;

            swimLaneView.ViewMap.SwimLaneStates.Clear();
            var states = this.viewControl.PART_MainGridView.Columns
                .Where(c => !c.Header.Equals(string.Empty))
                .Select(c => c.Header.ToString());

            foreach (var state in states)
            {
                swimLaneView.ViewMap.SwimLaneStates.Add(state);
            }
        }

        /// <summary>
        /// Called when [view layout changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnViewLayoutChanged(object sender, EventArgs e)
        {
            this.ResetLayoutAndContent();
        }

        /// <summary>
        /// Called when [exectute reset command].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void OnExectuteResetCommand(object sender, ExecutedRoutedEventArgs e)
        {
            this.viewControl.SwimLaneView.ApplySort();

            foreach (var swimLaneRow in this.viewControl.SwimLaneView.SwimLaneRows)
            {
                swimLaneRow.RowHeight = Properties.Settings.Default.SwimLaneRowHeight;
            }

            this.ResetLayoutAndContent();
        }

        /// <summary>
        /// Handles the bucket transition.
        /// </summary>
        /// <param name="eventArgs">The <see cref="DragDataEventArgs&lt;IWorkbenchItem&gt;"/> instance containing the event data.</param>
        /// <param name="isPreTransition">if set to <c>true</c> [is pre transition].</param>
        private void HandleBucketTransition(DragDataEventArgs<IWorkbenchItem> eventArgs, bool isPreTransition)
        {
            var targetStateCollection = eventArgs.Target.ItemsSource as StateCollection;
            var sourceStateCollection = eventArgs.Source.ItemsSource as StateCollection;

            if (targetStateCollection == null || sourceStateCollection == null)
            {
                return;
            }

            var swimLaneView = this.viewControl.SwimLaneView;

            var isTargetBucket = swimLaneView.BucketStates.Contains(targetStateCollection);
            var isTargetSwimlane = !swimLaneView.BucketStates.Contains(targetStateCollection);
            var isSourceBucket = swimLaneView.BucketStates.Contains(sourceStateCollection);
            var isSourceSwimlane = !swimLaneView.BucketStates.Contains(sourceStateCollection);

            // If moved from bucket to bucket, do not change link data
            if (isTargetBucket && isSourceBucket)
            {
                return;
            }

            // If moved from swimlane to bucket, temporarily add parent reference to bucket
            if (isTargetBucket && isSourceSwimlane)
            {
                targetStateCollection.Parent = isPreTransition ? sourceStateCollection.Parent : null;
                return;
            }

            if (!isPreTransition || !isTargetSwimlane || !isSourceBucket)
            {
                return;
            }

            // If moved from bucket to swimlane, remove existing parent references
            var linkName = swimLaneView.ViewMap.LinkName;
            var parentTypes = swimLaneView.ViewMap.ParentTypes;

            var linksToRemove =
                eventArgs.Items.SelectMany(i => i.ChildLinks.Where(
                    l =>
                    parentTypes.Contains(l.Parent.GetTypeName())
                    && Equals(l.LinkName, linkName)))
                    .ToArray();

            foreach (var link in linksToRemove)
            {
                this.linkManagerService.RemoveLink(link);
            }
        }

        /// <summary>
        /// Invalidates the commands.
        /// </summary>
        private void InvalidateCommands()
        {
            this.viewControl.Dispatcher.BeginInvoke(
                DispatcherPriority.ApplicationIdle,
                new Action(CommandManager.InvalidateRequerySuggested));
        }

        /// <summary>
        /// Called when [swim lane row collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnSwimLaneRowCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.elementDragController == null || this.viewControl.PART_MainListView == null || e.OldItems == null)
            {
                return;
            }

            // Release the drag controller hooks for the removed items.
            var generator = this.viewControl.PART_MainListView.ItemContainerGenerator;

            foreach (var itemContainer in
                e.OldItems.OfType<SwimLaneRow>().Select(generator.ContainerFromItem).OfType<FrameworkElement>())
            {
                this.dragTargetHelper.UnregisterCollection(itemContainer);
            }
        }

        /// <summary>
        /// Builds the grid view.
        /// </summary>
        private void BuildGridView()
        {
            this.dragTargetHelper.UnregisterAllCollections();

            var control = this.viewControl;

            foreach (var column in control.PART_MainGridView.Columns)
            {
                BindingOperations.ClearAllBindings(column);
            }

            control.PART_MainGridView.Columns.Clear();
            control.PART_OtherItemsGridView.Columns.Clear();

            for (var i = 0; i < control.SwimLaneView.RowHeaders.Count; i++)
            {
                var column = new GridViewColumn();
                var headerBinding = new Binding
                {
                    ElementName = "PART_UserControl",
                    Path = new PropertyPath("SwimLaneView.RowHeaders"),
                    Converter = this.indexedItemConverter,
                    ConverterParameter = i
                };

                BindingOperations.SetBinding(column, GridViewColumn.HeaderProperty, headerBinding);
                column.HeaderTemplate = (DataTemplate)control.FindResource("RowHeaderTemplate");

                column.CellTemplate = i == 0
                                          ? control.TryFindResource("ParentCellTemplate") as DataTemplate
                                          : control.TryFindResource(string.Concat("ColumnTemplate", i)) as DataTemplate;

                // If matching template found; add column.
                if (column.CellTemplate != null)
                {
                    control.PART_MainGridView.Columns.Add(column);
                }
            }

            control.PART_OtherItemsListView.ItemsSource = new[] { control.SwimLaneView };

            control.PART_OtherItemsGridView.Columns.Add(new GridViewColumn
            {
                Header = Properties.Resources.String020,
                CellTemplate = (DataTemplate)control.FindResource("OrphanCellTemplate")
            });

            for (var i = 0; i < control.SwimLaneView.BucketStates.Count(); i++)
            {
                var column = new GridViewColumn
                {
                    Header = control.SwimLaneView.BucketStates[i].State,
                    CellTemplate = control.TryFindResource(string.Concat("BucketCellTemplate", i)) as DataTemplate
                };

                if (column.CellTemplate != null)
                {
                    control.PART_OtherItemsGridView.Columns.Add(column);
                }
            }
        }

        /// <summary>
        /// Resets the content of the layout and.
        /// </summary>
        private void ResetLayoutAndContent()
        {
            this.SetSortFields();
            this.BuildGridView();
            this.ResizeColumnsToFit();
            this.viewControl.PART_ParentSelector.SelectedIndex = 0;
        }
    }
}
