// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectDataServiceConsumer.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ProjectDataServiceConsumer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Helpers
{
    using System;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Services;

    /// <summary>
    /// The project data consumer class.
    /// </summary>
    public abstract class ProjectDataServiceConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDataServiceConsumer"/> class.
        /// </summary>
        protected ProjectDataServiceConsumer()
            : this(ServiceManager.Instance.GetService<IProjectDataService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDataServiceConsumer"/> class.
        /// </summary>
        /// <param name="projectDataService">The project data service.</param>
        protected ProjectDataServiceConsumer(IProjectDataService projectDataService)
        {
            if (projectDataService == null)
            {
                throw new ArgumentNullException("projectDataService");
            }

            this.ProjectDataService = projectDataService;
        }

        /// <summary>
        /// Gets the project data service.
        /// </summary>
        /// <value>The project data service.</value>
        protected IProjectDataService ProjectDataService { get; private set; }
    }
}
