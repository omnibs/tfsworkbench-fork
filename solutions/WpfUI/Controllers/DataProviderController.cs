// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataProviderController.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the DataProviderHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.Controllers
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows;

    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.UIElements;
    using TfsWorkbench.WpfUI.ProjectSelector;
    using TfsWorkbench.WpfUI.Properties;

    /// <summary>
    /// The Data provider helper
    /// </summary>
    internal class DataProviderController : ProjectDataServiceConsumer, IDataProviderController
    {
        /// <summary>
        /// The application controller instance.
        /// </summary>
        private readonly IApplicationController controller;

        /// <summary>
        /// Gets the data provider.
        /// </summary>
        private readonly IDataProvider dataProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProviderController"/> class.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        /// <param name="controller">The controller.</param>
        public DataProviderController(IDataProvider dataProvider, IApplicationController controller)
        {
            if (dataProvider == null)
            {
                throw new ArgumentNullException("dataProvider");
            }

            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            this.dataProvider = dataProvider;
            this.controller = controller;

            this.dataProvider.ElementDataLoaded += this.OnDataLoaded;
            this.dataProvider.ElementSaveComplete += this.OnDataSaveComplete;
            this.dataProvider.ElementSaveError += this.OnSaveError;
            this.dataProvider.LayoutLoadError += this.OnLayoutLoadError;
            this.dataProvider.LayoutSaveError += this.LayoutSaveError;
            this.dataProvider.ElementDataLoadError += this.OnDataLoadError;
        }

        /// <summary>
        /// Clears the control cache.
        /// </summary>
        public void ClearControlCache()
        {
            this.dataProvider.ClearControlItemGroup();
        }

        /// <summary>
        /// Resets the project layout.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <returns>The reset project data.</returns>
        public IProjectData ResetProjectLayout(IProjectData projectData)
        {
            if (projectData == null)
            {
                throw new ArgumentNullException("projectData");
            }

            // Capture the current project details
            var projectCollectionUrl = projectData.ProjectCollectionUrl;
            var projectName = projectData.ProjectName;
            var iterationPath = projectData.ProjectIterationPath;
            var areaPath = projectData.ProjectAreaPath;

            Uri projectCollectionUri;
            if (!this.TryGetProjectUri(projectCollectionUrl, out projectCollectionUri))
            {
                return projectData;
            }

            // Delete the existing layout data
            this.DeleteExistingProjectLayoutFile(projectCollectionUri, projectName);

            // Release the project resources
            this.ProjectDataService.ClearAllCurrentProjectData();

            // Load the default layout.
            projectData = this.dataProvider.GetProjectLayout(projectCollectionUri, projectName);

            projectData.ProjectAreaPath = areaPath;
            projectData.ProjectIterationPath = iterationPath;

            // Start the refresh event
            this.dataProvider.BeginRefreshAllProjectData(projectData);

            return projectData;
        }

        /// <summary>
        /// Resets the project data.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <returns>The reset project data.</returns>
        public IProjectData RefreshProjectData(IProjectData projectData)
        {
            if (projectData == null)
            {
                throw new ArgumentNullException("projectData");
            }

            // Capture the current project details
            var projectCollectionUrl = projectData.ProjectCollectionUrl;

            Uri projectCollectionUri;
            if (!this.TryGetProjectUri(projectCollectionUrl, out projectCollectionUri))
            {
                return projectData;
            }

            // Start the refresh event
            this.dataProvider.BeginRefreshAllProjectData(projectData);

            return projectData;
        }

        /// <summary>
        /// Begins the save project data event.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        public void BeginSaveProjectData(IProjectData projectData)
        {
            this.dataProvider.BeginSaveProjectData(projectData);
        }

        /// <summary>
        /// Generates the project load control.
        /// </summary>
        /// <returns>A new instacne of the project load control.</returns>
        public FrameworkElement GenerateProjectLoadControl()
        {
            return new ProjectSelectorView { DataContext = new ProjectSelectorViewModel() };
        }

        /// <summary>
        /// Gets the control items.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>The associated control item collection.</returns>
        public IControlItemGroup GetControlItems(IWorkbenchItem workbenchItem)
        {
            return this.dataProvider.GetControlItemGroup(workbenchItem);
        }

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

            this.dataProvider.ElementDataLoaded -= this.OnDataLoaded;
            this.dataProvider.ElementSaveComplete -= this.OnDataSaveComplete;
            this.dataProvider.ElementSaveError -= this.OnSaveError;
            this.dataProvider.LayoutLoadError -= this.OnLayoutLoadError;
            this.dataProvider.ElementDataLoadError -= this.OnDataLoadError;
        }

        /// <summary>
        /// Deletes the existing project layout file.
        /// </summary>
        /// <param name="projectCollectionUri">The project collection URI.</param>
        /// <param name="projectName">Name of the project.</param>
        private void DeleteExistingProjectLayoutFile(Uri projectCollectionUri, string projectName)
        {
            var projectLayoutPath = this.ProjectDataService.DefaultFilePath(projectCollectionUri, projectName);
            if (!File.Exists(projectLayoutPath))
            {
                return;
            }
            
            try
            {
                File.Delete(projectLayoutPath);
            }
            catch (Exception ex)
            {
                CommandLibrary.ApplicationExceptionCommand.Execute(ex, this.controller.MainWindow);
            }
        }

        /// <summary>
        /// Tries the get project URI.
        /// </summary>
        /// <param name="projectUrlCandidate">The project URL candidate.</param>
        /// <param name="projectUri">The project URI.</param>
        /// <returns><c>True</c> if the sepecified candidate is valid; otherwise <c>false</c>.</returns>
        private bool TryGetProjectUri(string projectUrlCandidate, out Uri projectUri)
        {
            if (!Uri.TryCreate(projectUrlCandidate, UriKind.Absolute, out projectUri))
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.String033,
                    projectUrlCandidate);

                this.controller.SetStatusMessage(message);
            }

            return projectUri != null;
        }

        /// <summary>
        /// Called when [save error].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="WorkbenchItemSaveFailedEventArgs"/> instance containing the event data.</param>
        private void OnSaveError(object sender, WorkbenchItemSaveFailedEventArgs e)
        {
            this.controller.WorkbenchItemSaveError(e.WorkbenchItem, e.Errors);
        }

        /// <summary>
        /// Layouts the load error.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.MessageEventArgs"/> instance containing the event data.</param>
        private void OnLayoutLoadError(object sender, MessageEventArgs e)
        {
            MessageBox.Show(
                e.Context, Resources.String034, MessageBoxButton.OK, MessageBoxImage.Exclamation);

            this.controller.SetStatusMessage(Resources.String035);
        }

        /// <summary>
        /// Called when [data load error].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ExceptionEventArgs"/> instance containing the event data.</param>
        private void OnDataLoadError(object sender, ExceptionEventArgs e)
        {
            CommandLibrary.ApplicationExceptionCommand.Execute(e.Context, this.controller.MainWindow);
        }

        /// <summary>
        /// Layouts the save error.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.MessageEventArgs"/> instance containing the event data.</param>
        private void LayoutSaveError(object sender, MessageEventArgs e)
        {
            this.controller.SetStatusMessage(string.Concat(Resources.String036, e.Context));
            this.controller.EnableInput(true);
        }

        /// <summary>
        /// Called when [save complete].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnDataSaveComplete(object sender, EventArgs e)
        {
            this.controller.SetStatusMessage(Resources.String037);
            this.controller.EnableInput(true);
        }

        /// <summary>
        /// Called when [data loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ProjectDataEventArgs"/> instance containing the event data.</param>
        private void OnDataLoaded(object sender, ProjectDataEventArgs e)
        {
            this.controller.ApplyLoadedData(e.Context);
        }
    }
}
