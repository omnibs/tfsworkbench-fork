// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectSelectorViewModel.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ProjectLoadControlViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.ProjectSelector
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;

    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.UIElements;
    using TfsWorkbench.UIElements.PopupControls;
    using TfsWorkbench.WpfUI.Controllers;
    using TfsWorkbench.WpfUI.Properties;

    using Settings = TfsWorkbench.Core.Properties.Settings;

    /// <summary>
    /// The project load control view model.
    /// </summary>
    public class ProjectSelectorViewModel : IProjectSelectorViewModel
    {
        /// <summary>
        /// The project data service instance.
        /// </summary>
        private readonly IProjectDataService projectDataService;

        /// <summary>
        /// The command element. Used as ui context to application level commands.
        /// </summary>
        private IInputElement commandElement;

        /// <summary>
        /// The is busy flag;
        /// </summary>
        private bool isBusy;

        /// <summary>
        /// The error message.
        /// </summary>
        private string errorMessage;

        /// <summary>
        /// The status message.
        /// </summary>
        private string statusMessage;

        /// <summary>
        /// The project selector loader.
        /// </summary>
        private ILoaderWithVolumeCheck loader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectSelectorViewModel"/> class.
        /// </summary>
        public ProjectSelectorViewModel()
            : this(ServiceManager.Instance.GetService<IProjectDataService>().CurrentProjectData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectSelectorViewModel"/> class.
        /// </summary>
        /// <param name="projectData">The project data instance.</param>
        public ProjectSelectorViewModel(IProjectData projectData)
            : this(projectData, ServiceManager.Instance.GetService<IProjectSelectorService>(), ServiceManager.Instance.GetService<IProjectDataService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectSelectorViewModel"/> class.
        /// </summary>
        /// <param name="projectData">The project data instance.</param>
        /// <param name="projectSelectorService">The project selector service.</param>
        /// <param name="projectDataService">The project data service.</param>
        public ProjectSelectorViewModel(IProjectData projectData, IProjectSelectorService projectSelectorService, IProjectDataService projectDataService)
        {
            if (projectSelectorService == null)
            {
                throw new ArgumentNullException("projectSelectorService");
            }

            if (projectDataService == null)
            {
                throw new ArgumentNullException("projectDataService");
            }

            this.projectDataService = projectDataService;
            this.AttachSelectionService(projectSelectorService);
            this.InitialiseProjectValues(projectData);
            this.GenerateCommands();
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the project selector service.
        /// </summary>
        /// <value>The project selector service.</value>
        public IProjectSelectorService ProjectSelectorService { get; private set; }

        /// <summary>
        /// Gets the project data.
        /// </summary>
        /// <value>The project data.</value>
        public IProjectData ProjectData { get; private set; }

        /// <summary>
        /// Gets the show project selector command.
        /// </summary>
        /// <value>The show project selector command.</value>
        public ICommand ShowProjectSelectorCommand { get; private set; }

        /// <summary>
        /// Gets the load project data command.
        /// </summary>
        /// <value>The load project data command.</value>
        public ICommand LoadProjectDataCommand { get; private set; }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        /// <value>The cancel command.</value>
        public ICommand CancelCommand { get; private set; }

        /// <summary>
        /// Gets the ensure project nodes loaded.
        /// </summary>
        /// <value>The ensure project nodes loaded.</value>
        public ICommand EnsureProjectNodesLoadedCommand { get; private set; }

        /// <summary>
        /// Gets the name of the selected project.
        /// </summary>
        /// <value>The name of the selected project.</value>
        public string SelectedProjectName
        {
            get
            {
                return this.GetProjectValueWithNullCheck(pd => pd.ProjectName);
            }
        }

        /// <summary>
        /// Gets the selected collectionend point.
        /// </summary>
        /// <value>The selected collectionend point.</value>
        public string SelectedCollectionEndPoint
        {
            get
            {
                return this.GetProjectValueWithNullCheck(pd => pd.ProjectCollectionUrl);
            }
        }

        /// <summary>
        /// Gets or sets the iteration path.
        /// </summary>
        /// <value>The iteration path.</value>
        public string IterationPath
        {
            get
            {
                return this.GetProjectValueWithNullCheck(pd => pd.ProjectIterationPath);
            }

            set
            {
                this.SetProjectValueWithNullCheck(pd => pd.ProjectIterationPath = value);

                this.OnPropertyChanged("IterationPath");
            }
        }

        /// <summary>
        /// Gets or sets AreaPath.
        /// </summary>
        public string AreaPath
        {
            get
            {
                return this.GetProjectValueWithNullCheck(pd => pd.ProjectAreaPath);
            }

            set
            {
                this.SetProjectValueWithNullCheck(pd => pd.ProjectAreaPath = value);

                this.OnPropertyChanged("AreaPath");
            }
        }

        /// <summary>
        /// Gets the area root node.
        /// </summary>
        /// <value>The area root node.</value>
        public IProjectNode AreaRootNode
        {
            get
            {
                return this.GetProjectNodeWithNullCheck(Settings.Default.AreaPathFieldName);
            }
        }

        /// <summary>
        /// Gets the iteration root node.
        /// </summary>
        /// <value>The iteration root node.</value>
        public IProjectNode IterationRootNode
        {
            get
            {
                return this.GetProjectNodeWithNullCheck(Settings.Default.IterationPathFieldName);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is busy.
        /// </summary>
        /// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            private set
            {
                this.isBusy = value;

                this.OnPropertyChanged("IsBusy");
            }
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage
        {
            get
            {
                return this.errorMessage;
            }

            private set
            {
                this.errorMessage = value;

                this.OnPropertyChanged("ErrorMessage");
            }
        }

        /// <summary>
        /// Gets the status message.
        /// </summary>
        /// <value>The status message.</value>
        public string StatusMessage
        {
            get
            {
                return this.statusMessage;
            }

            private set
            {
                this.statusMessage = value;

                this.OnPropertyChanged("StatusMessage");
            }
        }

        /// <summary>
        /// Expands the tree view combo box.
        /// </summary>
        /// <param name="expandableControl">The combo box tree view.</param>
        private static void ExpandTreeViewComboBox(IExpandable expandableControl)
        {
            if (expandableControl != null)
            {
                expandableControl.Expand();
            }
        }

        /// <summary>
        /// Indicates if a global project data instance is present.
        /// </summary>
        /// <returns><c>True</c> if a global project data instance is present; otherwise <c>false</c>.</returns>
        private bool GlobalProjectDataInstanceIsPresent()
        {
            return this.projectDataService.CurrentProjectData != null;
        }

        /// <summary>
        /// Connects to selection service.
        /// </summary>
        /// <param name="projectSelectorService">The project selector service.</param>
        private void AttachSelectionService(IProjectSelectorService projectSelectorService)
        {
            this.ProjectSelectorService = projectSelectorService;
            this.ProjectSelectorService.AsyncException += this.OnAsyncException;
        }

        /// <summary>
        /// Called when [async exception].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.ExceptionEventArgs"/> instance containing the event data.</param>
        private void OnAsyncException(object sender, ExceptionEventArgs e)
        {
            this.ClearStatusAndApplicationMessage();
            CommandLibrary.ApplicationExceptionCommand.Execute(e.Context, this.commandElement);
            this.ErrorMessage = e.Context.Message;
            this.IsBusy = false;
            this.DisposeLoader();
        }

        /// <summary>
        /// Generates the commands.
        /// </summary>
        private void GenerateCommands()
        {
            this.ShowProjectSelectorCommand = new RelayCommand(this.OnShowProjectSelector);
            this.LoadProjectDataCommand = new RelayCommand(this.OnExecuteLoadProject, this.CanExecuteLoadProject);
            this.CancelCommand = new RelayCommand(this.OnCancel);
            this.EnsureProjectNodesLoadedCommand = new RelayCommand(this.OnEnsureProjectNodesLoaded, this.CanEnsureProjectNodesLoaded);
        }

        /// <summary>
        /// Determines whether this instance [can ensure project nodes loaded].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// <c>true</c> if this instance [can ensure project nodes loaded]; otherwise, <c>false</c>.
        /// </returns>
        private bool CanEnsureProjectNodesLoaded(object context)
        {
            return this.ProjectData != null;
        }

        /// <summary>
        /// Called when [ensure project nodes loaded].
        /// </summary>
        /// <param name="context">The context.</param>
        private void OnEnsureProjectNodesLoaded(object context)
        {
            if (this.ProjectData == null || this.ProjectSelectorService.HasProjectNodes(this.ProjectData))
            {
                return;
            }

            this.SetBusyStatusAndClearPreviousErrorMessage();
            Action callBack = () =>
                {
                    this.ClearStatusAndApplicationMessage();
                    this.IsBusy = false;
                    this.RaisePropertyChangedNotifications();
                    ExpandTreeViewComboBox(context as IExpandable);
                };

            this.ProjectSelectorService.BeginEnsureNodesLoaded(this.ProjectData, callBack);

            this.SetStatusAndExecuteApplicationMessageCommand(Resources.String063);
        }

        /// <summary>
        /// Sets the busy status and clear previous error message.
        /// </summary>
        private void SetBusyStatusAndClearPreviousErrorMessage()
        {
            this.IsBusy = true;
            this.ErrorMessage = null;
        }

        /// <summary>
        /// Called when [cancel].
        /// </summary>
        /// <param name="context">The context.</param>
        private void OnCancel(object context)
        {
            this.commandElement = context as UIElement;

            this.CloseDialogAndReleaseResources();
        }

        /// <summary>
        /// Disconnects the selector service.
        /// </summary>
        private void DetachSelectorService()
        {
            this.ProjectSelectorService.AsyncException -= this.OnAsyncException;
            this.ProjectSelectorService = null;
        }

        /// <summary>
        /// Determines whether this instance [can execute load project].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// <c>true</c> if this instance [can execute load project]; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteLoadProject(object context)
        {
            var hasProjectData = this.ProjectData != null;

            var hasValidIterationPath = !string.IsNullOrEmpty(this.IterationPath);

            var hasValidAreaPath = !string.IsNullOrEmpty(this.AreaPath);

            return hasProjectData && hasValidIterationPath && hasValidAreaPath;
        }

        /// <summary>
        /// Called when [execute load project].
        /// </summary>
        /// <param name="context">The context.</param>
        private void OnExecuteLoadProject(object context)
        {
            this.commandElement = context as IInputElement;
            this.SetBusyStatusAndClearPreviousErrorMessage();
            this.StartLoading();
        }

        /// <summary>
        /// Starts the loading.
        /// </summary>
        private void StartLoading()
        {
            this.CreateLoader();

            this.AttachLoaderListeners();

            this.loader.Start();

            this.SetStatusAndExecuteApplicationMessageCommand(Resources.String062);
        }

        /// <summary>
        /// Sets the status and execute application message command.
        /// </summary>
        /// <param name="message">The message.</param>
        private void SetStatusAndExecuteApplicationMessageCommand(string message)
        {
            this.StatusMessage = message;
            CommandLibrary.ApplicationMessageCommand.Execute(message, this.commandElement);
        }

        /// <summary>
        /// Clears the status and application message.
        /// </summary>
        private void ClearStatusAndApplicationMessage()
        {
            this.StatusMessage = null;
            CommandLibrary.ApplicationMessageCommand.Execute(string.Empty, this.commandElement);
        }

        /// <summary>
        /// Creates the loader.
        /// </summary>
        private void CreateLoader()
        {
            this.loader = this.ProjectSelectorService.CreateLoader(this.ProjectData);
        }

        /// <summary>
        /// Attaches the loader listeners.
        /// </summary>
        private void AttachLoaderListeners()
        {
            this.loader.Aborted += this.OnLoadAborted;
            this.loader.Complete += this.OnLoadComplete;
            this.loader.VolumeWarning += this.OnVolumeWarning;
            this.loader.ConfirmLoadData += this.OnConfirmLoadData;
        }

        /// <summary>
        /// Disposes the loader.
        /// </summary>
        private void DisposeLoader()
        {
            if (this.loader == null)
            {
                return;
            }

            this.DetachLoaderListeners();

            this.loader.Dispose();

            this.loader = null;
        }

        /// <summary>
        /// Detaches the loader listeners.
        /// </summary>
        private void DetachLoaderListeners()
        {
            this.loader.Aborted -= this.OnLoadAborted;
            this.loader.Complete -= this.OnLoadComplete;
            this.loader.VolumeWarning -= this.OnVolumeWarning;
            this.loader.ConfirmLoadData -= this.OnConfirmLoadData;
        }

        /// <summary>
        /// Called when [load aborted].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnLoadAborted(object sender, EventArgs e)
        {
            this.IsBusy = false;

            this.ApplyIgnoreVolumeWarningDecision();

            this.ClearStatusAndApplicationMessage();

            this.DisposeLoader();
        }

        /// <summary>
        /// Called when [load complete].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnLoadComplete(object sender, EventArgs e)
        {
            this.ApplyIgnoreVolumeWarningDecision();

            this.CloseDialogAndReleaseResources();
        }

        /// <summary>
        /// Closes the dialog and release resources.
        /// </summary>
        private void CloseDialogAndReleaseResources()
        {
            CommandLibrary.CloseDialogCommand.Execute(this.commandElement, this.commandElement);

            this.DisposeLoader();

            this.DetachSelectorService();

            if (this.ProjectData != this.projectDataService.CurrentProjectData)
            {
                this.projectDataService.ClearAll(this.ProjectData);
            }
        }

        /// <summary>
        /// Applies the ignore volume warnng decision.
        /// </summary>
        private void ApplyIgnoreVolumeWarningDecision()
        {
            this.ProjectData.HideVolumeWarning = this.loader.IgnoreFutureVolumeWarnings;
        }

        /// <summary>
        /// Called when [Close current project] raised.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnConfirmLoadData(object sender, CancelEventArgs e)
        {
            if (this.ProjectDataIsGlobalInstance())
            {
                return;
            }

            CommandLibrary.CloseProjectCommand.Execute(null, this.commandElement);
            CommandController.WaitForAllCommandsToComplete();

            e.Cancel = this.GlobalProjectDataInstanceIsPresent();
        }

        /// <summary>
        /// Indicates if the local project data property matches the global instance.
        /// </summary>
        /// <returns><c>True</c> is local matches global; otherw <c>false</c>.</returns>
        private bool ProjectDataIsGlobalInstance()
        {
            return this.projectDataService.CurrentProjectData == this.ProjectData;
        }

        /// <summary>
        /// Called when [volume warning].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.ContextEventArgs&lt;DecisionControl&gt;"/> instance containing the event data.</param>
        private void OnVolumeWarning(object sender, ContextEventArgs<DecisionControl> e)
        {
            CommandLibrary.ShowDialogCommand.Execute(e.Context, this.commandElement);
        }

        /// <summary>
        /// Called when [show project selector].
        /// </summary>
        /// <param name="context">The context.</param>
        private void OnShowProjectSelector(object context)
        {
            var projectData = this.ProjectSelectorService.ShowProjectSelector();

            if (projectData != null)
            {
                this.InitialiseProjectValues(projectData);
            }
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Sets the project data and apply paths.
        /// </summary>
        /// <param name="value">The value.</param>
        private void InitialiseProjectValues(IProjectData value)
        {
            this.ProjectData = this.GetSpecifiedOrLastProjectData(value);

            this.RaisePropertyChangedNotifications();
        }

        /// <summary>
        /// Raises the property changed notifications.
        /// </summary>
        private void RaisePropertyChangedNotifications()
        {
            this.OnPropertyChanged("ProjectData");
            this.OnPropertyChanged("SelectedProjectName");
            this.OnPropertyChanged("SelectedCollectionEndPoint");
            this.OnPropertyChanged("IterationPath");
            this.OnPropertyChanged("AreaPath");
            this.OnPropertyChanged("IterationRootNode");
            this.OnPropertyChanged("AreaRootNode");
        }

        /// <summary>
        /// Gets the specified or last project data.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The specfied instance if not null; otherwise the previous instance.</returns>
        private IProjectData GetSpecifiedOrLastProjectData(IProjectData value)
        {
            return value ?? this.ProjectSelectorService.GetLastProjectData();
        }

        /// <summary>
        /// Gets the project value with null check.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="getValue">The get value method.</param>
        /// <returns><c>Null</c> if project data is null; otherwise the getValue output.</returns>
        private T GetProjectValueWithNullCheck<T>(Func<IProjectData, T> getValue)
        {
            return this.ProjectData == null ? default(T) : getValue(this.ProjectData);
        }

        /// <summary>
        /// Gets the project node with null check.
        /// </summary>
        /// <param name="nodeName">Name of the node.</param>
        /// <returns><c>Null</c> if node does not exist; otherwise project node.</returns>
        private IProjectNode GetProjectNodeWithNullCheck(string nodeName)
        {
            IProjectNode projectNode = null;
            if (this.ProjectData != null && this.ProjectData.ProjectNodes != null)
            {
                this.ProjectData.ProjectNodes.TryGetValue(nodeName, out projectNode);
            }

            return projectNode;
        }

        /// <summary>
        /// Sets the project value if project data is not null.
        /// </summary>
        /// <param name="setValue">The set value method.</param>
        private void SetProjectValueWithNullCheck(Action<IProjectData> setValue)
        {
            if (this.ProjectData != null)
            {
                setValue(this.ProjectData);
            }
        }
    }
}
