// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemList.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for ItemList.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ItemListUI 
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Core.DataObjects;
    using Core.Helpers;
    using Core.Interfaces;
    using Core.Properties;

    using Helpers;

    using TfsWorkbench.Core.EventArgObjects;

    using UIElements;

    /// <summary>
    /// Interaction logic for ItemList.xaml
    /// </summary>
    public partial class ItemList : IDisposable
    {
        /// <summary>
        /// The WorkbenchItemTypeName dependency property.
        /// </summary>
        private static readonly DependencyProperty workbenchItemTypeNameProperty = DependencyProperty.Register(
            "WorkbenchItemTypeName",
            typeof(string),
            typeof(ItemList));

        /// <summary>
        /// The states dependency property.
        /// </summary>
        private static readonly DependencyProperty statesProperty = DependencyProperty.Register(
            "States",
            typeof(Collection<SelectedValue>),
            typeof(ItemList));

        /// <summary>
        /// The users dependency property.
        /// </summary>
        private static readonly DependencyProperty usersProperty = DependencyProperty.Register(
            "Users",
            typeof(ObservableCollection<SelectedValue>),
            typeof(ItemList));

        /// <summary>
        /// The drag event handler instance.
        /// </summary>
        private readonly DragDeltaEventHandler dragEventHandler = ThumbDragDelta;

        /// <summary>
        /// The list sorter.
        /// </summary>
        private readonly ListSorter listSorter = new ListSorter();

        /// <summary>
        /// The column to field name map.
        /// </summary>
        private readonly Dictionary<GridViewColumn, string> columnFieldMap = new Dictionary<GridViewColumn, string>();

        /// <summary>
        /// The multi select helper.
        /// </summary>
        private readonly MultiSelectHelper multiSelectHelper;

        /// <summary>
        /// The initialisation required flag.
        /// </summary>
        private bool initialisationRequired;

        /// <summary>
        /// The data provider;
        /// </summary>
        private IDataProvider dataProvider;

        /// <summary>
        /// The item list helper.
        /// </summary>
        private ItemListHelper itemListHelper;

        /// <summary>
        /// The project data field.
        /// </summary>
        private IProjectData projectData;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemList"/> class.
        /// </summary>
        public ItemList()
        {
            this.ControlItemGroups = new SortableObservableCollection<MultiSelectControlItemGroup>();
            this.States = new Collection<SelectedValue>();
            this.Users = new ObservableCollection<SelectedValue>();

            this.SetDefaultSortOrder();

            InitializeComponent();

            this.PART_MainListView.AddHandler(Thumb.DragDeltaEvent, this.dragEventHandler, true);

            this.multiSelectHelper = new MultiSelectHelper(this);

            var descriptor = DependencyPropertyDescriptor.FromProperty(IsVisibleProperty, typeof(ItemList));
            descriptor.AddValueChanged(this, this.OnVisibilityChanged);
        }

        /// <summary>
        /// Gets the workbench item type name property.
        /// </summary>
        /// <value>The workbench item type name property.</value>
        public static DependencyProperty WorkbenchItemTypeNameProperty
        {
            get { return workbenchItemTypeNameProperty; }
        }

        /// <summary>
        /// Gets the states property.
        /// </summary>
        /// <value>The states property.</value>
        public static DependencyProperty StatesProperty
        {
            get { return statesProperty; }
        }

        /// <summary>
        /// Gets the users property.
        /// </summary>
        /// <value>The users property.</value>
        public static DependencyProperty UsersProperty
        {
            get { return usersProperty; }
        }

        /// <summary>
        /// Gets or sets the data provider.
        /// </summary>
        /// <value>The data provider.</value>
        public IDataProvider DataProvider
        {
            get
            {
                return this.dataProvider;
            }

            set
            {
                if (this.dataProvider == value)
                {
                    return;
                }

                this.dataProvider = value;

                if (value == null)
                {
                    return;
                }

                this.itemListHelper = new ItemListHelper(this);
            }
        }

        /// <summary>
        /// Gets or sets the project data.
        /// </summary>
        /// <value>The project data.</value>
        public IProjectData ProjectData
        {
            get
            {
                return this.projectData;
            }

            set
            {
                if (this.projectData == value)
                {
                    return;
                }

                if (this.projectData != null)
                {
                    this.projectData.WorkbenchItems.CollectionChanged -= this.OnCollectionChanged;
                }

                this.ClearAllProjectDataResources();

                this.projectData = value;

                if (this.projectData != null)
                {
                    this.projectData.WorkbenchItems.CollectionChanged += this.OnCollectionChanged;
                }

                if (this.IsVisible)
                {
                    this.InitialiseList();
                }
                else
                {
                    this.initialisationRequired = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the states.
        /// </summary>
        /// <value>The states.</value>
        public Collection<SelectedValue> States
        {
            get
            {
                return (Collection<SelectedValue>)this.GetValue(StatesProperty);
            }

            set
            {
                this.SetValue(StatesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>The users.</value>
        public ObservableCollection<SelectedValue> Users
        {
            get { return (ObservableCollection<SelectedValue>)this.GetValue(UsersProperty); }
            set { this.SetValue(UsersProperty, value); }
        }

        /// <summary>
        /// Gets or sets the name of the workbench item type.
        /// </summary>
        /// <value>The name of the workbench item type.</value>
        public string WorkbenchItemTypeName
        {
            get
            {
                return (string)this.GetValue(WorkbenchItemTypeNameProperty);
            }

            set
            {
                this.SetValue(WorkbenchItemTypeNameProperty, value);
            }
        }

        /// <summary>
        /// Gets the control item collections.
        /// </summary>
        /// <value>The control item collections.</value>
        public SortableObservableCollection<MultiSelectControlItemGroup> ControlItemGroups { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool isDisposing)
        {
            if (!isDisposing)
            {
                return;
            }

            this.ClearAllProjectDataResources();

            var descriptor = DependencyPropertyDescriptor.FromProperty(IsVisibleProperty, typeof(ItemList));
            descriptor.RemoveValueChanged(this, this.OnVisibilityChanged);

            this.ProjectData = null;
            this.DataProvider = null;
        }

        /// <summary>
        /// Handles the DragDelta event of the Thumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private static void ThumbDragDelta(object sender, RoutedEventArgs e)
        {
            const double MaxColumnWidth = 1000;
            const double MinColumnWidth = 100;

            var senderAsThumb = e.OriginalSource as Thumb;

            if (senderAsThumb == null)
            {
                return;
            }

            var header = senderAsThumb.TemplatedParent as GridViewColumnHeader;

            if (header == null)
            {
                return;
            }

            if (header.Column.ActualWidth < MinColumnWidth)
            {
                header.Column.Width = MinColumnWidth;
            }

            if (header.Column.ActualWidth > MaxColumnWidth)
            {
                header.Column.Width = MaxColumnWidth;
            }
        }

        /// <summary>
        /// Sets the default sort order.
        /// </summary>
        private void SetDefaultSortOrder()
        {
            this.listSorter.FieldName = Settings.Default.IdFieldName;
        }

        /// <summary>
        /// Creates the states collection.
        /// </summary>
        private void CreateStatesCollection()
        {
            if (this.ProjectData == null || string.IsNullOrEmpty(this.WorkbenchItemTypeName))
            {
                return;
            }

            var workbenchItemTypes = this.projectData.ItemTypes[this.WorkbenchItemTypeName];

            foreach (var state in workbenchItemTypes.States)
            {
                this.States.Add(
                    new SelectedValue
                        {
                            IsSelected = !state.Equals(Settings.Default.ExclusionState), 
                            Text = state
                        });
            }
        }

        /// <summary>
        /// Clears all project data resources.
        /// </summary>
        private void ClearAllProjectDataResources()
        {
            this.multiSelectHelper.ClearAllSelections();
            this.States.Clear();
            this.PART_MainGridView.Columns.Clear();
            this.columnFieldMap.Clear();
            this.itemListHelper.RemoveAllAssociatedCollections();
            this.ControlItemGroups.Clear();
        }

        /// <summary>
        /// Called when [visibility changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnVisibilityChanged(object sender, EventArgs e)
        {
            if (!this.IsVisible || !this.initialisationRequired)
            {
                return;
            }

            this.initialisationRequired = false;
            this.InitialiseList();
        }

        /// <summary>
        /// Initialises the list.
        /// </summary>
        private void InitialiseList()
        {
            CommandLibrary.DisableUserInputCommand.Execute(true, this);
            CommandLibrary.ApplicationMessageCommand.Execute("Loading list. Please wait...", this);

            Action callback = () =>
                {
                    this.CreateStatesCollection();
                    this.BuildGridViewColumns();
                    this.BuildUserList();
                    this.PopulateList();

                    CommandLibrary.ApplicationMessageCommand.Execute("List loaded.", this);
                    CommandLibrary.DisableUserInputCommand.Execute(false, this);
                };

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, callback);
        }

        /// <summary>
        /// Populates the list.
        /// </summary>
        private void PopulateList()
        {
            if (this.projectData == null)
            {
                return;
            }

            var workbenchItems = this.ProjectData.WorkbenchItems
                .Where(this.itemListHelper.IsCorrectType)
                .ToArray();

            this.itemListHelper.AddAssociatedCollection(workbenchItems.Where(this.itemListHelper.IsIncluded).ToArray());
            this.itemListHelper.RemoveAssociatedCollection(workbenchItems.Where(w => !this.itemListHelper.IsIncluded(w)).ToArray());

            this.SortList();
        }

        /// <summary>
        /// Loads the users.
        /// </summary>
        private void BuildUserList()
        {
            this.Users.Clear();

            var ownerFieldName = WorkbenchItemHelper.GetOwnerFieldName(this.WorkbenchItemTypeName);

            if (this.projectData == null || string.IsNullOrEmpty(ownerFieldName))
            {
                this.PART_OwnerFilterSelector.Visibility = Visibility.Collapsed;
                return;
            }

            // Get a list of matching work items.
            var workbenchItems =
                this.projectData.WorkbenchItems
                    .Where(w => Equals(w.GetTypeName(), this.WorkbenchItemTypeName));

            if (!workbenchItems.Any())
            {
                return;
            }

            var valueList = new List<object>();

            // Add the unassigned indicator.
            this.Users.Add(new SelectedValue { Text = ItemListHelper.UnassignedIndicator, IsSelected = true });

            // Add any allowed values from the first list item.
            var allowedValues = workbenchItems.First().AllowedValues[ownerFieldName] as IEnumerable<object>;

            if (allowedValues != null)
            {
                valueList.AddRange(allowedValues.Where(v => v != null));
            }

            // Add any existing clean values
            foreach (var valueCandidate in
                workbenchItems
                    .Where(w => !w.ValueProvider.IsDirty)
                    .Select(workbenchItem => workbenchItem.GetOwner())
                    .Where(owner => !valueList.Contains(owner)))
            {
                valueList.Add(valueCandidate);
            }

            foreach (var user in valueList.Select(v => v.ToString())
                .Where(v => !string.IsNullOrEmpty(v))
                .OrderBy(v => v)
                .Select(v => new SelectedValue { Text = v, IsSelected = true }))
            {
                this.Users.Add(user);
            }
        }

        /// <summary>
        /// Builds the grid view column list.
        /// </summary>
        private void BuildGridViewColumns()
        {
            if (this.projectData == null || this.DataProvider == null || string.IsNullOrEmpty(this.WorkbenchItemTypeName))
            {
                return;
            }

            // Get an unassociated control collection
            var controlItems = this.DataProvider.GetControlItemGroup(this.ProjectData, this.WorkbenchItemTypeName);

            if (controlItems == null)
            {
                return;
            }

            var i = 0;

            foreach (var controlItem in controlItems.ControlItems)
            {
                var column = new GridViewColumn
                    {
                        Header = controlItem.DisplayText,
                        CellTemplate = this.FindResource(string.Concat("CellTemplate", i)) as DataTemplate
                    };

                this.columnFieldMap.Add(column, controlItem.FieldName);

                this.PART_MainGridView.Columns.Add(column);

                i++;

                if (i > 30)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Called when [collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.RepositoryChangedEventArgs&lt;IWorkbenchItem&gt;"/> instance containing the event data.</param>
        private void OnCollectionChanged(object sender, RepositoryChangedEventArgs<IWorkbenchItem> e)
        {
            if (this.Dispatcher == null)
            {
                return;
            }

            Action execute = () =>
            {
                switch (e.Action)
                {
                    case ChangeActionOption.Add:
                        foreach (var item in e.Context.Where(this.itemListHelper.IsIncluded)) 
                            {
                                this.itemListHelper.AddAssociatedCollection(item);
                            }

                        this.BuildUserList();

                        break;
                    case ChangeActionOption.Remove:
                        foreach (var item in e.Context)
                        {
                            this.itemListHelper.RemoveAssociatedCollection(item);
                        }

                        break;
                    case ChangeActionOption.Clear:
                        this.ControlItemGroups.Clear();

                        break;
                    case ChangeActionOption.Refresh:
                        this.ControlItemGroups.Clear();
                        this.PopulateList();

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
        /// Handles the Click event of the RefreshAll control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RefreshAllClick(object sender, RoutedEventArgs e)
        {
            this.multiSelectHelper.ClearAllSelections();

            var workbenchItems = this.ControlItemGroups.Select(cic => cic.WorkbenchItem).ToArray();

            SendOrPostCallback callback = delegate
                {
                    IWorkbenchItem activeItem = null;
                    try
                    {
                        foreach (var workbenchItem in workbenchItems)
                        {
                            activeItem = workbenchItem;
                            CommandLibrary.RefreshItemCommand.Execute(workbenchItem, this);
                            this.RefreshBindings(workbenchItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        var activeItemDescription = activeItem == null
                                                        ? "None"
                                                        : string.Concat(
                                                            activeItem.GetId(),
                                                            " - ",
                                                            activeItem.GetState());

                        var wrappedException = new ApplicationException(string.Concat("An error occured during the refresh process. Active item: ", activeItemDescription), ex);

                        if (!CommandLibrary.ApplicationExceptionCommand.CanExecute(wrappedException, this))
                        {
                            throw;
                        }

                        CommandLibrary.ApplicationExceptionCommand.Execute(wrappedException, this);
                    }
                    finally
                    {
                        CommandLibrary.DisableUserInputCommand.Execute(false, this);
                    }
                };

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, callback, null);

            CommandLibrary.DisableUserInputCommand.Execute(true, this);
        }

        /// <summary>
        /// Refreshes the bindings.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        private void RefreshBindings(IWorkbenchItem workbenchItem)
        {
            var ControlItemGroup =
                this.ControlItemGroups.FirstOrDefault(cic => cic.WorkbenchItem.Equals(workbenchItem));

            var container = this.PART_MainListView.ItemContainerGenerator.ContainerFromItem(ControlItemGroup);

            if (container == null)
            {
                return;
            }

            var comboBoxes = container.GetAllChildElementsOfType<ComboBox>();

            var bindingExpressions = comboBoxes
                .Select(comboBox => comboBox.GetBindingExpression(ItemsControl.ItemsSourceProperty))
                .Where(binding => binding != null);

            foreach (var binding in bindingExpressions)
            {
                binding.UpdateTarget();
            }
        }

        /// <summary>
        /// Handles the Click event of the SaveAll control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnSaveAllClick(object sender, RoutedEventArgs e)
        {
            var workbenchItems = this.ControlItemGroups.Select(cic => cic.WorkbenchItem).Where(wbi => wbi.ValueProvider.IsDirty).ToArray();

            SendOrPostCallback callback = delegate
            {
                foreach (var workbenchItem in workbenchItems)
                {
                    CommandLibrary.SaveItemCommand.Execute(workbenchItem, this);
                    this.RefreshBindings(workbenchItem);
                }

                CommandLibrary.DisableUserInputCommand.Execute(false, this);
            };

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, callback, null);

            CommandLibrary.DisableUserInputCommand.Execute(true, this);
        }

        /// <summary>
        /// Handles the Click event of the CreateItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCreateItemClick(object sender, RoutedEventArgs e)
        {
            SendOrPostCallback callback = delegate
                {
                    CommandLibrary.CreateParentCommand.Execute(this.WorkbenchItemTypeName, this);
                };

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, callback, null);

            CommandLibrary.DisableUserInputCommand.Execute(true, this);
        }

        /// <summary>
        /// Grids the view column header clicked handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;

            if (headerClicked == null || headerClicked.Role == GridViewColumnHeaderRole.Padding)
            {
                return;
            }

            string fieldName;
            if (!this.columnFieldMap.TryGetValue(headerClicked.Column, out fieldName))
            {
                return;
            }

            var callback = (SendOrPostCallback)delegate
                {
                    this.SortList(fieldName);

                    CommandLibrary.DisableUserInputCommand.Execute(false, this);
                };

            CommandLibrary.DisableUserInputCommand.Execute(true, this);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, callback, null);
        }

        /// <summary>
        /// Sorts the list.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        private void SortList(string fieldName)
        {
            if (Equals(this.listSorter.FieldName, fieldName))
            {
                this.listSorter.Direction = this.listSorter.Direction == SortDirection.Ascending
                                                ? SortDirection.Descending
                                                : SortDirection.Ascending;
            }
            else
            {
                this.listSorter.Direction = SortDirection.Ascending;
            }

            this.listSorter.FieldName = fieldName;

            this.SortList();
        }

        /// <summary>
        /// Sorts the list.
        /// </summary>
        private void SortList()
        {
            this.ControlItemGroups.Sort(this.listSorter);
        }

        /// <summary>
        /// Called when [selected states changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnFiltersChanged(object sender, EventArgs e)
        {
            var callback = (SendOrPostCallback)delegate
            {
                this.PopulateList();

                CommandLibrary.DisableUserInputCommand.Execute(false, this);
            };

            CommandLibrary.DisableUserInputCommand.Execute(true, this);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, callback, null);
        }

        /// <summary>
        /// Handles the OnMouseDoubleClick event of the MainListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnMainListViewOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var source = e.OriginalSource as FrameworkElement;
            if (source == null)
            {
                return;
            }

            var controlItem = source.DataContext as IControlItem;

            if (controlItem != null)
            {
                CommandLibrary.EditItemCommand.Execute(controlItem.WorkbenchItem, this);
                return;
            }

            var ControlItemGroup = source.DataContext as IControlItemGroup;

            if (ControlItemGroup == null)
            {
                return;
            }

            CommandLibrary.EditItemCommand.Execute(ControlItemGroup.WorkbenchItem, this);
            return;
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the MainListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnMainListViewPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;

            if (element == null || !element.IsEnabled)
            {
                return;
            }

            var controlItem = element.DataContext as MultiSelectControlItem;

            if (controlItem == null)
            {
                return;
            }

            e.Handled = this.multiSelectHelper.HandleControlClick(controlItem);
        }
    }
}
