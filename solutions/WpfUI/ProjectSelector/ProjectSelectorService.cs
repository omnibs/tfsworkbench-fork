// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectSelectorService.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the Service type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.ProjectSelector
{
    using System;
    using System.ComponentModel;

    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.WpfUI.Properties;

    using Settings = TfsWorkbench.Core.Properties.Settings;

    /// <summary>
    /// The project selector service.
    /// </summary>
    internal class ProjectSelectorService : ProjectDataServiceConsumer, IProjectSelectorService
    {
        /// <summary>
        /// The call back action for the data loaded event;
        /// </summary>
        private Action dataLoadedCallBack;

        /// <summary>
        /// The volume checking loader factory method.
        /// </summary>
        private Func<IProjectData, ILoaderWithVolumeCheck> volumeCheckLoaderFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectSelectorService"/> class.
        /// </summary>
        public ProjectSelectorService()
        {
            if (this.ProjectDataService.CurrentDataProvider == null)
            {
                throw new ArgumentException(Resources.String065);
            }

            this.DataProvider = this.ProjectDataService.CurrentDataProvider;
        }

        /// <summary>
        /// Occurs when [async exception].
        /// </summary>
        public event EventHandler<ExceptionEventArgs> AsyncException;

        /// <summary>
        /// Gets the data provider.
        /// </summary>
        /// <value>The data provider.</value>
        public IDataProvider DataProvider { get; private set; }

        /// <summary>
        /// Gets or sets the volume check loader factory.
        /// </summary>
        /// <value>The volume check loader factory.</value>
        public Func<IProjectData, ILoaderWithVolumeCheck> VolumeCheckLoaderFactory
        {
            get
            {
                return this.volumeCheckLoaderFactory ?? (pd => new LoaderWithVolumeCheck(pd, this));
            }

            set
            {
                this.volumeCheckLoaderFactory = value;
            }
        }

        /// <summary>
        /// Gets the project data service.
        /// </summary>
        /// <value>The project data service.</value>
        internal new IProjectDataService ProjectDataService
        {
            get
            {
                return base.ProjectDataService;
            }
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <returns>A new instance of the project selector service type.</returns>
        public static IProjectSelectorService CreateInstance()
        {
            return new ProjectSelectorService();
        }

        /// <summary>
        /// Ensures the project nodes loaded.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="callBack">The call back.</param>
        public void BeginEnsureNodesLoaded(IProjectData projectData, Action callBack)
        {
            if (projectData == null)
            {
                throw new ArgumentNullException("projectData");
            }

            if (callBack == null)
            {
                throw new ArgumentNullException("callBack");
            }

            if (!IsMissingProjectNodes(projectData))
            {
                callBack();
            }
            else
            {
                this.LoadNodesOnBackgroundThread(projectData, callBack);
            }
        }

        /// <summary>
        /// Shows the project selector.
        /// </summary>
        /// <returns><c>Null</c> if no project selected; otherwise and <c>IProjectData</c> instance.</returns>
        public IProjectData ShowProjectSelector()
        {
            return this.DataProvider.ShowProjectSelector();
        }

        /// <summary>
        /// Begins the volume check.
        /// </summary>
        /// <param name="projectData">The project criteria.</param>
        /// <param name="callBack">The call back method. Invoked with the work item volume as parameter.</param>
        public void BeginVolumeCheck(IProjectData projectData, Action<int> callBack)
        {
            if (projectData == null)
            {
                throw new ArgumentNullException("projectData");
            }

            if (callBack == null)
            {
                throw new ArgumentNullException("callBack");
            }

            this.RunOnBackgroundThread(
                e => e.Result = this.GetItemCount(projectData), 
                e => callBack((int)e.Result));
        }

        /// <summary>
        /// Begins the load project from data provider routine.
        /// </summary>
        /// <param name="projectData">The project criteria.</param>
        /// <param name="callBack">The call back.</param>
        public void BeginLoad(IProjectData projectData, Action callBack)
        {
            if (projectData == null)
            {
                throw new ArgumentNullException("projectData");
            }

            if (callBack == null)
            {
                throw new ArgumentNullException("callBack");
            }

            this.dataLoadedCallBack = callBack;

            this.AttachDataProviderListeners();

            if (this.IsProjectGlobal(projectData))
            {
                this.BeginRefresh(projectData);
            }
            else
            {
                this.BeginLoad(projectData);
            }
        }

        /// <summary>
        /// Gets the last loaded project data.
        /// </summary>
        /// <returns>Instance of project data.</returns>
        public IProjectData GetLastProjectData()
        {
            try
            {
                return this.HasPreviousProjectLayout() ? this.LoadPreviousProjectLayout() : null;
            }
            catch (Exception ex)
            {
                this.OnAsyncError(new Exception(Resources.String052, ex));
            }

            return null;
        }

        /// <summary>
        /// Creates the loader.
        /// </summary>
        /// <param name="projectData">The project criteria.</param>
        /// <returns>An instance of the loader class.</returns>
        public ILoaderWithVolumeCheck CreateLoader(IProjectData projectData)
        {
            return this.VolumeCheckLoaderFactory(projectData);
        }

        /// <summary>
        /// Determines whether [the specified project data] [has project nodes].
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <returns>
        /// <c>true</c> if [the specified project data] [has project nodes]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasProjectNodes(IProjectData projectData)
        {
            return !IsMissingProjectNodes(projectData);
        }

        /// <summary>
        /// Determines whether [the specified project data] [is missing project nodes].
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <returns>
        /// <c>true</c> if [the specified project data] [is missing project nodes]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsMissingProjectNodes(IProjectData projectData)
        {
            IProjectNode dummyNode;

            return 
                projectData.ProjectNodes == null ||
                !projectData.ProjectNodes.TryGetValue(Settings.Default.AreaPathFieldName, out dummyNode) ||
                !projectData.ProjectNodes.TryGetValue(Settings.Default.IterationPathFieldName, out dummyNode);
        }

        /// <summary>
        /// Determines whether [the specified project data] [is global].
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <returns>
        /// <c>true</c> if [the specified project data] [is global]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsProjectGlobal(IProjectData projectData)
        {
            return projectData == this.ProjectDataService.CurrentProjectData;
        }

        /// <summary>
        /// Determines whether [has previous project layout].
        /// </summary>
        /// <returns>
        /// <c>true</c> if [has previous project layout]; otherwise, <c>false</c>.
        /// </returns>
        private bool HasPreviousProjectLayout()
        {
            return !string.IsNullOrEmpty(this.DataProvider.LastProjectName);
        }

        /// <summary>
        /// Loads the previous project layout.
        /// </summary>
        /// <returns>The previous project layout data loaded from file.</returns>
        private IProjectData LoadPreviousProjectLayout()
        {
            return this.ProjectDataService.LoadProjectLayoutData(this.GetPreviousDataFilePath());
        }

        /// <summary>
        /// Gets the previous data file path.
        /// </summary>
        /// <returns><c>Null</c> is no previous settings found; otherwise previous data file path.</returns>
        private string GetPreviousDataFilePath()
        {
            return this.ProjectDataService.DefaultFilePath(new Uri(this.DataProvider.LastProjectCollectionUrl), this.DataProvider.LastProjectName);
        }

        /// <summary>
        /// Begins the project data refresh.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        private void BeginRefresh(IProjectData projectData)
        {
            this.DataProvider.BeginRefreshAllProjectData(projectData);
        }

        /// <summary>
        /// Begins the load project data process.
        /// </summary>
        /// <param name="projectData">The project criteria.</param>
        private void BeginLoad(IProjectData projectData)
        {
            this.DataProvider.BeginLoadProjectData(
                this.BuildFilter(projectData),
                projectData);
        }

        /// <summary>
        /// Attaches the data provider listeners.
        /// </summary>
        private void AttachDataProviderListeners()
        {
            this.DataProvider.ElementDataLoaded += this.OnElementDataLoaded;
            this.DataProvider.ElementDataLoadError += this.OnElementDataLoadError;
        }

        /// <summary>
        /// Detaches the data provider listeners.
        /// </summary>
        private void DetachDataProviderListeners()
        {
            this.DataProvider.ElementDataLoaded -= this.OnElementDataLoaded;
            this.DataProvider.ElementDataLoadError -= this.OnElementDataLoadError;
        }

        /// <summary>
        /// Called when [element data load error].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.ExceptionEventArgs"/> instance containing the event data.</param>
        private void OnElementDataLoadError(object sender, ExceptionEventArgs e)
        {
            this.DetachDataProviderListeners();
            this.ClearDataLoadedCallBack();
            this.OnAsyncError(e.Context);
        }

        /// <summary>
        /// Called when [element data loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.ProjectDataEventArgs"/> instance containing the event data.</param>
        private void OnElementDataLoaded(object sender, ProjectDataEventArgs e)
        {
            this.DetachDataProviderListeners();
            this.SetPreviousProjectCriteria(e.Context);
            this.dataLoadedCallBack();
            this.ClearDataLoadedCallBack();
        }

        /// <summary>
        /// Sets the previous project criteria.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        private void SetPreviousProjectCriteria(IProjectData projectData)
        {
            this.DataProvider.LastProjectCollectionUrl = projectData.ProjectCollectionUrl;
            this.DataProvider.LastProjectName = projectData.ProjectName;
        }

        /// <summary>
        /// Clears the data loaded call back.
        /// </summary>
        private void ClearDataLoadedCallBack()
        {
            this.dataLoadedCallBack = null;
        }

        /// <summary>
        /// Runs the specfied actions on a background thread.
        /// </summary>
        /// <param name="doWork">The do work action.</param>
        /// <param name="runWorkerComplete">The run worker complete action.</param>
        private void RunOnBackgroundThread(Action<DoWorkEventArgs> doWork, Action<RunWorkerCompletedEventArgs> runWorkerComplete)
        {
            using (var bgw = new BackgroundWorker())
            {
                bgw.DoWork += (s, e) => doWork(e);
                bgw.RunWorkerCompleted += (s, e) =>
                {
                    if (e.Error != null)
                    {
                        this.OnAsyncError(e.Error);
                    }
                    else
                    {
                        runWorkerComplete(e);
                    }
                };

                bgw.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Gets the item count.
        /// </summary>
        /// <param name="projectData">The project criteria.</param>
        /// <returns>The number of items that match the selector criteria.</returns>
        private int GetItemCount(IProjectData projectData)
        {
            return this.DataProvider.GetItemCount(this.BuildFilter(projectData), projectData);
        }

        /// <summary>
        /// Builds the filter.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <returns>The fitler string.</returns>
        private string BuildFilter(IProjectData projectData)
        {
            return this.ProjectDataService.GeneratePathFilter(projectData.ProjectIterationPath, projectData.ProjectAreaPath);
        }

        /// <summary>
        /// Loads the nodes on background thread.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="callBack">The call back.</param>
        private void LoadNodesOnBackgroundThread(IProjectData projectData, Action callBack)
        {
            this.RunOnBackgroundThread(
                e => this.DataProvider.LoadProjectNodes(projectData),
                e => callBack());
        }

        /// <summary>
        /// Called when [async error].
        /// </summary>
        /// <param name="error">The error.</param>
        private void OnAsyncError(Exception error)
        {
            if (this.AsyncException != null)
            {
                this.AsyncException(this, new ExceptionEventArgs(error));
            }
        }
    }
}
