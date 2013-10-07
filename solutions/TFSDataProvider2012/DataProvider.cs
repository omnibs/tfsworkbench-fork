// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataProvider.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the DataProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using TfsWorkbench.Core.EventArgObjects;
using TfsWorkbench.Core.Interfaces;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using TfsWorkbench.Core.DataObjects;
using TfsWorkbench.Core.Helpers;
using TfsWorkbench.TFSDataProvider2012.Helpers;
using TfsWorkbench.TFSDataProvider2012.Properties;

namespace TfsWorkbench.TFSDataProvider2012
{
    /// <summary>
    /// Initialises and instance of TfsWorkbench.TFSDataProvider.DataProvider
    /// </summary>
    [Export(typeof(IDataProvider))]
    internal class DataProvider : ProjectDataServiceConsumer, IDataProvider
    {
        /// <summary>
        /// The filter key constant
        /// </summary>
        private const string FilterKey = "Filter";

        /// <summary>
        /// The project data key constant.
        /// </summary>
        private const string ProjectDataKey = "ProjectData";

        /// <summary>
        /// Gets or sets the data loaded method.
        /// </summary>
        /// <value>The data loaded method.</value>
        public event EventHandler<ProjectDataEventArgs> ElementDataLoaded;

        /// <summary>
        /// Occurs when [element data load error].
        /// </summary>
        public event EventHandler<ExceptionEventArgs> ElementDataLoadError;

        /// <summary>
        /// Gets or sets the save complete method.
        /// </summary>
        /// <value>The save complete method.</value>
        public event EventHandler ElementSaveComplete;

        /// <summary>
        /// Gets or sets the save error method.
        /// </summary>
        /// <value>The save error method.</value>
        public event EventHandler<WorkbenchItemSaveFailedEventArgs> ElementSaveError;

        /// <summary>
        /// Occurs on [layout load error].
        /// </summary>
        public event EventHandler<MessageEventArgs> LayoutLoadError;

        /// <summary>
        /// Occurs on [layout save error].
        /// </summary>
        public event EventHandler<MessageEventArgs> LayoutSaveError;

        /// <summary>
        /// Gets or sets the last name of the project.
        /// </summary>
        /// <value>The last name of the project.</value>
        public string LastProjectName
        {
            get
            {
                return Settings.Default.LastProjectName;
            }

            set
            {
                if (Settings.Default.LastProjectName == value)
                {
                    return;
                }

                Settings.Default.LastProjectName = value;
                Settings.Default.Save();
            }
        }

        /// <summary>
        /// Gets or sets the last project URL.
        /// </summary>
        /// <value>The last project URL.</value>
        public string LastProjectCollectionUrl
        {
            get
            {
                return Settings.Default.LastProjectCollectionUrl;
            }

            set
            {
                if (Settings.Default.LastProjectCollectionUrl == value)
                {
                    return;
                }

                Settings.Default.LastProjectCollectionUrl = value;
                Settings.Default.Save();
            }
        }

        /// <summary>
        /// Creates a new value provider.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>
        /// A new value provider for the specfied type.
        /// </returns>
        public IValueProvider CreateValueProvider(IProjectData projectData, string typeName)
        {
            AssertProjectDataIsNotNull(projectData);

            var project = ProjectService.Instance.GetProject(projectData);

            var workItemType =
                project.WorkItemTypes.Cast<WorkItemType>().FirstOrDefault(wit => wit.Name.Equals(typeName));

            if (workItemType == null)
            {
                throw new ArgumentException(Resources.String015);
            }

            return new WorkItemValueProvider { WorkItem = new WorkItem(workItemType) };
        }

        /// <summary>
        /// Gets the project data.
        /// </summary>
        /// <returns>
        /// <c>Null</c> if no project slected; otherwise the proejct data instance.
        /// </returns>
        public IProjectData ShowProjectSelector()
        {
            IProjectData output = null;

            using (var projectSelector = new ProjectSelector())
            {
                if (projectSelector.SelectProject())
                {
                    output = this.GetProjectLayout(projectSelector.CollectionUri, projectSelector.ProjectName);
                }
            }

            return output;
        }

