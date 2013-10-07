// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectDataEventArgs.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ProjectDataEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.EventArgObjects
{
    using Interfaces;

    /// <summary>
    /// Initializes instance of ProjectDataEventArgs
    /// </summary>
    public class ProjectDataEventArgs : ContextEventArgs<IProjectData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDataEventArgs"/> class.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        public ProjectDataEventArgs(IProjectData projectData)
            : base(projectData)
        {
        }
    }
}