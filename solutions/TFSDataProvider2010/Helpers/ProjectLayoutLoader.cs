// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectLayoutLoader.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ProjectLayoutLoader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using TfsWorkbench.Core.Helpers;
using TfsWorkbench.Core.Interfaces;

namespace TfsWorkbench.TFSDataProvider2010.Helpers
{
    /// <summary>
    /// The project layout loader class.
    /// </summary>
    internal class ProjectLayoutLoader : ProjectDataServiceConsumer, IDisposable
    {
        /// <summary>
        /// The collection Uri.
        /// </summary>
        private readonly Uri collectionUri;

        /// <summary>
        /// The project name.
        /// </summary>
        private readonly string projectName;

        /// <summary>
        /// The project data instance.
        /// </summary>
        private IProjectData projectData;

        /// <summary>
        /// The tfs project instance.
        /// </summary>
        private Project tfsProject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectLayoutLoader"/> class.
        /// </summary>
        /// <param name="collectionUri">The collection URI.</param>
        /// <param name="projectName">Name of the project.</param>
        public ProjectLayoutLoader(Uri collectionUri, string projectName)
        {
            AssertParametersAreValid(collectionUri, projectName);

            this.collectionUri = collectionUri;
            this.projectName = projectName;

            this.SetFilePath();
        }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets the file load exception.
        /// </summary>
        /// <value>The file load exception.</value>
        public Exception FileLoadException { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has file load exception.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has file load exception; otherwise, <c>false</c>.
        /// </value>
        public bool HasFileLoadException
        {
            get { return this.FileLoadException != null; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Loads the project layout.
        /// </summary>
        /// <returns>A project layout data instance.</returns>
        public IProjectData LoadProjectLayout()
        {
            this.SetTfsProjectInstance();

            if (!this.TryLoadFromFile())
            {
                this.GenerateAndSaveDefaultLayout();
            }

            this.EnsureProjectGuidAndWebAccessUrlAreSet();

            return this.projectData;
        }

        /// <summary>
        /// Asserts the parameters are valid.
        /// </summary>
        /// <param name="collectionUri">The collection URI.</param>
        /// <param name="projectName">Name of the project.</param>
        private static void AssertParametersAreValid(Uri collectionUri, string projectName)
        {
            if (collectionUri == null)
            {
                throw new ArgumentNullException("collectionUri");
            }

            if (projectName == null)
            {
                throw new ArgumentNullException("projectName");
            }
        }

        /// <summary>
        /// Sets the TFS project instance.
        /// </summary>
        private void SetTfsProjectInstance()
        {
            this.tfsProject = ProjectService.Instance.GetProject(this.collectionUri, this.projectName);
        }

        /// <summary>
        /// Sets the file path.
        /// </summary>
        private void SetFilePath()
        {
            this.FilePath = this.ProjectDataService.DefaultFilePath(this.collectionUri, this.projectName);
        }

        /// <summary>
        /// Tries the load from file.
        /// </summary>
        /// <returns><c>True</c> if project loaded from file; otherwise <c>false</c>.</returns>
        private bool TryLoadFromFile()
        {
            if (this.HasExistingLayoutFile())
            {
                try
                {
                    this.LoadLayoutFromFile();
                    return true;
                }
                catch (Exception ex)
                {
                    this.FileLoadException = ex;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether [has existing layout file].
        /// </summary>
        /// <returns>
        /// <c>true</c> if [has existing layout file]; otherwise, <c>false</c>.
        /// </returns>
        private bool HasExistingLayoutFile()
        {
            return File.Exists(this.FilePath);
        }

        /// <summary>
        /// Loads the layout from file.
        /// </summary>
        private void LoadLayoutFromFile()
        {
            this.projectData = this.ProjectDataService.LoadProjectLayoutData(this.FilePath);
        }

        /// <summary>
        /// Generates and saves a default project layout instance.
        /// </summary>
        private void GenerateAndSaveDefaultLayout()
        {
            this.projectData = TfsProjectDataLoader.GenerateDefaultProjectData(this.tfsProject);
            this.ProjectDataService.SaveProjectLayoutData(this.projectData, this.FilePath);
        }

        /// <summary>
        /// Ensures the project GUID and web access URL are set.
        /// </summary>
        private void EnsureProjectGuidAndWebAccessUrlAreSet()
        {
            if (!this.projectData.ProjectGuid.HasValue)
            {
                // Get the project guid.
                this.projectData.ProjectGuid = TfsProjectDataLoader.GetProjectGuid(this.tfsProject);
            }

            if (string.IsNullOrEmpty(this.projectData.WebAccessUrl))
            {
                // Get the default web access url.
                this.projectData.WebAccessUrl =
                    Factory.BuildDefaultWebAccessUrl(this.collectionUri).AbsoluteUri;
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposeManaged"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                this.projectData = null;
                this.tfsProject = null;
                this.FileLoadException = null;
            }
        }
    }
}
