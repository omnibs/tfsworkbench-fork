// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterServiceController.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the Controller type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.FilterService
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using System.Xml.Schema;

    using TfsWorkbench.Core.DataObjects;
    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.FilterService.Properties;
    using TfsWorkbench.UIElements;

    /// <summary>
    /// The filter plugin controller.
    /// </summary>
    internal class FilterServiceController : IFilterServiceController
    {
        /// <summary>
        /// The schema resource location.
        /// </summary>
        private const string FilterCollectionResourceLocation = "TfsWorkbench.FilterService.Resources.FilterCollection.xsd";

        /// <summary>
        /// The display element.
        /// </summary>
        private readonly IFilterServiceView view;

        /// <summary>
        /// The project data service instance.
        /// </summary>
        private readonly IProjectDataService projectDataService;

        /// <summary>
        /// The filter serfvice instance.
        /// </summary>
        private readonly IFilterService filterService;

        /// <summary>
        /// The filter collection.
        /// </summary>
        private readonly WorkbenchFilterCollection collection = new WorkbenchFilterCollection();

        /// <summary>
        /// The filter info instance.
        /// </summary>
        private readonly FilterInfo filterInfo;

        /// <summary>
        /// The internal serialiser instance.
        /// </summary>
        private SerializerInstance<WorkbenchFilterCollection> serialiser;

        /// <summary>
        /// A flag to indicate when the filter feild name is being initialised.
        /// </summary>
        private bool isInitialisingFieldName;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterServiceController"/> class.
        /// </summary>
        public FilterServiceController()
            : this(new FilterServiceView(), ServiceManager.Instance.GetService<IProjectDataService>(), ServiceManager.Instance.GetService<IFilterService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterServiceController"/> class.
        /// </summary>
        /// <param name="view">The dispaly element.</param>
        /// <param name="projectDataService">The project data service.</param>
        /// <param name="filterService">The filter service.</param>
        public FilterServiceController(IFilterServiceView view, IProjectDataService projectDataService, IFilterService filterService)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            if (projectDataService == null)
            {
                throw new ArgumentNullException("projectDataService");
            }

            if (filterService == null)
            {
                throw new ArgumentNullException("filterService");
            }

            this.filterInfo = new FilterInfo();

            this.view = view;
            this.projectDataService = projectDataService;
            this.filterService = filterService;
            this.view.Controller = this;
            this.SetUpCommandBindings(this.view);

            this.projectDataService.ProjectDataChanged += this.OnProjectDataChanged;
        }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>The filters.</value>
        public WorkbenchFilterCollection Filters
        {
            get
            {
                return this.collection;
            }
        }

        /// <summary>
        /// Gets the filter info.
        /// </summary>
        /// <value>The filter info.</value>
        public FilterInfo FilterInfo
        {
            get
            {
                return this.filterInfo;
            }
        }

        /// <summary>
        /// Gets the serialiser.
        /// </summary>
        /// <value>The serialiser.</value>
        private SerializerInstance<WorkbenchFilterCollection> Serialiser
        {
            get
            {
                return this.serialiser = this.serialiser ?? new SerializerInstance<WorkbenchFilterCollection>();
            }
        }

        /// <summary>
        /// Shows the filter dialog.
        /// </summary>
        /// <param name="executedRoutedEventArgs">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public void ShowFilterDialog(ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            this.collection.Clear();
            this.view.WorkbenchFilter = null;

            var currentProjectData = this.projectDataService.CurrentProjectData;

            if (currentProjectData == null)
            {
                return;
            }

            foreach (var filter in this.LoadProjectFilters(currentProjectData))
            {
                this.collection.And(filter);
            }

            CommandLibrary.ShowDialogCommand.Execute(this.view, executedRoutedEventArgs.OriginalSource as IInputElement);
        }

        /// <summary>
        /// Closes the filter dialog.
        /// </summary>
        public void CloseFilterDialog()
        {
            var projectData = this.projectDataService.CurrentProjectData;
            var currentFilter = this.filterService.FilterProvider as WorkbenchFilterCollection;
            var currentFilterDescription = currentFilter == null ? string.Empty : currentFilter.Description;

            var requiresUpdate = 
                projectData != null && !Equals(currentFilterDescription, this.collection.Description);

            Action callback = () =>
                    {
                        if (requiresUpdate)
                        {
                            this.ApplyFilter(projectData);
                            this.view.IsEnabled = true;
                        }

                        CommandLibrary.CloseDialogCommand.Execute(this.view, this.view);
                    };

            if (requiresUpdate)
            {
                this.view.IsEnabled = false;
                CommandLibrary.ApplicationMessageCommand.Execute(Resources.String034, this.view);
            }

            this.view.Dispatcher.BeginInvoke(DispatcherPriority.Background, callback);
        }

        /// <summary>
        /// Determines whether this instance [can show filter dialog].
        /// </summary>
        /// <param name="canExecuteRoutedEventArgs">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        public void CanShowFilterDialog(CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
        {
            canExecuteRoutedEventArgs.CanExecute = this.projectDataService.CurrentProjectData != null;
        }

        /// <summary>
        /// Executes the apply filters command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public void ExecuteApplyFiltersCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var projectData = this.projectDataService.CurrentProjectData;

            if (projectData == null)
            {
                return;
            }

            Action callback = () =>
                {
                    this.ApplyFilter(projectData);
                    this.view.IsEnabled = true;
                };

            this.view.IsEnabled = false;
            CommandLibrary.ApplicationMessageCommand.Execute(Resources.String034, this.view);
            this.view.Dispatcher.BeginInvoke(DispatcherPriority.Background, callback);
        }

        /// <summary>
        /// Determines whether this instance [can clear all filters] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        public void CanClearAllFilters(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.collection != null && this.collection.Any();
        }

        /// <summary>
        /// Executes the clear all filters command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public void ExecuteClearAllFiltersCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show(Resources.String037, Resources.String038, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
            {
                return;
            }

            this.collection.Clear();
        }

        /// <summary>
        /// Executes the clear and reapply.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public void ExecuteClearAllAndReApply(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show(Resources.String037, Resources.String038, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
            {
                return;
            }

            this.collection.Clear();

            var projectData = this.projectDataService.CurrentProjectData;
            var mainWindow = Application.Current != null ? Application.Current.MainWindow : null;

            if (projectData == null || mainWindow == null)
            {
                return;
            }

            Action callback = () =>
            {
                this.ApplyFilter(projectData);
                CommandLibrary.DisableUserInputCommand.Execute(false, mainWindow);
            };

            CommandLibrary.DisableUserInputCommand.Execute(true, mainWindow);
            CommandLibrary.ApplicationMessageCommand.Execute(Resources.String034, this.view);
            mainWindow.Dispatcher.BeginInvoke(DispatcherPriority.Background, callback);
        }

        /// <summary>
        /// Sets up command bindings.
        /// </summary>
        /// <param name="filterView">The filter view.</param>
        private void SetUpCommandBindings(IFilterServiceView filterView)
        {
            filterView.CommandBindings.Add(
                new CommandBinding(LocalCommandLibrary.AddFilterCommand, (s, e) => this.ExecuteAddFilterCommand(FilterActionOption.Exclude)));

            filterView.CommandBindings.Add(
                new CommandBinding(LocalCommandLibrary.SelectFilterCommand, (s, e) => this.SetSelectedFilter(e.Parameter as WorkbenchFilter)));

            filterView.CommandBindings.Add(
                new CommandBinding(LocalCommandLibrary.RemoveFilterCommand, this.ExecuteRemoveFilterCommand));

            filterView.CommandBindings.Add(
                new CommandBinding(LocalCommandLibrary.ClearAllFiltersCommand, this.ExecuteClearAllFiltersCommand, this.CanClearAllFilters));

            filterView.CommandBindings.Add(
                new CommandBinding(LocalCommandLibrary.ApplyFiltersCommand, this.ExecuteApplyFiltersCommand));
        }

        /// <summary>
        /// Executes the remove filter command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteRemoveFilterCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var filter = e.Parameter as WorkbenchFilter;

            if (filter == null)
            {
                return;
            }

            this.collection.Remove(filter);

            if (this.view.WorkbenchFilter == filter)
            {
                this.SetSelectedFilter(null);
            }
        }

        /// <summary>
        /// Executes the add exclusion filter command.
        /// </summary>
        /// <param name="filterAction">The filter action.</param>
        private void ExecuteAddFilterCommand(FilterActionOption filterAction)
        {
            WorkbenchFilter filter;
            if (this.TryCreateFilter(filterAction, out filter))
            {
                this.SetSelectedFilter(filter);
            }
        }

        /// <summary>
        /// Sets the selected filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        private void SetSelectedFilter(WorkbenchFilter filter)
        {
            if (this.view.WorkbenchFilter == filter)
            {
                return;
            }

            if (this.view.WorkbenchFilter != null)
            {
                // Remove handles
                this.view.WorkbenchFilter.PropertyChanged -= this.OnSelectedFilterPropertyChanged;
            }

            this.view.WorkbenchFilter = null;

            this.InitialiseStateOptions(filter);
            this.InitialiseFieldNames(filter);
            this.SetValueControlTemplate(filter);

            this.view.WorkbenchFilter = filter;

            if (this.view.WorkbenchFilter != null)
            {
                // Add handles
                this.view.WorkbenchFilter.PropertyChanged += this.OnSelectedFilterPropertyChanged;
            }
        }

        /// <summary>
        /// Tries to create a new filter.
        /// </summary>
        /// <param name="actionOption">The action option.</param>
        /// <param name="filter">The filter.</param>
        /// <returns><c>True</c> if the fitler is created; otherwise <c>false</c>.</returns>
        private bool TryCreateFilter(FilterActionOption actionOption, out WorkbenchFilter filter)
        {
            filter = null;

            var projectData = this.projectDataService.CurrentProjectData;

            if (projectData != null)
            {
                filter = new WorkbenchFilter(actionOption)
                    .ItemsOfType(Resources.String002)
                    .WithField(Resources.String030)
                    .That(FilterOperatorOption.IsEqualTo, Resources.String031);

                this.collection.Add(filter);
            }

            return filter != null; 
        }

        /// <summary>
        /// Called when [selected filter property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnSelectedFilterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case WorkbenchFilter.ItemTypeNamePropertyName:
                    this.InitialiseStateOptions(this.view.WorkbenchFilter);
                    this.InitialiseFieldNames(this.view.WorkbenchFilter);
                    break;

                case WorkbenchFilter.FieldNamePropertyName:
                    this.SetValueControlTemplate(this.view.WorkbenchFilter);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Initialises the state options.
        /// </summary>
        /// <param name="workbenchFilter">The workbench filter.</param>
        private void InitialiseStateOptions(WorkbenchFilter workbenchFilter)
        {
            var projectData = this.projectDataService.CurrentProjectData;
            var filterServiceView = this.view;
            if (projectData == null || workbenchFilter == null || filterServiceView == null)
            {
                return;
            }

            IEnumerable<string> avilableStates;

            if (workbenchFilter.ItemTypeName == Resources.String002)
            {
                avilableStates = projectData.ItemTypes.SelectMany(it => it.States).Distinct().OrderBy(s => s).ToArray();
            }
            else
            {
                ItemTypeData itemTypeData;
                avilableStates = projectData.ItemTypes.TryGetValue(workbenchFilter.ItemTypeName, out itemTypeData)
                    ? itemTypeData.States.OrderBy(s => s).ToArray()
                    : new string[] { };
            }

            string selectedState = null;
            if (workbenchFilter.FieldName == Resources.String030)
            {
                selectedState = workbenchFilter.Value;
            }

            this.filterInfo.StateOptions.Clear();

            foreach (var state in avilableStates)
            {
                this.filterInfo.StateOptions.Add(state);
            }

            if (selectedState != null)
            {
                workbenchFilter.Value = selectedState;
            }
        }

        /// <summary>
        /// Sets the value control template.
        /// </summary>
        /// <param name="filter">The filter.</param>
        private void SetValueControlTemplate(WorkbenchFilter filter)
        {
            if (this.isInitialisingFieldName || this.view == null)
            {
                return;
            }

            var contentControl = this.view.ValueCotnrol;

            if (contentControl != null && contentControl.ContentTemplateSelector != null)
            {
                contentControl.ContentTemplate =
                    contentControl.ContentTemplateSelector.SelectTemplate(filter, contentControl);
            }
        }

        /// <summary>
        /// Initialises the field names.
        /// </summary>
        /// <param name="workbenchFilter">Name of the item type.</param>
        private void InitialiseFieldNames(WorkbenchFilter workbenchFilter)
        {
            var filterServiceView = this.view;
            if (filterServiceView == null)
            {
                return;
            }

            this.isInitialisingFieldName = true;

            while (true)
            {
                // Capture the selected field name before clearing the list.
                var selectedFieldName = workbenchFilter == null ? null : workbenchFilter.FieldName;

                this.FilterInfo.FieldNames.Clear();

                var projectData = this.projectDataService.CurrentProjectData;

                if (projectData == null || workbenchFilter == null)
                {
                    break;
                }

                var fields =
                    new List<string>(
                        new[] { Resources.String048, Resources.String049, Resources.String050, Resources.String051 });

                if (workbenchFilter.ItemTypeName == Resources.String002)
                {
                    fields.AddRange(
                        projectData
                            .ItemTypes
                            .SelectMany(it => it.Fields)
                            .Select(f => f.DisplayName)
                            .OrderBy(dn => dn)
                            .Distinct());
                }
                else
                {
                    var selectedType = projectData.ItemTypes.FirstOrDefault(it => it.TypeName == workbenchFilter.ItemTypeName);

                    if (selectedType == null)
                    {
                        break;
                    }

                    fields.AddRange(selectedType.Fields.Select(f => f.DisplayName).OrderBy(dn => dn));
                }

                foreach (var fieldName in fields)
                {
                    this.FilterInfo.FieldNames.Add(fieldName);
                }

                // Re-apply the field name.
                if (this.FilterInfo.FieldNames.Contains(selectedFieldName))
                {
                    workbenchFilter.FieldName = selectedFieldName;
                }

                break;
            }

            this.isInitialisingFieldName = false;
        }

        /// <summary>
        /// Initialises the types.
        /// </summary>
        private void InitialiseTypes()
        {
            this.FilterInfo.ItemTypes.Clear();

            var projectData = this.projectDataService.CurrentProjectData;

            if (projectData == null || projectData.ItemTypes == null)
            {
                return;
            }

            this.FilterInfo.ItemTypes.Add(Resources.String002);

            foreach (var itemType in projectData.ItemTypes.OrderBy(it => it.TypeName))
            {
                this.FilterInfo.ItemTypes.Add(itemType.TypeName);
            }
        }

        /// <summary>
        /// Called when [project data changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.ProjectDataChangedEventArgs"/> instance containing the event data.</param>
        private void OnProjectDataChanged(object sender, ProjectDataChangedEventArgs e)
        {
            this.collection.Clear();
            this.InitialiseTypes();
            this.view.WorkbenchFilter = null;
            this.FilterInfo.FilterStatus = Settings.Default.DisplayName;

            var currentProjectData = e.NewValue; 

            if (currentProjectData == null)
            {
                return;
            }

            foreach (var filter in this.LoadProjectFilters(currentProjectData))
            {
                this.collection.And(filter);
            }

            this.ApplyFilter(currentProjectData);
        }

        /// <summary>
        /// Loads the project filters.
        /// </summary>
        /// <param name="currentProjectData">The current project data.</param>
        /// <returns>The current project filters.</returns>
        private IEnumerable<WorkbenchFilter> LoadProjectFilters(IProjectData currentProjectData)
        {
            WorkbenchFilterCollection output = null;

            if (currentProjectData != null && !string.IsNullOrEmpty(currentProjectData.Filter))
            {
                var schemaStream =
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(FilterCollectionResourceLocation);

                if (schemaStream == null)
                {
                    throw new ArgumentException(Resources.String012);
                }

                var serialisedCandidate = currentProjectData.Filter;

                var filterCollectionStream = new MemoryStream();

                var encoding = serialisedCandidate
                                   .ToLower()
                                   .Contains("encoding=\"utf-16\"") ? Encoding.Unicode : Encoding.UTF8;

                using (var sw = new StreamWriter(filterCollectionStream, encoding))
                {
                    sw.Write(serialisedCandidate);
                    sw.Flush();
                    filterCollectionStream.Position = 0;

                    try
                    {
                        XmlValidationHelper.ValidateSourceStream(filterCollectionStream, schemaStream);

                        output = this.Serialiser.Deserialize(serialisedCandidate);
                    }
                    catch (XmlSchemaValidationException ex)
                    {
                        var message = string.Concat(Resources.String013, Environment.NewLine, ex.Message);

                        CommandLibrary.ApplicationExceptionCommand.Execute(message, Application.Current.MainWindow);
                    }
                }
            }

            return output ?? new WorkbenchFilterCollection();
        }

        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        private void ApplyFilter(IProjectData projectData)
        {
            if (!this.collection.Any())
            {
                projectData.Filter = string.Empty;
                this.filterService.FilterProvider = null;
                this.FilterInfo.FilterStatus = Resources.String017;
            }
            else
            {
                projectData.Filter = this.Serialiser.Serialize(this.collection);
                this.filterService.FilterProvider = this.collection.Clone() as IFilterProvider;
                this.FilterInfo.FilterStatus = string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.String032,
                    projectData.WorkbenchItems.Count(),
                    projectData.WorkbenchItems.UnfilteredList.Count());
            }

            var mainWindow = Application.Current != null ? Application.Current.MainWindow : null;
            if (mainWindow != null)
            {
                CommandLibrary.ApplicationMessageCommand.Execute(
                    this.FilterInfo.FilterStatus, mainWindow);
            }
        }
    }
}
