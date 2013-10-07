// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectDataChangedEventArgs.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ProjectDataChangedEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.EventArgObjects
{
    using Interfaces;

    /// <summary>
    /// Initialises and instance of TfsWorkbench.Core.EventArgObjects.ProjectDataChangedEventArgs
    /// </summary>
    public class ProjectDataChangedEventArgs : OldToNewEventArgs<IProjectData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDataChangedEventArgs"/> class.
        /// </summary>
        /// <param name="oldProjectData">The old project data.</param>
        /// <param name="newProjectData">The new project data.</param>
        public ProjectDataChangedEventArgs(IProjectData oldProjectData, IProjectData newProjectData)
            : base(oldProjectData, newProjectData)
        {
        }
    }
}
