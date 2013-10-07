// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewWindow.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for ViewWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.TaskBoardUI
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using TfsWorkbench.Core.DataObjects;
    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.TaskBoardUI.DataObjects;
    using TfsWorkbench.TaskBoardUI.Helpers;
    using TfsWorkbench.TaskBoardUI.Properties;
    using TfsWorkbench.UIElements;

    /// <summary>
    /// Interaction logic for ViewWindow.xaml
    /// </summary>
    public partial class ViewWindow
    {
        /// <summary>
        /// The application main window.
        /// </summary>
        private readonly Window mainWindow;

        /// <summary>
        /// The project data service instance.
        /// </summary>
        private readonly IProjectDataService projectDataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewWindow"/> class.
        /// </summary>
        /// <param name="viewMap">The view map.</param>
        public ViewWindow(ViewMap viewMap)
            : this(viewMap, Application.Current.MainWindow, ServiceManager.Instance.GetService<IProjectDataService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewWindow"/> class.
        /// </summary>
        /// <param name="viewMap">The view map.</param>
        /// <param name="mainWindow">The main window.</param>
        /// <param name="projectDataService">The project data service.</param>
        public ViewWindow(ViewMap viewMap, Window mainWindow, IProjectDataService projectDataService)
        {
            if (viewMap == null)
            {
                throw new ArgumentNullException("viewMap");
            }

            if (mainWindow == null)
            {
                throw new ArgumentNullException("mainWindow");
            }

            if (projectDataService == null)
            {
                throw new ArgumentNullException("projectDataService");
            }

            if (projectDataService.CurrentProjectData == null)
            {
                throw new ArgumentException("Project Data Cannot Be Null");
            }

            this.InitializeComponent();

            this.mainWindow = mainWindow;
            this.projectDataService = projectDataService;
            this.Icon = mainWindow.Icon;
            this.mainWindow.Closed += this.OnMainWindowClosed;
            this.SetUpCommandRedirection(mainWindow);
            this.SetUpViewControl(viewMap, projectDataService.CurrentProjectData);

            this.AttachProjectDataServiceListeners();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the event data.</param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }

            Settings.Default.WindowHeight = this.Height;
            Settings.Default.WindowWidth = this.Width;
            Settings.Default.WindowLeft = this.Left;
            Settings.Default.WindowTop = this.Top;

            Settings.Default.Save();
        }

        /// <summary>
        /// Attaches the project data service listeners.
        /// </summary>
        private void AttachProjectDataServiceListeners()
        {
            this.projectDataService.ProjectDataChanged += this.OnProjectDataChanged;
        }

        /// <summary>
        /// Called when [main window closed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnMainWindowClosed(object sender, EventArgs eventArgs)
        {
            this.Close();
        }

        /// <summary>
        /// Builds the swim lane view.
        /// </summary>
        /// <param name="viewMap">The view map.</param>
        /// <param name="projectData">The project data.</param>
        private void SetUpViewControl(ViewMap viewMap, IProjectData projectData)
        {
            var swimLaneView = new SwimLaneView(viewMap, false);

            SwimLaneService.Instance.SwimLaneViews.Add(swimLaneView);
            SwimLaneHelper.SyncroniseViewItems(swimLaneView, projectData.WorkbenchItems);

            this.PART_ViewControl.ProjectData = projectData;
            this.PART_ViewControl.SwimLaneView = swimLaneView;
        }

        /// <summary>
        /// Called when [project data changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.ProjectDataChangedEventArgs"/> instance containing the event data.</param>
        private void OnProjectDataChanged(object sender, ProjectDataChangedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Sets up command redirection.
        /// </summary>
        /// <param name="window">The window.</param>
        private void SetUpCommandRedirection(UIElement window)
        {
            foreach (var binding in window.CommandBindings.OfType<CommandBinding>()
                .Where(binding => binding.Command != CommandLibrary.EditViewCommand))
            {
                this.CommandBindings.Add(binding);
            }
        }

        /// <summary>
        /// Called when [window closed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnWindowClosed(object sender, EventArgs e)
        {
            this.mainWindow.Closed -= this.OnMainWindowClosed;

            this.DetattchProjectDataServiceListeners();

            SwimLaneService.Instance.SwimLaneViews.Remove(this.PART_ViewControl.SwimLaneView);

            this.CommandBindings.Clear();

            this.PART_ViewControl.ReleaseResources();
        }

        /// <summary>
        /// Detattches the project data service listeners.
        /// </summary>
        private void DetattchProjectDataServiceListeners()
        {
            this.projectDataService.ProjectDataChanged -= this.OnProjectDataChanged;
        }
    }
}
