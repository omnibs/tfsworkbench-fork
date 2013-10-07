// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataProvider.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IDataProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Interfaces
{
    using System;
    using System.Collections.Generic;

    using EventArgObjects;

    using TfsWorkbench.Core.DataObjects;

    /// <summary>
    /// The Data Provider interface
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Gets or sets the data loaded method.
        /// </summary>
        /// <value>The data loaded method.</value>
        event EventHandler<ProjectDataEventArgs> ElementDataLoaded;

        /// <summary>
        /// Gets or sets the save complete method.
        /// </summary>
        /// <value>The save complete method.</value>
        event EventHandler ElementSaveComplete;

        /// <summary>
        /// Gets or sets the save error method.
        /// </summary>
        /// <value>The save error method.</value>
        event EventHandler<WorkbenchItemSaveFailedEventArgs> ElementSaveError;

        /// <summary>
        /// Occurs on [layout load error].
        /// </summary>
        event EventHandler<MessageEventArgs> LayoutLoadError;

        /// <summary>
        /// Occurs on [layout save error].
        /// </summary>
        event EventHandler<MessageEventArgs> LayoutSaveError;

        /// <summary>
        /// Occurs when [element data load error].
        /// </summary>
        event EventHandler<ExceptionEventArgs> ElementDataLoadError;

        /// <summary>
        /// Gets or sets the last project URL.
        /// </summary>
        /// <value>The last project URL.</value>
        string LastProjectCollectionUrl { get; set; } 

        /// <summary>
        /// Gets or sets the last name of the project.
        /// </summary>
        /// <value>The last name of the project.</value>
        string LastProjectName { get; set; }

        /// <summary>
        /// Creates a new value provider.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>A new value provider for the specfied type.</returns>
        IValueProvider CreateValueProvider(IProjectData projectData, string typeName);

        /// <summary>
        /// Shows the project selector dialog.
        /// </summary>
        /// <returns>The project data.</returns>
        IProjectData ShowProjectSelector();

        /// <summary>
        /// Gets the project data.
        /// </summary>
        /// <param name="projectCollectionUri">The project collection URI.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>The project data object.</returns>
        IProjectData GetProjectLayout(Uri projectCollectionUri, string projectName);

        /// <summary>
        /// Loads the work item data.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="projectData">The view map collection.</param>
        /// <exception cref="ArgumentException"></exception>
        void BeginLoadProjectData(string filter, IProjectData projectData);

        /// <summary>
        /// Begins the save items process.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        void BeginSaveProjectData(IProjectData projectData);

        /// <summary>
        /// Refreshes all project data.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        void BeginRefreshAllProjectData(IProjectData projectData);

        /// <summary>
        /// Loads the project nodes.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        void LoadProjectNodes(IProjectData projectData);

        /// <summary>
        /// Saves the project nodes.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        void SaveProjectNodes(IProjectData projectData);

        /// <summary>
        /// Renames the project node.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="originalPath">The original path.</param>
        /// <param name="newName">The new name.</param>
        void RenameProjectNode(IProjectData projectData, string originalPath, string newName);

        /// <summary>
        /// Deletes the project node.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="nodePath">The node path.</param>
        void DeleteProjectNode(IProjectData projectData, string nodePath);

        /// <summary>
        /// Clears the control item collection.
        /// </summary>
        void ClearControlItemGroup();

        /// <summary>
        /// Gets the control collection.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>A control item collection for the specified workbench item.</returns>
        IControlItemGroup GetControlItemGroup(IWorkbenchItem workbenchItem);

        /// <summary>
        /// Gets the control collection.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="workbenchItemType">Type of the workbench item.</param>
        /// <returns>
        /// A control item collection for the specified item type.
        /// </returns>
        IControlItemGroup GetControlItemGroup(IProjectData projectData, string workbenchItemType);

        /// <summary>
        /// Gets the item count.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="projectData">The project data.</param>
        /// <returns>A count of the number of matched items.</returns>
        int GetItemCount(string filter, IProjectData projectData);

        /// <summary>
        /// Gets the revised item ids.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="projectData">The project data.</param>
        /// <param name="revisionDate">The revision date.</param>
        /// <returns>A list of all item ids (and revision numbers) that have been revised since the specified revision date.</returns>
        IEnumerable<IdAndRevision> GetRevisedItemIds(string filter, IProjectData projectData, DateTime revisionDate);

        /// <summary>
        /// Gets the report service end point.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <returns>The report server end point for the specfied project data.</returns>
        Uri GetReportServiceEndPoint(IProjectData projectData);

        /// <summary>
        /// Gets the report folder.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <returns><c>Null</c> if reporting folder not found; otherwise <c>folder path</c>.</returns>
        string GetReportFolder(IProjectData projectData);

        /// <summary>
        /// Gets the work item edit panel.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>An instance of the work item edit panel.</returns>
        object GetWorkItemEditPanel(IWorkbenchItem workbenchItem);
    }
}