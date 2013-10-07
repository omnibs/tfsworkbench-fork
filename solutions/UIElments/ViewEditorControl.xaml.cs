// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewEditorControl.xaml.cs" company="EMC Consulting">
//   EMC Consulting 2009
// </copyright>
// <summary>
//   Interaction logic for ViewEditorControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Emcc.TeamSystem.TaskBoard.UIElements
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Core.DataObjects;
    using Core.Helpers;
    using Core.Interfaces;

    /// <summary>
    /// Interaction logic for ViewEditorControl.xaml
    /// </summary>
    public partial class ViewEditorControl : UserControl
    {
        /// <summary>
        /// The View Property development.
        /// </summary>
        private static readonly DependencyProperty viewProperty = DependencyProperty.Register(
            "View",
            typeof(IView),
            typeof(ViewEditorControl),
            new PropertyMetadata(null, OnSourcePropertyChanged));

        /// <summary>
        /// The View Property development.
        /// </summary>
        private static readonly DependencyProperty projectDataProperty = DependencyProperty.Register(
            "ProjectData",
            typeof(IProjectData),
            typeof(ViewEditorControl),
            new PropertyMetadata(null, OnSourcePropertyChanged));

        /// <summary>
        /// The current child type applied to this control.
        /// </summary>
        private string appliedChildType;

        /// <summary>
        /// The current parent type applied to this view
        /// </summary>
        private string appliedparentType;

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
        public static DependencyProperty ViewProperty
        {
            get { return viewProperty; }
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
        public IView View
        {
            get { return (IView)this.GetValue(ViewProperty); }
            set { this.SetValue(ViewProperty, value); }
        }

        /// <summary>
        /// Called when [source property changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSourcePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as ViewEditorControl;

            if (control == null || control.View == null || control.ProjectData == null)
            {
                return;
            }

            control.SetupChildStates();
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
        /// Setups the child states.
        /// </summary>
        private void SetupChildStates()
        {
            this.appliedChildType = this.View.ViewMap.ChildType;
            this.appliedparentType = this.View.ViewMap.ParentType;

            this.BucketStates.Items.Clear();
            this.SwimLaneStates.Items.Clear();
            this.UnassignedStates.Items.Clear();

            foreach (var state in this.View.ViewMap.BucketStates)
            {
                this.BucketStates.Items.Add(state);
            }

            foreach (var state in this.View.ViewMap.SwimLaneStates)
            {
                this.SwimLaneStates.Items.Add(state);
            }

            foreach (var state in this.ProjectData.ItemTypes[this.View.ViewMap.ChildType].States)
            {
                if (!this.View.ViewMap.BucketStates.Contains(state) && !this.View.ViewMap.SwimLaneStates.Contains(state))
                {
                    this.UnassignedStates.Items.Add(state);
                }
            }
        }

        /// <summary>
        /// Syncs the states.
        /// </summary>
        private void SyncStates()
        {
            this.View.ViewMap.SwimLaneStates.Clear();
            this.View.ViewMap.BucketStates.Clear();

            foreach (var state in this.SwimLaneStates.Items.OfType<string>())
            {
                this.View.ViewMap.SwimLaneStates.Add(state);
            }

            foreach (var state in this.BucketStates.Items.OfType<string>())
            {
                this.View.ViewMap.BucketStates.Add(state);
            }
        }

        /// <summary>
        /// Handles the Click event of the ToSwimLane control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ToSwimLane_Click(object sender, RoutedEventArgs e)
        {
            MoveItems(this.UnassignedStates, this.SwimLaneStates);
            this.SyncStates();
        }

        /// <summary>
        /// Handles the Click event of the FromSwimLine control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void FromSwimLine_Click(object sender, RoutedEventArgs e)
        {
            MoveItems(this.SwimLaneStates, this.UnassignedStates);
            this.SyncStates();
        }

        /// <summary>
        /// Handles the Click event of the ToBucket control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ToBucket_Click(object sender, RoutedEventArgs e)
        {
            MoveItems(this.UnassignedStates, this.BucketStates);
            this.SyncStates();
        }

        /// <summary>
        /// Handles the Click event of the FromBucket control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void FromBucket_Click(object sender, RoutedEventArgs e)
        {
            MoveItems(this.BucketStates, this.UnassignedStates);
            this.SyncStates();
        }

        /// <summary>
        /// Handles the Click event of the Close control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.View.InitialiseLayout(this.View.ViewMap);
            ProjectDataHelper.RefreshView(this.ProjectData, this.View);
            CommandLibrary.CloseDialog.Execute(this, Application.Current.MainWindow);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ParentType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ParentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var parentTypeName = this.ParentType.SelectedValue as string;
            if (parentTypeName == null || this.appliedparentType.Equals(parentTypeName))
            {
                return;
            }

            this.View.ViewMap.ParentStates.Clear();

            foreach (var state in this.ProjectData.ItemTypes[parentTypeName].States)
            {
                var isSelected = !state.Equals("Deleted");
                this.View.ViewMap.ParentStates.Add(new SelectedValue { IsSelected = isSelected, Value = state });
            }

            this.appliedparentType = parentTypeName;
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ChildType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ChildType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var childTypeName = this.ChildType.SelectedValue as string;
            if (childTypeName == null || this.appliedChildType.Equals(childTypeName))
            {
                return;
            }

            this.View.ViewMap.SwimLaneStates.Clear();
            this.View.ViewMap.BucketStates.Clear();

            foreach (var state in this.ProjectData.ItemTypes[childTypeName].States)
            {
                if (state.Equals("Deleted"))
                {
                    this.View.ViewMap.BucketStates.Add(state);
                }
                else
                {
                    this.View.ViewMap.SwimLaneStates.Add(state);
                }
            }

            this.SetupChildStates();
        }

        /// <summary>
        /// Handles the Click event of the Delete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            CommandLibrary.DeleteView.Execute(this.View, Application.Current.MainWindow);

            if (!this.ProjectData.Views.Contains(this.View))
            {
                CommandLibrary.CloseDialog.Execute(this, Application.Current.MainWindow);
            }
        }
    }
}