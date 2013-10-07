// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainController.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the MainController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI
{
    using System;
    using System.Threading;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Core.EventArgObjects;
    using Core.Interfaces;

    using DataObjects;

    using Helpers;

    using Properties;

    using TfsWorkbench.Core.Services;

    using UIElements;

    /// <summary>
    /// Initializes instance of MainController
    /// </summary>
    internal class MainController
    {
        /// <summary>
        /// The project setup control.
        /// </summary>
        private readonly DisplayMode displayMode;

        /// <summary>
        /// The project data service.
        /// </summary>
        private readonly IProjectDataService projectDataService;

        /// <summary>
        /// The cloase dialog callback.
        /// </summary>
        private SendOrPostCallback closeDialogCallBack;

        /// <summary>
        /// The refresh all callback.
        /// </summary>
        private SendOrPostCallback refreshAllCallBack;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainController"/> class.
        /// </summary>
        /// <param name="displayMode">The project setup control.</param>
        public MainController(DisplayMode displayMode)
            : this(displayMode, ServiceManager.Instance.GetService<IProjectDataService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainController"/> class.
        /// </summary>
        /// <param name="displayMode">The project setup control.</param>
        /// <param name="projectDataService">The project data service.</param>
        public MainController(DisplayMode displayMode, IProjectDataService projectDataService)
        {
            if (projectDataService == null)
            {
                throw new ArgumentNullException("projectDataService");
            }

            this.displayMode = displayMode;
            this.projectDataService = projectDataService;
            this.SetupCommandBindings();

            this.projectDataService.ProjectDataChanged += this.OnProjectDataChanged;
        }

        /// <summary>
        /// Shows the advanced setup dialog.
        /// </summary>
        /// <typeparam name="T">The setup dialog type.</typeparam>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ShowSetupDialog<T>(object sender, ExecutedRoutedEventArgs e) where T : SetupDialogBase, new()
        {
            IDataProvider dataProvider;
            IProjectData projectData;
            if (!this.TryGetGlobalElelements(out dataProvider, out projectData))
            {
                return;
            }

            var projectSetup = new ProjectSetup(projectData.ProjectName)
            {
                ProjectNode = projectData.ProjectNodes[Core.Properties.Settings.Default.IterationPathFieldName],
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.Date.AddDays(6 * Settings.Default.DefaultWorkStreamCadance)
            };

            projectSetup = SetupControllerHelper.AddRelease(projectSetup);

            projectSetup = SetupControllerHelper.AddWorkStream(projectSetup);

            projectSetup = SetupControllerHelper.AddTeam(projectSetup);

            var setupDialog = new T { ProjectSetup = projectSetup };

            setupDialog.ApplySetup += this.OnApplySetup;

            CommandLibrary.ShowDialogCommand.Execute(setupDialog, this.displayMode);
        }

        /// <summary>
        /// Determines whether this instance [can execute setup] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanExecuteSetup(object sender, CanExecuteRoutedEventArgs e)
        {
            var projectData = this.projectDataService.CurrentProjectData;
            e.CanExecute = SetupControllerHelper.IsValidScrumProject(projectData) && SetupControllerHelper.HasRootPathLoaded(projectData);
        }

        /// <summary>
        /// Sets up the command bindings.
        /// </summary>
        private void SetupCommandBindings()
        {
            this.displayMode.CommandBindings.Add(
                new CommandBinding(
                    LocalCommandLibrary.ShowAdvancedSetupCommand,
                    this.ShowSetupDialog<AdvancedSetupControl>,
                    this.CanExecuteSetup));

            this.displayMode.CommandBindings.Add(
                new CommandBinding(
                    LocalCommandLibrary.ShowQuickSetupCommand,
                    this.ShowSetupDialog<QuickStartControl>,
                    this.CanExecuteSetup));
        }

        /// <summary>
        /// Tries the get global elelements.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        /// <param name="projectData">The project data.</param>
        /// <returns><c>True</c> if the elements are populated; otherwise <c>false</c>.l</returns>
        private bool TryGetGlobalElelements(out IDataProvider dataProvider, out IProjectData projectData)
        {
            dataProvider = null;
            projectData = null;

            if (this.displayMode == null)
            {
                return false;
            }

            projectData = this.projectDataService.CurrentProjectData;
            dataProvider = this.projectDataService.CurrentDataProvider;

            return projectData != null && dataProvider != null;
        }

        /// <summary>
        /// Called when [project data changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.ProjectDataChangedEventArgs"/> instance containing the event data.</param>
        private void OnProjectDataChanged(object sender, ProjectDataChangedEventArgs e)
        {
            this.displayMode.ProjectVisualiser.ProjectData = e.NewValue;
            this.refreshAllCallBack = delegate { };
            this.closeDialogCallBack = delegate { };
        }

        /// <summary>
        /// Called when [apply advanced setup].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnApplySetup(object sender, ProjectSetupEventArgs e)
        {
            IDataProvider dataProvider;
            IProjectData projectData;
            if (!this.TryGetGlobalElelements(out dataProvider, out projectData))
            {
                CommandLibrary.ApplicationExceptionCommand.Execute(
                    new Exception("Apply setup method has failed. Unable to find global elements"), this.displayMode);

                return;
            }

            var setupDialog = sender as SetupDialogBase;
            if (setupDialog == null)
            {
                return;
            }

            var projectSetup = e.ProjectSetup;

            // Build the new work item structure
            var releases = SetupControllerHelper.CreateProjectStructure(projectSetup);

            try
            {
                // Commit the new paths to the data store.
                // This invalidates any loaded work items that reference altered paths.
                dataProvider.SaveProjectNodes(projectData);
            }
            catch (Exception ex)
            {
                if (CommandLibrary.ApplicationExceptionCommand.CanExecute(ex, this.displayMode))
                {
                    CommandLibrary.ApplicationExceptionCommand.Execute(ex, this.displayMode);
                    setupDialog.IsEnabled = true;
                    return;
                }

                throw;
            }

            // Clear the existing work item data
            SetupControllerHelper.ClearExistingItems(projectData);

            this.refreshAllCallBack = delegate
                {
                    dataProvider.BeginRefreshAllProjectData(projectData);
                };

            this.closeDialogCallBack = delegate
                {
                    SetupControllerHelper.AddNewStructureItems(releases, dataProvider, projectData);

                    foreach (var viewMap in projectData.ViewMaps)
                    {
                        viewMap.OnLayoutUpdated();
                    }

                    this.displayMode.ProjectVisualiser.ProjectData = null;
                    this.displayMode.ProjectVisualiser.ProjectData = projectData;

                    dataProvider.ElementDataLoadError -= this.OnDataLoadError;
                    dataProvider.ElementSaveComplete -= this.OnSaveComplete;
                    dataProvider.ElementDataLoaded -= this.OnDataLoaded;
                    dataProvider.ElementSaveError -= this.OnSaveError;

                    setupDialog.ApplySetup -= this.OnApplySetup;

                    CommandLibrary.CloseDialogCommand.Execute(setupDialog, setupDialog);
                };

            dataProvider.ElementDataLoadError += this.OnDataLoadError;
            dataProvider.ElementSaveComplete += this.OnSaveComplete;
            dataProvider.ElementDataLoaded += this.OnDataLoaded;
            dataProvider.ElementSaveError += this.OnSaveError;

            // Begin the async save process
            dataProvider.BeginSaveProjectData(projectData);
        }

        /// <summary>
        /// Called when [save error].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="WorkbenchItemSaveFailedEventArgs"/> instance containing the event data.</param>
        private void OnSaveError(object sender, WorkbenchItemSaveFailedEventArgs e)
        {
            this.DispatchMethod(this.closeDialogCallBack);
        }

        /// <summary>
        /// Called when [data loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ProjectDataEventArgs"/> instance containing the event data.</param>
        private void OnDataLoaded(object sender, EventArgs e)
        {
            this.DispatchMethod(this.closeDialogCallBack);
        }

        /// <summary>
        /// Called when [save complete].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnSaveComplete(object sender, EventArgs e)
        {
            this.DispatchMethod(this.refreshAllCallBack);
        }

        /// <summary>
        /// Called when [data load error].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ExceptionEventArgs"/> instance containing the event data.</param>
        private void OnDataLoadError(object sender, ExceptionEventArgs e)
        {
            this.DispatchMethod(this.closeDialogCallBack);
        }

        /// <summary>
        /// Dispatches the method.
        /// </summary>
        /// <param name="callback">The callback.</param>
        private void DispatchMethod(SendOrPostCallback callback)
        {
            this.displayMode.Dispatcher.BeginInvoke(DispatcherPriority.Background, callback, null);
        }
    }
}
