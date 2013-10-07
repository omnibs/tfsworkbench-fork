// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkbenchItemCreator.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the WorkbenchItemCreator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.WorkbenchItemGenerators
{
    using System;

    using TfsWorkbench.Core.Interfaces;

    /// <summary>
    /// The workbench item creator class.
    /// </summary>
    internal class WorkbenchItemCreator : WorkbenchItemCreatorBase
    {
        /// <summary>
        /// The type name.
        /// </summary>
        private readonly string typeName;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchItemCreator"/> class.
        /// </summary>
        /// <param name="projectDataService">The project data service.</param>
        /// <param name="typeName">Name of the type.</param>
        public WorkbenchItemCreator(IProjectDataService projectDataService, string typeName)
            : base(projectDataService.CurrentProjectData, projectDataService.CurrentDataProvider)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException("typeName");
            }

            this.typeName = typeName;
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>A new instance of the child workbench item.</returns>
        public override IWorkbenchItem Create()
        {
            return this.GenerateNewInstance(this.typeName);
        }
    }
}