// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditItemControlv2.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for EditItemControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    using Core.Interfaces;

    using TfsWorkbench.Core.Helpers;

    using UIElements;

    /// <summary>
    /// Interaction logic for EditItemControl.xaml
    /// </summary>
    public partial class EditItemControlv2
    {
        /// <summary>
        /// The workbench item property.
        /// </summary>
        private static readonly DependencyProperty workbenchItemProperty = DependencyProperty.Register(
            "WorkbenchItem", 
            typeof(IWorkbenchItem), 
            typeof(EditItemControlv2),
            new PropertyMetadata(null, OnWorkbenchItemChanged));

        /// <summary>
        /// The project data property.
        /// </summary>
        private static readonly DependencyProperty projectDataProperty = DependencyProperty.Register(
            "ProjectData", typeof(IProjectData), typeof(EditItemControlv2));

        /// <summary>
        /// The dataProvider dependency property.
        /// </summary>
        private static readonly DependencyProperty dataProviderProperty = DependencyProperty.Register(
            "DataProvider",
            typeof(IDataProvider),
            typeof(EditItemControlv2));

        /// <summary>
        /// The initial state of the workbench item.
        /// </summary>
        private string initialWorkbenchItemState;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditItemControlv2"/> class.
        /// </summary>
        public EditItemControlv2()
        {
            this.InitializeComponent();
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
        /// Gets the project data property.
        /// </summary>
        /// <value>The project data property.</value>
        public static DependencyProperty ProjectDataProperty
        {
            get { return projectDataProperty; }
        }

        /// <summary>
        /// Gets the DataProvider property.
        /// </summary>
        /// <value>The name property.</value>
        public static DependencyProperty DataProviderProperty
        {
            get { return dataProviderProperty; }
        }

        /// <summary>
        /// Gets or sets the instance DataProvider.
        /// </summary>
        /// <returns>The instance DataProvider.</returns>
        public IDataProvider DataProvider
        {
            get { return (IDataProvider)this.GetValue(DataProviderProperty); }
            set { this.SetValue(DataProviderProperty, value); }
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
        /// Gets or sets the control items.
        /// </summary>
        /// <value>The control items.</value>
        public IProjectData ProjectData
        {
            get { return (IProjectData)this.GetValue(ProjectDataProperty); }
            set { this.SetValue(ProjectDataProperty, value); }
        }

        /// <summary>
        /// Set the workbench item via a dispatcher job.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <remarks>Use this method to delay the setting of the workbench item and improve win form control rendering performance.</remarks>
        public void DelaySetWorkbenchItem(IWorkbenchItem workbenchItem)
        {
            this.Dispatcher.BeginInvoke(
                DispatcherPriority.ApplicationIdle, new Action(() => this.WorkbenchItem = workbenchItem));
        }

        /// <summary>
        /// Called when [workbench item changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWorkbenchItemChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as EditItemControlv2;
            if (control == null || control.WorkbenchItem == null)
            {
                return;
            }

            control.initialWorkbenchItemState = control.WorkbenchItem.GetState();
            control.PART_ContentGrid.Children.Clear();
            control.PART_ContentGrid.Children.Add(
                (UIElement)control.DataProvider.GetWorkItemEditPanel(control.WorkbenchItem));
        }

        /// <summary>
        /// Handles the Click event of the CloseButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.CloseDialog();
        }

        /// <summary>
        /// Closes the dialog.
        /// </summary>
        private void CloseDialog()
        {
            this.WorkbenchItem.OnPropertyChanged();
            var finalWorkbenchItemState = this.WorkbenchItem.GetState();
            if (finalWorkbenchItemState != this.initialWorkbenchItemState)
            {
                var itemStateChangeEventArgs = new Core.EventArgObjects.ItemStateChangeEventArgs(
                    this.WorkbenchItem, 
                    this.initialWorkbenchItemState, 
                    finalWorkbenchItemState);

                this.ProjectData.WorkbenchItems.OnItemStateChanged(this, itemStateChangeEventArgs);
            }

            this.ReleaseReferencedObjects();
            CommandLibrary.CloseDialogCommand.Execute(this, Application.Current.MainWindow);
        }

        /// <summary>
        /// Releases the control collection.
        /// </summary>
        private void ReleaseReferencedObjects()
        {
            var disposables = this.GetAllDisposableChildren();

            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }

            PART_ContentGrid.Children.Clear();

            this.WorkbenchItem = null;
            this.DataProvider = null;
            this.ProjectData = null;
        }

        /// <summary>
        /// Called when [save button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.WorkbenchItem == null || !this.WorkbenchItem.ValueProvider.IsDirty)
            {
                return;
            }

            CommandLibrary.SaveItemCommand.Execute(this.WorkbenchItem, this);
        }

        /// <summary>
        /// Called when [refresh button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnRefreshButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.WorkbenchItem == null)
            {
                return;
            }

            CommandLibrary.RefreshItemCommand.Execute(this.WorkbenchItem, this);
        }

        /// <summary>
        /// Called when [discard button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnDiscardButtonClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(Properties.Resources.String049, Properties.Resources.String050, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            CommandLibrary.DiscardItemCommand.Execute(this.WorkbenchItem, this);

            this.CloseDialog();
        }
    }
}