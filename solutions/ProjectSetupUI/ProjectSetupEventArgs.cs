// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectSetupEventArgs.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ProjectSetupEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI
{
    using System;

    using DataObjects;

    /// <summary>
    /// Initializes instance of ProjectSetupEventArgs
    /// </summary>
    internal class ProjectSetupEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectSetupEventArgs"/> class.
        /// </summary>
        /// <param name="projectSetup">The project setup.</param>
        public ProjectSetupEventArgs(ProjectSetup projectSetup)
        {
            ProjectSetup = projectSetup;
        }

        /// <summary>
        /// Gets the project setup.
        /// </summary>
        /// <value>The project setup.</value>
        public ProjectSetup ProjectSetup { get; private set; }
    }
}