        /// <summary>
        /// Gets the project data.
        /// </summary>
        /// <param name="projectCollectionUri">The project collection URI.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>The project data object.</returns>
        public IProjectData GetProjectLayout(Uri projectCollectionUri, string projectName)
        {
            IProjectData output;
            using (var projectLayoutLoader = new ProjectLayoutLoader(projectCollectionUri, projectName))
            {
                output = projectLayoutLoader.LoadProjectLayout();

                if (projectLayoutLoader.HasFileLoadException)
                {
                    this.OnLayoutLoadError(projectLayoutLoader.FileLoadException, projectLayoutLoader.FilePath);
                }
            }

            return output;
        }

        /// <summary>
        /// Loads the project nodes.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        public void LoadProjectNodes(IProjectData projectData)
        {
            AssertProjectDataIsNotNull(projectData);

            var project = ProjectService.Instance.GetProject(projectData);

            projectData.ProjectNodes.Clear();

            TfsProjectDataLoader.LoadProjectNodes(project, projectData);
        }

        /// <summary>
        /// Saves the project nodes.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        public void SaveProjectNodes(IProjectData projectData)
        {
            AssertProjectDataIsNotNull(projectData);

            var project = ProjectService.Instance.GetProject(projectData);

            ProjectClassificationHelper.ApplyChanges(project, projectData);

            project.Store.RefreshCache();
        }

        /// <summary>
        /// Begins the load the work item data process.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="projectData">The project data.</param>
        public void BeginLoadProjectData(string filter, IProjectData projectData)
        {
            AssertProjectDataIsNotNull(projectData);

            using (var backgroundWorker = new BackgroundWorker())
            {
                backgroundWorker.DoWork += LoadElements;

                backgroundWorker.RunWorkerCompleted += this.LoadElementsComplete;

                var args = new Dictionary<string, object> { { FilterKey, filter }, { ProjectDataKey, projectData } };

                backgroundWorker.RunWorkerAsync(args);
            }
        }

        /// <summary>
        /// Gets the item count.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="projectData">The project data.</param>
        /// <returns>A count of the number of matched items.</returns>
        public int GetItemCount(string filter, IProjectData projectData)
        {
            AssertProjectDataIsNotNull(projectData);

            var project = ProjectService.Instance.GetProject(projectData);

            return TfsProjectDataLoader.GetItemCount(project, filter);
        }

        /// <summary>
        /// Gets the revised item ids.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="projectData">The project data.</param>
        /// <param name="revisionDate">The revision date.</param>
        /// <returns>A list of all items that have been revised since the specified revision date.</returns>
        public IEnumerable<IdAndRevision> GetRevisedItemIds(string filter, IProjectData projectData, DateTime revisionDate)
        {
            AssertProjectDataIsNotNull(projectData);

            var project = ProjectService.Instance.GetProject(projectData);

            var revisions = TfsProjectDataLoader.GetRevisions(project, filter, revisionDate);

            return revisions == null
                       ? new IdAndRevision[] { }
                       : revisions.Select(w => new IdAndRevision { Id = w.Id, Revision = w.Rev }).ToArray();
        }

