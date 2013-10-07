// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkbenchItemCreatorBase.cs" company="None">
//   None
// </copyright>
// <summary>
//   The workbench item creator base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.WorkbenchItemGenerators
{
    using System;

    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;

    /// <summary>
    /// The workbench item creator base class.
    /// </summary>
    internal abstract class WorkbenchItemCreatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchItemCreatorBase"/> class.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="dataProvider">The data provider.</param>
        protected WorkbenchItemCreatorBase(IProjectData projectData, IDataProvider dataProvider)
        {
            AssertParametersAreNotNull(projectData, dataProvider);

            this.ProjectData = projectData;
            this.DataProvider = dataProvider;
        }

        /// <summary>
        /// Gets the project data.
        /// </summary>
        /// <value>The project data.</value>
        protected IProjectData ProjectData { get; private set; }

        /// <summary>
        /// Gets the data provider.
        /// </summary>
        /// <value>The data provider.</value>
        protected IDataProvider DataProvider { get; private set; }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>A new instance of the child workbench item.</returns>
        public abstract IWorkbenchItem Create();

        /// <summary>
        /// Generates the new instance.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>A new instance of the specfied type.</returns>
        protected IWorkbenchItem GenerateNewInstance(string typeName)
        {
            var valueProvider = this.DataProvider.CreateValueProvider(this.ProjectData, typeName);

            var workbenchItem = Factory.BuildItem(valueProvider);

            this.ApplyDefaultValues(typeName, workbenchItem);

            return workbenchItem;
        }

        /// <summary>
        /// Asserts the parameters are not null.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="dataProvider">The data provider.</param>
        private static void AssertParametersAreNotNull(IProjectData projectData, IDataProvider dataProvider)
        {
            if (projectData == null)
            {
                throw new ArgumentNullException("projectData");
            }

            if (dataProvider == null)
            {
                throw new ArgumentNullException("dataProvider");
            }
        }

        /// <summary>
        /// Applies the default values.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        private void ApplyDefaultValues(string typeName, IWorkbenchItem workbenchItem)
        {
            workbenchItem[Properties.Settings.Default.TitleFieldName] = string.Concat("New ", typeName);

            if (!string.IsNullOrEmpty(this.ProjectData.ProjectIterationPath))
            {
                workbenchItem[Properties.Settings.Default.IterationPathFieldName] = this.ProjectData.ProjectIterationPath;
            }

            if (!string.IsNullOrEmpty(this.ProjectData.ProjectAreaPath))
            {
                workbenchItem[Properties.Settings.Default.AreaPathFieldName] = this.ProjectData.ProjectAreaPath;
            }
        }
    }
}