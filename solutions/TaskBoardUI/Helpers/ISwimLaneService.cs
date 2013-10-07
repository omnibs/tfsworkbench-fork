// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISwimLaneService.cs" company="None">
//   None
// </copyright>
// <summary>
//   The swim lane service interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.TaskBoardUI.Helpers
{
    using System.Collections.ObjectModel;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.TaskBoardUI.DataObjects;

    /// <summary>
    /// The swim lane service interface.
    /// </summary>
    public interface ISwimLaneService
    {
        /// <summary>
        /// Gets the swim lanes views.
        /// </summary>
        /// <value>The swim lanes views.</value>
        ObservableCollection<SwimLaneView> SwimLaneViews { get; }

        /// <summary>
        /// Generates the views.
        /// </summary>
        /// <param name="projectDataCandidate">The project data.</param>
        void Initialise(IProjectData projectDataCandidate);

        /// <summary>
        /// Releases the resources.
        /// </summary>
        void ReleaseResources();
    }
}