        /// <summary>
        /// Gets the report service end point.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <returns>
        /// The report server end point for the specfied project data.
        /// </returns>
        public Uri GetReportServiceEndPoint(IProjectData projectData)
        {
            AssertProjectDataIsNotNull(projectData);

            var project = ProjectService.Instance.GetProject(projectData);
            var tfsRegistration = project.Store.TeamProjectCollection.GetService<IRegistration>();

            var entities = tfsRegistration.GetRegistrationEntries("Reports");

            Func<ServiceInterface, bool> isReportServiceInterface =
                si =>
                "ReportWebServiceUrl".Equals(si.Name, StringComparison.OrdinalIgnoreCase) 
                || "ReportsService".Equals(si.Name, StringComparison.OrdinalIgnoreCase);

            foreach (var serviceInterface in entities.SelectMany(e => e.ServiceInterfaces).Where(isReportServiceInterface))
            {
                Uri output;
                if (Uri.TryCreate(serviceInterface.Url, UriKind.Absolute, out output))
                {
                    return output;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the report folder.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <returns><c>Null</c> if reporting folder not found; otherwise <c>folder path</c>.</returns>
        public string GetReportFolder(IProjectData projectData)
        {
            AssertProjectDataIsNotNull(projectData);

            var project = ProjectService.Instance.GetProject(projectData);
            var tfsRegistration = project.Store.TeamProjectCollection.GetService<IRegistration>();

            var entities = tfsRegistration.GetRegistrationEntries("TeamProjects");

            Func<ServiceInterface, bool> isReportFolderInterface =
                si =>
                string.Concat(projectData.ProjectName, ":", "ReportFolder").Equals(si.Name, StringComparison.OrdinalIgnoreCase);

            return entities.SelectMany(e => e.ServiceInterfaces)
                .Where(isReportFolderInterface)
                .Select(serviceInterface => serviceInterface.Url)
                .FirstOrDefault();
        }

        /// <summary>
        /// Begins the save items process.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        public void BeginSaveProjectData(IProjectData projectData)
        {
            AssertProjectDataIsNotNull(projectData);

            var backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += this.SaveItems;

            backgroundWorker.RunWorkerCompleted += this.SaveItemsComplete;

            backgroundWorker.RunWorkerAsync(projectData);
        }

        /// <summary>
        /// Gets the control collection.
        /// </summary>
        /// <param name="workbenchItem">The task board item.</param>
        /// <returns>A control item collection for the specified item.</returns>
        public IControlItemGroup GetControlItemGroup(IWorkbenchItem workbenchItem)
        {
            if (workbenchItem == null)
            {
                throw new ArgumentNullException("workbenchItem");
            }

            var valueProvider = workbenchItem.ValueProvider as WorkItemValueProvider;

            if (valueProvider == null)
            {
                throw new ArgumentException("The workbench item value provider was not of the expected type.");
            }

            return ControlItemHelper.GetControlItemGroup(workbenchItem);
        }

        /// <summary>
        /// Gets the control collection.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="workbenchItemType">Type of the workbench item.</param>
        /// <returns>
        /// A control item collection for the specified item type.
        /// </returns>
        public IControlItemGroup GetControlItemGroup(IProjectData projectData, string workbenchItemType)
        {
            AssertProjectDataIsNotNull(projectData);

            var project = ProjectService.Instance.GetProject(projectData);

            return ControlItemHelper.GetControlItemGroup(project, workbenchItemType);
        }

        /// <summary>
        /// Clears the control item collection.
        /// </summary>
        public void ClearControlItemGroup()
        {
            ControlItemHelper.ClearCache();
        }

        /// <summary>
        /// Refreshes all project data.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        public void BeginRefreshAllProjectData(IProjectData projectData)
        {
            AssertProjectDataIsNotNull(projectData);

            var iterationPath = projectData.ProjectIterationPath;
            var areaPath = projectData.ProjectAreaPath;

            // Remove all work item data
            this.ProjectDataService.ClearAllCurrentProjectData();

            var project = ProjectService.Instance.GetProject(projectData);

            project.Store.RefreshCache();
            project.Store.SyncToCache();
            ProjectService.Instance.SetActiveProject(null);

            projectData.ProjectAreaPath = areaPath;
            projectData.ProjectIterationPath = iterationPath;

            var filter = this.ProjectDataService.GeneratePathFilter(projectData.ProjectIterationPath, projectData.ProjectAreaPath);

            this.BeginLoadProjectData(filter, projectData);
        }

        /// <summary>
        /// Renames the project node.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="originalPath">The original path.</param>
        /// <param name="newName">The new name.</param>
        public void RenameProjectNode(IProjectData projectData, string originalPath, string newName)
        {
            AssertProjectDataIsNotNull(projectData);

            var project = ProjectService.Instance.GetProject(projectData);

            ProjectClassificationHelper.RenameIterationNode(project, originalPath, newName);
        }

        /// <summary>
        /// Deletes the project node.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="nodePath">The node path.</param>
        public void DeleteProjectNode(IProjectData projectData, string nodePath)
        {
            AssertProjectDataIsNotNull(projectData);

            var project = ProjectService.Instance.GetProject(projectData);

            ProjectClassificationHelper.DeleteIterationNode(project, nodePath);
        }

        /// <summary>
        /// Gets the work item edit panel.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>An instance of the work item edit panel.</returns>
        public object GetWorkItemEditPanel(IWorkbenchItem workbenchItem)
        {
            if (workbenchItem == null)
            {
                throw new ArgumentNullException("workbenchItem");
            }

            var provider = workbenchItem.ValueProvider as WorkItemValueProvider;

            if (provider == null)
            {
                throw new ArgumentException(Resources.String013);
            }

            return WorkItemEditor2012.Factory.BuildWorkItemEditPanel(provider.WorkItem);
        }

        /// <summary>
        /// Asserts the project data is not null.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        private static void AssertProjectDataIsNotNull(IProjectData projectData)
        {
            if (projectData == null)
            {
                throw new ArgumentNullException("projectData");
            }
        }

        /// <summary>
        /// Loads the items.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        private static void LoadElements(object sender, DoWorkEventArgs e)
        {
            var args = e.Argument as Dictionary<string, object>;

            if (args == null)
            {
                throw new ArgumentException(Resources.String016);
            }

            var filter = args[FilterKey] as string;
            var projectData = args[ProjectDataKey] as IProjectData;

            if (projectData == null)
            {
                throw new ArgumentException(Resources.String016);
            }

            var project = ProjectService.Instance.GetProject(projectData);
            
            TfsProjectDataLoader.LoadProjectNodes(project, projectData);
            TfsProjectDataLoader.LoadProjectWorkItems(project, filter, projectData);
            TfsProjectDataLoader.LoadWorkItemTypeData(project, projectData);
            TfsProjectDataLoader.LoadLinkTypes(project, projectData);

            e.Result = projectData;
        }

        /// <summary>
        /// Called when [layout load error].
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="filePath">The file path.</param>
        private void OnLayoutLoadError(Exception exception, string filePath)
        {
            if (this.LayoutLoadError == null)
            {
                return;
            }

            var sb = new StringBuilder();

            sb.AppendLine(Resources.String017);
            sb.AppendLine();
            sb.AppendLine(filePath);
            sb.AppendLine();
            sb.AppendLine();

            var innerException = exception;
            while (innerException != null)
            {
                sb.AppendLine(innerException.Message);

                innerException = innerException.InnerException;
            }

            this.LayoutLoadError(this, new MessageEventArgs(sb.ToString()));
        }

        /// <summary>
        /// Calls the save complete method.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void SaveItemsComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (this.LayoutSaveError == null)
                {
                    throw e.Error;
                }

                var ex = e.Error;

                var message = Resources.String018;

                while (ex != null)
                {
                    message = string.Concat(
                        message, Environment.NewLine, ex.Message);

                    ex = ex.InnerException;
                }

                this.LayoutSaveError(this, new MessageEventArgs(message));
            }

            if (this.ElementSaveComplete != null)
            {
                this.ElementSaveComplete(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Saves the items.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        private void SaveItems(object sender, DoWorkEventArgs e)
        {
            var projectData = e.Argument as IProjectData;
            if (projectData == null)
            {
                throw new ArgumentException(Resources.String016);
            }

            // If project nodes are altered, you must reload affected work items.
            this.SaveProjectNodes(projectData);

            var workbenchItems = projectData.WorkbenchItems.OrderBy(w => w.ParentLinks.Count());

            foreach (var item in workbenchItems.ToArray())
            {
                this.SaveItemIfDirty(item);
            }
        }

        /// <summary>
        /// Saves the item if dirty.
        /// </summary>
        /// <param name="workbenchItem">The task board item.</param>
        private void SaveItemIfDirty(IWorkbenchItem workbenchItem)
        {
            if (workbenchItem.ValueProvider.IsDirty)
            {
                var validationErrors = workbenchItem.ValueProvider.Save();

                if (validationErrors != null && validationErrors.Any() && this.ElementSaveError != null)
                {
                    this.ElementSaveError(this, new WorkbenchItemSaveFailedEventArgs(workbenchItem, validationErrors));
                }
            }

            foreach (var link in workbenchItem.ChildLinks.Where(l => l.Child.ValueProvider.IsDirty).ToArray())
            {
                this.SaveItemIfDirty(link.Child);
            }

            // If the item has children, call a sync job.
            if (workbenchItem.ChildLinks.Any())
            {
                workbenchItem.ValueProvider.SyncToLatest();
            }

            workbenchItem.OnPropertyChanged();
        }

        /// <summary>
        /// Loads the item complete.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void LoadElementsComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (this.ElementDataLoadError == null)
                {
                    throw e.Error;
                }

                this.ElementDataLoadError(this, new ExceptionEventArgs(e.Error));
                return;
            }

            var projectData = e.Result as IProjectData;

            if (this.ElementDataLoaded != null)
            {
                this.ElementDataLoaded(this, new ProjectDataEventArgs(projectData));
            }
        }
    }
}
