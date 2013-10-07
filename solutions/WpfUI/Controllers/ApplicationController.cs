// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationController.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ApplicationController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;

    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.UIElements;
    using TfsWorkbench.WpfUI.ProjectSelector;
    using TfsWorkbench.WpfUI.Properties;

    /// <summary>
    /// Initializes instance of ApplicationController
    /// </summary>
    internal class ApplicationController : ProjectDataServiceConsumer, IApplicationController
    {
        /// <summary>
        /// The display elements.
        /// </summary>
        [ImportMany]
        private readonly ICollection<IDisplayMode> displayModes = new Collection<IDisplayMode>();

        /// <summary>
        /// The plugin collection.
        /// </summary>
        [ImportMany]
        private readonly ICollection<IWorkbenchPlugin> plugins = new Collection<IWorkbenchPlugin>();

        /// <summary>
        /// The assmbly data array.
        /// </summary>
        private static readonly List<string[]> assemblyData = new List<string[]>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationController"/> class.
        /// </summary>
        /// <param name="mainWindow">The main window.</param>
        public ApplicationController(MainAppWindow mainWindow)
        {
            if (mainWindow == null)
            {
                throw new ArgumentNullException("mainWindow");
            }

            Instance = this;

            this.MainWindow = mainWindow;
            this.MainWindow.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(this.Setup));
            this.DialogController = new DialogController(this.MainWindow);

            this.EnableInput(false);
        }

        /// <summary>
        /// Gets the assembly data.
        /// </summary>
        /// <value>The assembly data.</value>
        public static IEnumerable<string[]> AssemblyData
        {
            get { return assemblyData; }
        }

        /// <summary>
        /// Gets or sets the application controller instance.
        /// </summary>
        /// <value>The the application controller instance.</value>
        public static IApplicationController Instance { get; set; }

        /// <summary>
        /// Gets the data provider helper.
        /// </summary>
        /// <value>The data provider helper.</value>
        public IDataProviderController DataProviderController { get; private set; }

        /// <summary>
        /// Gets the dialog controller.
        /// </summary>
        /// <value>The dialog controller.</value>
        public IDialogController DialogController { get; private set; }

        /// <summary>
        /// Gets the main window.
        /// </summary>
        /// <value>The main window.</value>
        public MainAppWindow MainWindow { get; private set; }

        /// <summary>
        /// Saves the project layout.
        /// </summary>
        public void SaveProjectLayout()
        {
            var projectData = this.ProjectDataService.CurrentProjectData;

            if (projectData == null)
            {
                return;
            }

            var projectCollectionUri = new Uri(
                projectData.ProjectCollectionUrl, UriKind.Absolute);

            var projectLayoutPath = this.ProjectDataService.DefaultFilePath(
                projectCollectionUri, projectData.ProjectName);

            this.ProjectDataService.SaveProjectLayoutData(projectData, projectLayoutPath);
        }

        /// <summary>
        /// Workbenches the item save error.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="errors">The errors.</param>
        public void WorkbenchItemSaveError(IWorkbenchItem workbenchItem, IEnumerable<string> errors)
        {
            var message = string.Format(
                CultureInfo.InvariantCulture,
                Resources.String044,
                workbenchItem.GetTypeName(),
                workbenchItem.GetId());

            message = errors.Aggregate(message, (current, error) => string.Concat(current, Environment.NewLine, error));

            this.SetStatusMessage(message);

            MessageBox.Show(message);

            this.MainWindow.Dispatcher.BeginInvoke(
                DispatcherPriority.Input,
                new Action(() => this.EnableInput(true)));
        }

        /// <summary>
        /// Raises the application message command.
        /// </summary>
        /// <param name="message">The message.</param>
        public void SetStatusMessage(string message)
        {
            var callback = (SendOrPostCallback)delegate
                {
                    CommandLibrary.ApplicationMessageCommand.Execute(message, this.MainWindow);
                };

            this.MainWindow.Dispatcher.BeginInvoke(DispatcherPriority.Background, callback, null);
        }

        /// <summary>
        /// Closes the project.
        /// </summary>
        public void CloseProject()
        {
            var projectData = this.ProjectDataService.CurrentProjectData;

            if (projectData == null || !this.DiscardAnyUnsavedChanges())
            {
                return;
            }

            HighlightHelper.ClearAllHighlights();

            this.SaveProjectLayout();

            this.ProjectDataService.ClearAllCurrentProjectData();

            this.DataProviderController.ClearControlCache();

            this.MainWindow.TitleControl.ProjectData = null;
            this.MainWindow.MainMenu.ProjectData = null;

            this.ProjectDataService.CurrentProjectData = null;

            this.MainWindow.UpdateLayout();

            this.SetStatusMessage(Resources.String002);
        }

        /// <summary>
        /// Enables main window input.
        /// </summary>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        public void EnableInput(bool enable)
        {
            if (enable && this.DialogController.IsDisplayingModalDialog)
            {
                return;
            }

            this.MainWindow.PATH_DisabledOverlay.Visibility = enable ? Visibility.Collapsed : Visibility.Visible;

            this.MainWindow.PART_InternalGrid.IsEnabled = enable;

            if (enable)
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>
        /// Discards the unsaved changes.
        /// </summary>
        /// <returns><c>True</c> if chages are to be discarded; otherwise <c>false</c></returns>
        public bool DiscardAnyUnsavedChanges()
        {
            var projectData = this.ProjectDataService.CurrentProjectData;

            return projectData == null || !projectData.WorkbenchItems.Any(tbi => tbi.ValueProvider.IsDirty) ||
                   MessageBox.Show(
                       Resources.String003,
                       Resources.String004,
                       MessageBoxButton.YesNo, 
                       MessageBoxImage.Exclamation) == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Sets the active display activeDisplayMode.
        /// </summary>
        /// <param name="activeDisplayMode">The active display element.</param>
        public void SetActiveDisplayMode(IDisplayMode activeDisplayMode)
        {
            this.MainWindow.TitleControl.ActiveDisplayMode = activeDisplayMode;
        }

        /// <summary>
        /// Applies the loaded data.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        public void ApplyLoadedData(IProjectData projectData)
        {
            if (projectData == null)
            {
                throw new ArgumentNullException("projectData");
            }

            this.ProjectDataService.CurrentProjectData = projectData;

            projectData.Dispatcher = this.MainWindow.Dispatcher;

            this.MainWindow.TitleControl.ProjectData = null;
            this.MainWindow.TitleControl.ProjectData = projectData;

            this.MainWindow.MainMenu.ProjectData = projectData;

            this.SetStatusMessage(
                string.Format(
                    CultureInfo.InvariantCulture, 
                    Resources.String001, 
                    projectData.WorkbenchItems.Count(), 
                    projectData.WorkbenchItems.UnfilteredList.Count()));

            projectData.WorkbenchItems.OnRefreshCollection();

            this.EnableInput(true);
        }

        /// <summary>
        /// Registers the services.
        /// </summary>
        private static void RegisterServices()
        {
            ServiceManager.Instance.RegisterConstructor<IProjectSelectorService, ProjectSelectorService>();
        }

        /// <summary>
        /// Sets up this instance.
        /// </summary>
        private void Setup()
        {
            try
            {
                CommandController.SetupCommandBindings();

                try
                {
                    this.Compose();
                }
                catch (ReflectionTypeLoadException rtlex)
                {
                    var message = rtlex.LoaderExceptions
                        .Aggregate(
                            Resources.String045, 
                            (current, loaderException) => string.Concat(current, Environment.NewLine, loaderException.Message));

                    throw new ArgumentException(message, rtlex);
                }

                RegisterServices();

                CommandLibrary.LoadProjectCommand.Execute(null, this.MainWindow);
            }
            catch (Exception ex)
            {
                if (CommandLibrary.ApplicationExceptionCommand.CanExecute(ex, this.MainWindow))
                {
                    CommandLibrary.ApplicationExceptionCommand.Execute(ex, this.MainWindow);
                    this.EnableInput(true);
                    return;
                }

                throw;
            }
        }

        /// <summary>
        /// Composes this instance.
        /// </summary>
        private void Compose()
        {
            IDataProvider dataProvider = null;
            using (var catalog = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory))
            {
                using (var container = new CompositionContainer(catalog))
                {
                    try
                    {
                        var lazyDataProvider = container.GetExport<IDataProvider>();
                        if (lazyDataProvider != null)
                        {
                            dataProvider = lazyDataProvider.Value;
                        }
                    }
                    catch (ImportCardinalityMismatchException)
                    {
                        dataProvider = null;
                    }
                    catch (ObjectDisposedException)
                    {
                        dataProvider = null;
                    }

                    if (dataProvider == null)
                    {
                        throw new ArgumentException(Resources.String046);
                    }

                    this.ProjectDataService.CurrentDataProvider = dataProvider;

                    var batch = new CompositionBatch();
                    batch.AddPart(this);
                    container.Compose(batch);
                }
            }

            assemblyData.Clear();

            assemblyData.AddRange(this.GetAssemblyLoadedData(dataProvider));

            this.RegisterDataProviderHelper(dataProvider);
            this.RegisterDisplayModes();
            this.RegisterPlugins();
        }

        /// <summary>
        /// Registers the plugins.
        /// </summary>
        private void RegisterPlugins()
        {
            foreach (var plugin in this.plugins.OrderBy(p => p.DisplayPriority))
            {
                foreach (var commandBinding in plugin.CommandBindings)
                {
                    CommandController.RegisterCommandBinding(commandBinding);
                }

                var menu = plugin.MenuItem;
                var button = plugin.ControlElement;

                if (menu != null)
                {
                    this.MainWindow.MainMenu.PART_PlugInMenu.Items.Add(menu);
                }

                if (button != null)
                {
                    button.SetValue(DockPanel.DockProperty, Dock.Right);
                    button.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 4, 0));
                    this.MainWindow.PART_PlugInPanel.Children.Insert(0, button);
                }
            }
        }

        /// <summary>
        /// Registers the data provider helper.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        private void RegisterDataProviderHelper(IDataProvider dataProvider)
        {
            this.DataProviderController = new DataProviderController(dataProvider, this);
        }

        /// <summary>
        /// Gets the loaded assembly data.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        /// <returns>
        /// A list of string arrays containing the loaded assembly data.
        /// </returns>
        private IEnumerable<string[]> GetAssemblyLoadedData(IDataProvider dataProvider)
        {
            Func<string, Assembly, string[]> getAssemblyDetails = (title, assembly) =>
                {
                    var assemblyName = assembly.GetName();

                    return new[] { title, assemblyName.Name, assemblyName.Version.ToString() };
                };

            var output = new List<string[]>
                {
                    getAssemblyDetails("Shell", Assembly.GetExecutingAssembly())
                };

            if (dataProvider != null)
            {
                output.Add(getAssemblyDetails("Data Provider", dataProvider.GetType().Assembly));
            }

            output.AddRange(this.displayModes.Select(displayMode => getAssemblyDetails(displayMode.Title, displayMode.GetType().Assembly)));
            output.AddRange(this.plugins.Select(plugin => getAssemblyDetails(plugin.DisplayName, plugin.GetType().Assembly)));

            return output;
        }

        /// <summary>
        /// Registers the display elements.
        /// </summary>
        private void RegisterDisplayModes() 
        {
            this.MainWindow.PART_DisplayModeGrid.Children.Clear();

            foreach (var displayMode in this.displayModes.OrderBy(de => de.DisplayPriority))
            {
                var element = displayMode as UIElement;

                if (element != null)
                {
                    this.MainWindow.PART_DisplayModeGrid.Children.Add(element);
                }

                var menu = displayMode.MenuControl;

                if (menu != null)
                {
                    this.MainWindow.MainMenu.PART_ViewMenu.Items.Add(menu);
                }

                this.MainWindow.TitleControl.AddDisplayMode(displayMode);

                if (displayMode.IsHighlightProvider)
                {
                    this.ProjectDataService.HighlightProviders.Add(displayMode);
                }
            }

            var primaryDisplayMode = this.displayModes.OrderBy(de => de.DisplayPriority).FirstOrDefault();

            if (primaryDisplayMode != null)
            {
                if (CommandLibrary.ShowDisplayModeCommand.CanExecute(primaryDisplayMode, this.MainWindow))
                {
                    CommandLibrary.ShowDisplayModeCommand.Execute(primaryDisplayMode, this.MainWindow);
                }
                else
                {
                    this.SetActiveDisplayMode(primaryDisplayMode);
                }
            }
        }
    }
}