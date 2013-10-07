// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeVisualiser.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for NodeVisualiser.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI.NodeVisualisation
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Core.Interfaces;

    using DataObjects;

    using TfsWorkbench.Core.Services;
    using TfsWorkbench.UIElements.PopupControls;

    using UIElements;

    /// <summary>
    /// Interaction logic for NodeVisualiser.xaml
    /// </summary>
    public partial class NodeVisualiser
    {
        /// <summary>
        /// The controller.
        /// </summary>
        private readonly NodeVisualiserController controller;

        /// <summary>
        /// The project data property.
        /// </summary>
        private static readonly DependencyProperty projectDataProperty = DependencyProperty.Register(
            "ProjectData",
            typeof(IProjectData),
            typeof(NodeVisualiser),
            new PropertyMetadata(null, OnProjectDataChanged));

        /// <summary>
        /// The project data property.
        /// </summary>
        private static readonly DependencyProperty dataProviderProperty = DependencyProperty.Register(
            "DataProvider",
            typeof(IDataProvider),
            typeof(NodeVisualiser));

        /// <summary>
        /// The node filter property.
        /// </summary>
        private static readonly DependencyProperty nodeFilterProperty = DependencyProperty.Register(
            "NodeFilter",
            typeof(string),
            typeof(NodeVisualiser),
            new PropertyMetadata(null, OnNodeFilterChanged));

        /// <summary>
        /// The project filter node property.
        /// </summary>
        private static readonly DependencyProperty projectFilterNodeProperty = DependencyProperty.Register(
            "RootNode",
            typeof(IProjectNode),
            typeof(NodeVisualiser));

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeVisualiser"/> class.
        /// </summary>
        public NodeVisualiser()
            : this(ServiceManager.Instance.GetService<IProjectDataService>().CurrentDataProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeVisualiser"/> class.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        public NodeVisualiser(IDataProvider dataProvider)
        {
            if (dataProvider == null)
            {
                throw new ArgumentNullException("dataProvider");
            }

            this.InitializeComponent();

            this.controller = new NodeVisualiserController(this);
            this.DataProvider = dataProvider;
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
        /// Gets the node filter property.
        /// </summary>
        /// <value>The node filter property.</value>
        public static DependencyProperty NodeFilterProperty
        {
            get { return nodeFilterProperty; }
        }

        /// <summary>
        /// Gets the data provider property.
        /// </summary>
        /// <value>The data provider property.</value>
        public static DependencyProperty DataProviderProperty
        {
            get { return dataProviderProperty; }
        }

        /// <summary>
        /// Gets the project filter node property.
        /// </summary>
        /// <value>The project filter node property.</value>
        public static DependencyProperty ProjectFilterNodeProperty
        {
            get { return projectFilterNodeProperty; }
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
        /// Gets or sets the data provider.
        /// </summary>
        /// <value>The data provider.</value>
        public IDataProvider DataProvider
        {
            get { return (IDataProvider)this.GetValue(DataProviderProperty); } 
            set { this.SetValue(DataProviderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the node filter.
        /// </summary>
        /// <value>The node filter.</value>
        public string NodeFilter
        {
            get { return (string)this.GetValue(NodeFilterProperty); }
            set { this.SetValue(NodeFilterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the project filter node.
        /// </summary>
        /// <value>The project filter node.</value>
        public IProjectNode ProjectFilterNode
        {
            get { return (IProjectNode)this.GetValue(ProjectFilterNodeProperty); }
            set { this.SetValue(ProjectFilterNodeProperty, value); }
        }

        /// <summary>
        /// Gets the root node.
        /// </summary>
        /// <value>The root node.</value>
        internal ProjectNodeVisual RootNode { get; private set; }

        /// <summary>
        /// Refreshes the tree view bindings.
        /// </summary>
        internal void RefreshTreeViewBindings()
        {
            var bindingExpression = this.PART_NodeFilterControl.GetBindingExpression(ComboBoxTreeView.RootNodeProperty);
            if (bindingExpression != null)
            {
                bindingExpression.UpdateTarget();
            }
        }

        /// <summary>
        /// Resets the layout.
        /// </summary>
        internal void ResetLayout()
        {
            if (this.RootNode != null)
            {
                NodeLayoutHelper.ClearVisuals(this.PART_LayoutCanvas, this.RootNode);
            }
            
            var callBack = (SendOrPostCallback)delegate
                {
                    try
                    {
                        this.controller.VisualiseProjectLayout();
                        this.PART_LayoutScroller.Focus();
                    }
                    catch (Exception ex)
                    {
                        var wrappedException =
                            new ApplicationException("An error occured while resetting the Project Setup layout", ex);

                        if (!CommandLibrary.ApplicationExceptionCommand.CanExecute(wrappedException, this))
                        {
                            throw;
                        }

                        CommandLibrary.ApplicationExceptionCommand.Execute(wrappedException, this);
                    }
                };

            this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, callBack, null);
        }

        /// <summary>
        /// Called when [node filter changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnNodeFilterChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as NodeVisualiser;
            if (control == null)
            {
                return;
            }

            CommandLibrary.DisableUserInputCommand.Execute(true, control);

            var callback = (SendOrPostCallback)delegate
                {
                    control.controller.VisualiseProjectLayout();
                    CommandLibrary.DisableUserInputCommand.Execute(false, control);
                };

            control.Dispatcher.BeginInvoke(DispatcherPriority.Background, callback, null);
        }

        /// <summary>
        /// Called when [project data changed].
        /// </summary>
        /// <param name="dependencyObject">The dependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnProjectDataChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as NodeVisualiser;
            if (control == null)
            {
                return;
            }

            var oldProject = e.OldValue as IProjectData;
            var newProject = e.NewValue as IProjectData;

            if (oldProject != null)
            {
                // Release the event handlers
                oldProject.WorkbenchItems.CollectionChanged -= control.controller.OnCollectionchanged;
            }

            if (newProject != null)
            {
                // Wire up the event handlers
                newProject.WorkbenchItems.CollectionChanged += control.controller.OnCollectionchanged;

                // Set the project context values
                control.ProjectFilterNode =
                    newProject.ProjectNodes[Core.Properties.Settings.Default.IterationPathFieldName];
                control.RootNode = new ProjectNodeVisual(control.ProjectFilterNode, null);
                control.NodeFilter = newProject.ProjectIterationPath;
            }
            else
            {
                control.ProjectFilterNode = EmptyProjectNode.CreateInstance();
                control.RootNode = null;
                control.NodeFilter = string.Empty;
            }

            control.RefreshTreeViewBindings();
        }

        /// <summary>
        /// Handles the Click event of the ResetButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ResetButtonClick(object sender, RoutedEventArgs e)
        {
            this.ResetLayout();
        }

        /// <summary>
        /// Handles the PreviewMouseWheel event of the Border control.
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
    }
}