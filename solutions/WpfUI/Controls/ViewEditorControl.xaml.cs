// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewEditorControl.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for ViewEditorControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Core.DataObjects;
    using Core.Helpers;
    using Core.Interfaces;

    using UIElements;
    using System.Collections.Generic;

    /// <summary>
    /// Enumeration used for when reordering items in a list box
    /// </summary>
    enum MoveDirection
    {
        Up,
        Down
    }

    /// <summary>
    /// Interaction logic for ViewEditorControl.xaml
    /// </summary>
    public partial class ViewEditorControl
    {
        /// <summary>
        /// The View Property development.
        /// </summary>
        private static readonly DependencyProperty viewMapProperty = DependencyProperty.Register(
            "ViewMap",
            typeof(ViewMap),
            typeof(ViewEditorControl),
            new PropertyMetadata(null, OnViewMapPropertyChanged));

        /// <summary>
        /// The View Property development.
        /// </summary>
        private static readonly DependencyProperty projectDataProperty = DependencyProperty.Register(
            "ProjectData",
            typeof(IProjectData),
            typeof(ViewEditorControl),
            new PropertyMetadata(null, OnViewMapPropertyChanged));

        /// <summary>
        /// The value selections property.
        /// </summary>
        private static readonly DependencyProperty parentTypeSelectionsProperty = DependencyProperty.Register(
            "ParentTypeSelections", 
            typeof(Collection<SelectedValue>),
            typeof(ViewEditorControl),
            new PropertyMetadata(new Collection<SelectedValue>()));

        /// <summary>
        /// The current child type applied to this control.
        /// </summary>
        private string appliedChildType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewEditorControl"/> class.
        /// </summary>
        public ViewEditorControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the view property.
        /// </summary>
        /// <value>The view property.</value>
        public static DependencyProperty ViewMapProperty
        {
            get { return viewMapProperty; }
        }

        /// <summary>
        /// Gets the project data property.
        /// </summary>
        /// <value>The project data property.</value>
        public static DependencyProperty ProjectDataProperty
        {
            get { return projectDataProperty; }
        }

        /// <summary>
        /// Gets the value selections property.
        /// </summary>
        /// <value>The value selections property.</value>
        public static DependencyProperty ParentTypeSelectionsProperty
        {
            get
            {
                return parentTypeSelectionsProperty;
            }
        }

        /// <summary>
        /// Gets or sets the project data.
        /// </summary>
        /// <value>The project data.</value>
        public IProjectData ProjectData
        {
            get { return (IProjectData)this.GetValue(ProjectDataProperty); }
            set { this.SetValue(ProjectDataProperty, value); }
        }

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>The view object.</value>
        public ViewMap ViewMap
        {
            get { return (ViewMap)this.GetValue(ViewMapProperty); }
            set { this.SetValue(ViewMapProperty, value); }
        }

        /// <summary>
        /// Gets or sets the parent type selections.
        /// </summary>
        /// <value>The parent type selections.</value>
        public Collection<SelectedValue> ParentTypeSelections
        {
            get { return (Collection<SelectedValue>)this.GetValue(ParentTypeSelectionsProperty); }
            set { this.SetValue(ParentTypeSelectionsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the save project layout.
        /// </summary>
        /// <value>The save project layout.</value>
        public Action SaveProjectLayout { get; set; }

        /// <summary>
        /// Setups the child states.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="viewMap">The view map.</param>
        /// <param name="childTypeName">Name of the child type.</param>
        /// <returns>The view map including the child states.</returns>
        public static ViewMap SetupChildStates(IProjectData projectData, ViewMap viewMap, string childTypeName)
        {
            if (projectData == null)
            {
                throw new ArgumentNullException("projectData");
            }

            if (viewMap == null)
            {
                throw new ArgumentNullException("viewMap");
            }

            if (childTypeName == null)
            {
                throw new ArgumentNullException("childTypeName");
            }

            viewMap.SwimLaneStates.Clear();
            viewMap.BucketStates.Clear();
            viewMap.StateItemColours.Clear();

            var childItemType = projectData.ItemTypes[childTypeName];
            if (childItemType != null)
            {
                foreach (var state in childItemType.States)
                {
                    if (state.Equals(Core.Properties.Settings.Default.ExclusionState))
                    {
                        viewMap.BucketStates.Add(state);
                    }
                    else
                    {
                        viewMap.SwimLaneStates.Add(state);
                    }

                    viewMap.StateItemColours.Add(Factory.BuildStateColour(state));
                }
            }

            return viewMap;
        }

        /// <summary>
        /// Called when [source property changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnViewMapPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as ViewEditorControl;

            if (control == null || control.ViewMap == null || control.ProjectData == null)
            {
                return;
            }

            control.ApplyViewStates();
            control.SetupParentTypes();
        }

        /// <summary>
        /// Moves the items.
        /// </summary>
        /// <param name="from">the source list.</param>
        /// <param name="to">The target list.</param>
        private static void MoveItems(ListBox from, ListBox to)
        {
            var selectedItems = from.SelectedItems.OfType<string>().ToArray();

            foreach (var item in selectedItems)
            {
                if (!to.Items.Contains(item))
                {
                    to.Items.Add(item);
                }

                to.SelectedItem = item;

                from.Items.Remove(item);
            }
        }

        /// <summary>
        /// Moves an item within a listbox.
        /// </summary>
        /// <param name="from">The listbox to move within</param>
        /// <param name="direction">The direction to move in</param>
        private static void MoveItem(ListBox from, MoveDirection direction)
        {
            if (from.SelectedIndex != -1)
            {
                int newIndex = MoveItem(from.Items, from.SelectedIndex, direction);
                from.SelectedItem = from.Items[newIndex];
            }
        }

        /// <summary>
        /// Moves an indexed item within a collection in the direction specified
        /// </summary>
        /// <param name="collection">The collection</param>
        /// <param name="indexToMove">The index of the item to move</param>
        /// <param name="direction">The direction to move in</param>
        /// <returns>The new index of the item</returns>
        private static int MoveItem(ItemCollection collection, int indexToMove, MoveDirection direction)
        {
            if (indexToMove >= 0 && indexToMove < collection.Count)
            {
                int indexToSwapWith = GetIndexToSwapWith(collection.Count, indexToMove, direction);

                if (indexToSwapWith != -1)
                {
                    object tmp = collection[indexToMove];
                    collection.RemoveAt(indexToMove);
                    collection.Insert(indexToSwapWith, tmp);
                    return indexToSwapWith;
                }
            }
            return indexToMove;
        }

        /// <summary>
        /// Calculates an index to swap with
        /// </summary>
        /// <param name="itemCount">The item count</param>
        /// <param name="indexToMove">The index to move</param>
        /// <param name="direction">The direction to move in</param>
        /// <returns>Index of new item position</returns>
        private static int GetIndexToSwapWith(int itemCount, int indexToMove, MoveDirection direction)
        {
            int indexToSwapWith = -1;

            switch (direction)
            {
                case MoveDirection.Up:
                    if (indexToMove > 0)
                    {
                        indexToSwapWith = indexToMove - 1;
                    }
                    break;
                case MoveDirection.Down:
                    if (indexToMove < itemCount - 1)
                    {
                        indexToSwapWith = indexToMove + 1;
                    }
                    break;
            }

            return indexToSwapWith;
        }

        /// <summary>
        /// Sets the selected parent types.
        /// </summary>
        private void SetupParentTypes()
        {
            this.ParentTypeSelections.Clear();

            foreach (var typeName in this.ProjectData.ItemTypes.Select(it => it.TypeName).OrderBy(tn => tn))
            {
                this.ParentTypeSelections.Add(
                    new SelectedValue { Text = typeName, IsSelected = this.ViewMap.ParentTypes.Contains(typeName) });
            }
        }

        /// <summary>
        /// Syncs the states.
        /// </summary>
        private void SyncStates()
        {
            this.ViewMap.SwimLaneStates.Clear();
            this.ViewMap.BucketStates.Clear();

            foreach (var state in this.SwimLaneStates.Items.OfType<string>())
            {
                this.ViewMap.SwimLaneStates.Add(state);
            }

            foreach (var state in this.BucketStates.Items.OfType<string>())
            {
                this.ViewMap.BucketStates.Add(state);
            }
        }

        /// <summary>
        /// Setups the child states.
        /// </summary>
        private void ApplyViewStates()
        {
            this.appliedChildType = this.ViewMap.ChildType;

            var itemTypeData = this.ProjectData.ItemTypes[this.ViewMap.ChildType];
            if (itemTypeData == null)
            {
                return;
            }

            Func<string, bool> isValidChildState = state => itemTypeData.States.Contains(state);

            this.BucketStates.Items.Clear();
            this.SwimLaneStates.Items.Clear();
            this.UnassignedStates.Items.Clear();

            foreach (var state in this.ViewMap.BucketStates.Where(isValidChildState))
            {
                this.BucketStates.Items.Add(state);
            }

            foreach (var state in this.ViewMap.SwimLaneStates.Where(isValidChildState))
            {
                this.SwimLaneStates.Items.Add(state);
            }

            foreach (var state in
                itemTypeData.States.Where(state => !this.ViewMap.BucketStates.Contains(state) && !this.ViewMap.SwimLaneStates.Contains(state)))
            {
                this.UnassignedStates.Items.Add(state);
            }

            var binding = this.PART_ColorList.GetBindingExpression(ItemsControl.ItemsSourceProperty);
            if (binding != null)
            {
                binding.UpdateTarget();
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ChildType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnChildTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var childTypeName = this.ChildType.SelectedValue as string;
            if (childTypeName == null || Equals(this.appliedChildType, childTypeName))
            {
                return;
            }

            SetupChildStates(this.ProjectData, this.ViewMap, childTypeName);

            this.ApplyViewStates();
        }

        /// <summary>
        /// Handles the Click event of the ToSwimLane control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ToSwimLaneClick(object sender, RoutedEventArgs e)
        {
            MoveItems(this.UnassignedStates, this.SwimLaneStates);
            this.SyncStates();
        }

        /// <summary>
        /// Handles the Click event of the FromSwimLine control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void FromSwimLineClick(object sender, RoutedEventArgs e)
        {
            MoveItems(this.SwimLaneStates, this.UnassignedStates);
            this.SyncStates();
        }

        /// <summary>
        /// Handles the Click event of the ToBucket control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ToBucketClick(object sender, RoutedEventArgs e)
        {
            MoveItems(this.UnassignedStates, this.BucketStates);
            this.SyncStates();
        }

        /// <summary>
        /// Handles the Click event of the FromBucket control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void FromBucketClick(object sender, RoutedEventArgs e)
        {
            MoveItems(this.BucketStates, this.UnassignedStates);
            this.SyncStates();
        }

        /// <summary>
        /// Handles the Click event of the Close control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            if (!this.ViewMap.ParentTypes.Any())
            {
                MessageBox.Show(Properties.Resources.String047, Properties.Resources.String048, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            this.ViewMap.OnLayoutUpdated();

            CommandLibrary.CloseDialogCommand.Execute(this, Application.Current.MainWindow);

            if (this.SaveProjectLayout != null)
            {
                this.SaveProjectLayout();
            }
        }

        /// <summary>
        /// Handles the Click event of the Delete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            CommandLibrary.DeleteViewCommand.Execute(this.ViewMap, Application.Current.MainWindow);

            if (!this.ProjectData.ViewMaps.Contains(this.ViewMap))
            {
                CommandLibrary.CloseDialogCommand.Execute(this, Application.Current.MainWindow);
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ColourSelector control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ColourSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            if (comboBox == null)
            {
                return;
            }

            var stateColour = comboBox.DataContext as StateColour;
            if (stateColour == null)
            {
                return;
            }

            var selectedItem = comboBox.SelectedItem as ComboBoxItem;

            if (selectedItem == null)
            {
                return;
            }

            stateColour.ColourAsString = selectedItem.Content as string;
        }

        /// <summary>
        /// Called when [check change].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCheckChange(object sender, RoutedEventArgs e)
        {
            this.ViewMap.ParentTypes.Clear();

            foreach (var parentType in this.ParentTypeSelections.Where(pts => pts.IsSelected).Select(pts => pts.Text))
            {
                this.ViewMap.ParentTypes.Add(parentType);
            }
        }

        /// <summary>
        /// Handles the click event for the swim lane up button
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SwimLaneUp(object sender, RoutedEventArgs e)
        {
            MoveItem(this.SwimLaneStates, MoveDirection.Up);
            this.SyncStates();
        }

        /// <summary>
        /// Handles the click event for the swim lane down button
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SwimLaneDown(object sender, RoutedEventArgs e)
        {
            MoveItem(this.SwimLaneStates, MoveDirection.Down);
            this.SyncStates();
        }
    }
}