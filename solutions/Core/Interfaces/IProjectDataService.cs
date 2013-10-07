// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectDataService.cs" company="None">
//   None
// </copyright>
// <summary>
//   The project data service interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Interfaces
{
    using System;
    using System.Collections.Generic;

    using TfsWorkbench.Core.EventArgObjects;

    /// <summary>
    /// The project data service interface.
    /// </summary>
    public interface IProjectDataService
    {
        /// <summary>
        /// Occurs when [project data changed].
        /// </summary>
        event EventHandler<ProjectDataChangedEventArgs> ProjectDataChanged;

        /// <summary>
        /// Gets or sets the current project data.
        /// </summary>
        /// <value>The current project data.</value>
        IProjectData CurrentProjectData { get; set; }

        /// <summary>
        /// Gets or sets the current data provider.
        /// </summary>
        /// <value>The current data provider.</value>
        IDataProvider CurrentDataProvider { get; set; }

        /// <summary>
        /// Gets the highlight providers.
        /// </summary>
        /// <value>The highlight providers.</value>
        ICollection<IHighlightProvider> HighlightProviders { get; }

        /// <summary>
        /// Clears all items.
        /// </summary>
        void ClearAllCurrentProjectData();

        /// <summary>
        /// Loads the Project Data.
        /// </summary>
        /// <param name="path">The source file path.</param>
        /// <returns>The loaded project data object.</returns>
        IProjectData LoadProjectLayoutData(string path);

        /// <summary>
        /// Saves the project data.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="path">The output path.</param>
        void SaveProjectLayoutData(IProjectData projectData, string path);

        /// <summary>
        /// Gets the default file path.
        /// </summary>
        /// <param name="projectCollectionUri">The project collection URI.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>
        /// The default data file path for the specified parameters.
        /// </returns>
        string DefaultFilePath(Uri projectCollectionUri, string projectName);

        /// <summary>
        /// Generates the path filter.
        /// </summary>
        /// <param name="iterationPath">The iteration path.</param>
        /// <param name="areaPath">The area path.</param>
        /// <returns>The filter string.</returns>
        string GeneratePathFilter(string iterationPath, string areaPath);

        /// <summary>
        /// Generates the path filter.
        /// </summary>
        /// <param name="iterationPath">The iteration path.</param>
        /// <param name="areaPath">The area path.</param>
        /// <param name="additionalFilter">The additional filter.</param>
        /// <returns>The filter string.</returns>
        string GeneratePathFilter(string iterationPath, string areaPath, string additionalFilter);

        /// <summary>
        /// Creates the new child.
        /// </summary>
        /// <param name="childCreationParameters">The child creation parameters.</param>
        /// <returns>A new child item.</returns>
        IWorkbenchItem CreateNewChild(IChildCreationParameters childCreationParameters);

        /// <summary>
        /// Creates the duplicate.
        /// </summary>
        /// <param name="sourceItem">The source item.</param>
        /// <returns>A duplicate instance of the source item.</returns>
        IWorkbenchItem CreateDuplicate(IWorkbenchItem sourceItem);

        /// <summary>
        /// Creates the new item.
        /// </summary>
        /// <param name="itemTypeName">Name of the item type.</param>
        /// <returns>A new item instance.</returns>
        IWorkbenchItem CreateNewItem(string itemTypeName);

        /// <summary>
        /// Clears all.
        /// </summary>
        /// <param name="projectDataToClear">The project data to clear.</param>
        void ClearAll(IProjectData projectDataToClear);
    }
}