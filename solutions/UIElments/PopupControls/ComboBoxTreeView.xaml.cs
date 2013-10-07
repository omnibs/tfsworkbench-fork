// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComboBoxTreeView.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for ComboBoxTreeView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.PopupControls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Threading;

    using TfsWorkbench.Core.Interfaces;

    /// <summary>
    /// Interaction logic for ComboBoxTreeView.xaml
    /// </summary>
    public partial class ComboBoxTreeView : IPopupControl, IExpandable
    {
        /// <summary>
        /// The root node property.
        /// </summary>
        private static readonly DependencyProperty rootNodeProperty = DependencyProperty.Register(
            "RootNode",
            typeof(IProjectNode),
            typeof(ComboBoxTreeView),
            new PropertyMetadata(null, OnRootNodePropertyChanged));

        /// <summary>
        /// The selected node property.
        /// </summary>
        private static readonly DependencyProperty selectedNodeProperty = DependencyProperty.Register(
            "SelectedNode",
            typeof(IProjectNode),
            typeof(ComboBoxTreeView));

        /// <summary>
        /// The value property.
        /// </summary>
        private static readonly DependencyProperty valueProperty = DependencyProperty.Register(
            "Value",
            typeof(object),
            typeof(ComboBoxTreeView));

        /// <summary>
        /// Flag to indicate is the selection was trigger by a key press.
        /// </summary>
        private bool isKeyPressTriggered;

        /// <summary>
        /// The last expanded nodes.
        /// </summary>
        private IEnumerable<IProjectNode> lastExpandedNodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComboBoxTreeView"/> class.
        /// </summary>
        public ComboBoxTreeView()
        {
            this.InitializeComponent();

            PopupControlHelper.Instance.RegisterPopupControl(this);

            this.PART_MainTreeView.ItemContainerGenerator.StatusChanged += this.OnItemContainerStatusChanged;
        }

        /// <summary>
        /// Gets the root node property.
        /// </summary>
        /// <value>The root node property.</value>
        public static DependencyProperty RootNodeProperty
        {
            get { return rootNodeProperty; }
        }

        /// <summary>
        /// Gets the selected node property.
        /// </summary>
        /// <value>The selected node property.</value>
        public static DependencyProperty SelectedNodeProperty
        {
            get { return selectedNodeProperty; }
        }

        /// <summary>
        /// Gets the value property.
        /// </summary>
        /// <value>The value property.</value>
        public static DependencyProperty ValueProperty
        {
            get { return valueProperty; }
        }

        /// <summary>
        /// Gets or sets the root node.
        /// </summary>
        /// <value>The root node.</value>
        public IProjectNode RootNode
        {
            get { return (IProjectNode)this.GetValue(RootNodeProperty); }
            set { this.SetValue(RootNodeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Selected node.
        /// </summary>
        /// <value>The Selected node.</value>
        public IProjectNode SelectedNode
        {
            get { return (IProjectNode)this.GetValue(SelectedNodeProperty); }
            set { this.SetValue(SelectedNodeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get { return this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpanded
        {
            get
            {
                return this.PART_TreeViewPopup.IsOpen;
            }
        }

        /// <summary>
        /// Expands this instance.
        /// </summary>
        public void Expand()
        {
            this.PART_TreeViewPopup.IsOpen = true;
        }

        /// <summary>
        /// Called when [handle mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void OnHandleMouseDown(object sender, RoutedEventArgs e)
        {
            if (!this.PART_TreeViewPopup.IsOpen)
            {
                return;
            }

            var source = e.OriginalSource as DependencyObject;

            if (source.IsInstanceOrChildOf(this.PART_ToggleButton) || source.IsInstanceOrChildOf(this.PART_MainTreeView))
            {
                return;
            }

            this.PART_TreeViewPopup.IsOpen = false;
        }

        /// <summary>
        /// Called when [root node property changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRootNodePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ComboBoxTreeView;

            if (control == null || control.RootNode == null)
            {
                return;
            }

            control.PART_MainTreeView.ItemsSource = new[] { control.RootNode };
        }

        /// <summary>
        /// Tries the get tree view item.
        /// </summary>
        /// <param name="containerGenerator">The container generator.</param>
        /// <param name="projectNode">The project node.</param>
        /// <param name="treeViewItem">The tree view item.</param>
        /// <returns>
        /// <c>True</c> if the tree view item container is found; otherwise <c>false</c>.
        /// </returns>
        private static bool TryGetTreeViewItem(ItemContainerGenerator containerGenerator, IProjectNode projectNode, out TreeViewItem treeViewItem)
        {
            treeViewItem = containerGenerator.ContainerFromItem(projectNode) as TreeViewItem;

            return treeViewItem != null;
        }

        /// <summary>
        /// Called when [item container status changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnItemContainerStatusChanged(object sender, EventArgs e)
        {
            var containerGenerator = sender as ItemContainerGenerator;
            if (containerGenerator == null || containerGenerator.Status != GeneratorStatus.ContainersGenerated)
            {
                return;
            }

            containerGenerator.StatusChanged -= this.OnItemContainerStatusChanged;

            var selectedPath = this.Value as string;

            if (string.IsNullOrEmpty(selectedPath) || this.RootNode == null)
            {
                return;
            }

            var selectedNode = this.GetSelectedProjectNode(this.RootNode, selectedPath);
            if (selectedNode == null)
            {
                return;
            }

            this.lastExpandedNodes = this.lastExpandedNodes ?? new[] { this.RootNode };

            this.ExpandNextAncestor(containerGenerator, selectedNode);
        }

        /// <summary>
        /// Expands the next ancestor.
        /// </summary>
        /// <param name="containerGenerator">The container generator.</param>
        /// <param name="selectedNode">The selected node.</param>
        private void ExpandNextAncestor(ItemContainerGenerator containerGenerator, IProjectNode selectedNode)
        {
            var projectNode = this.lastExpandedNodes.FirstOrDefault(n => this.IsProjectNodeAncester(selectedNode, n));
            if (projectNode == null)
            {
                return;
            }

            TreeViewItem treeViewItem;
            if (!TryGetTreeViewItem(containerGenerator, projectNode, out treeViewItem))
            {
                return;
            }

            if (projectNode == selectedNode)
            {
                treeViewItem.IsSelected = true;
                return;
            }

            this.lastExpandedNodes = projectNode.Children;

            if (!treeViewItem.IsExpanded)
            {
                treeViewItem.ItemContainerGenerator.StatusChanged += this.OnItemContainerStatusChanged;
                treeViewItem.IsExpanded = true;
            }
        }

        /// <summary>
        /// Gets the selected project node.
        /// </summary>
        /// <param name="projectNode">The project node.</param>
        /// <param name="selectedPath">The selected path.</param>
        /// <returns><c>Null</c> if selected node not found; otherwise <c>Selected node</c>.</returns>
        private IProjectNode GetSelectedProjectNode(IProjectNode projectNode, string selectedPath)
        {
            return projectNode.Path.Equals(selectedPath)
                ? projectNode
                : projectNode.Children
                    .Select(child => this.GetSelectedProjectNode(child, selectedPath))
                    .FirstOrDefault(result => result != null);
        }

        /// <summary>
        /// Determines whether the specified candidate is project node ancester.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        /// <c>true</c> if the specified candidate is ancester; otherwise, <c>false</c>.
        /// </returns>
        private bool IsProjectNodeAncester(IProjectNode target, IProjectNode candidate)
        {
            return Equals(target, candidate) || candidate.Children.Contains(target) || candidate.Children.Any(child => this.IsProjectNodeAncester(target, child));
        }

        /// <summary>
        /// Handles the SelectedItemChanged event of the TreeView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="object"/> instance containing the event data.</param>
        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.SelectedNode = e.NewValue as IProjectNode;

            var newValue = this.SelectedNode == null ? this.RootNode == null ? string.Empty : this.RootNode.Path : this.SelectedNode.Path;

            if (Equals(this.Value, newValue))
            {
                return;
            }

            this.Value = newValue;

            if (!this.isKeyPressTriggered)
            {
                this.Dispatcher.BeginInvoke(
                    DispatcherPriority.ApplicationIdle,
                    new Action(() => this.PART_TreeViewPopup.IsOpen = false));
            }
        }

        /// <summary>
        /// Called when [show popup].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnShowPopup(object sender, RoutedEventArgs e)
        {
            this.PART_MainTreeView.Focus();
            this.isKeyPressTriggered = true;
        }

        /// <summary>
        /// Handles the KeyDown event of the TreeView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void OnTreeViewKeyDown(object sender, KeyEventArgs e)
        {
            this.isKeyPressTriggered = true;

            if (e.Key == Key.Return)
            {
                this.PART_TreeViewPopup.IsOpen = false;
            }
        }

        /// <summary>
        /// Handles the MouseDown event of the TreeView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnTreeViewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.isKeyPressTriggered = false;
        }
    }
}