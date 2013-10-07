// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectSelectorService.cs" company="None">
//   None.
// </copyright>
// <summary>
//   Defines the IProjectSelectorService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.ProjectSelector
{
    using System;

    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Interfaces;

    /// <summary>
    /// The project selector service interface.
    /// </summary>
    public interface IProjectSelectorService
    {
        /// <summary>
        /// Occurs when [async exception].
        /// </summary>
        event EventHandler<ExceptionEventArgs> AsyncException;

        /// <summary>
        /// Shows the project selector.
        /// </summary>
        /// <returns><c>Null</c> if no project selected; otherwise <c>IProjectData</c> instance.</returns>
        IProjectData ShowProjectSelector();

        /// <summary>
        /// Determines whether [the specified project data] [has project nodes].
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <returns>
        /// <c>true</c> if [the specified project data] [has project nodes]; otherwise, <c>false</c>.
        /// </returns>
        bool HasProjectNodes(IProjectData projectData);

        /// <summary>
        /// Begins the ensures nodes loaded function.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="callBack">The call back.</param>
        void BeginEnsureNodesLoaded(IProjectData projectData, Action callBack);

        /// <summary>
        /// Begins the volume check.
        /// </summary>
        /// <param name="projectData">The project criteria.</param>
        /// <param name="callBack">The call back method. Invoked with the work item volume as parameter.</param>
        void BeginVolumeCheck(IProjectData projectData, Action<int> callBack);

        /// <summary>
        /// Begins the load project from data provider routine.
        /// </summary>
        /// <param name="projectData">The project criteria.</param>
        /// <param name="callBack">The call back.</param>
        void BeginLoad(IProjectData projectData, Action callBack);

        /// <summary>
        /// Gets the last loaded project data.
        /// </summary>
        /// <returns>Instance of project data.</returns>
        IProjectData GetLastProjectData();

        /// <summary>
        /// Creates the loader.
        /// </summary>
        /// <param name="projectData">The project criteria.</param>
        /// <returns>An instance of the loader class.</returns>
        ILoaderWithVolumeCheck CreateLoader(IProjectData projectData);
    }
}