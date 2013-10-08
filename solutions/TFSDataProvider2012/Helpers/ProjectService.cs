// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectService.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ProjectService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using TfsWorkbench.Core.Interfaces;
using TfsWorkbench.TFSDataProvider2012.Properties;

namespace TfsWorkbench.TFSDataProvider2012.Helpers
{
    using TfsWorkbench.Core.DataObjects;

    /// <summary>
    /// The TFS service version.
    /// </summary>
    internal enum TfsVersion
    {
        /// <summary>
        /// An unknown server version.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// TFS version 2005.
        /// </summary>
        Tfs2005 = 1,

        /// <summary>
        /// TFS version 2008.
        /// </summary>
        Tfs2008 = 2,

        /// <summary>
        /// TFS version 2010.
        /// </summary>
        Tfs2010 = 3
    }

    /// <summary>
    /// The project service class.
    /// </summary>
    internal class ProjectService
    {
        /// <summary>
        /// The service instance.
        /// </summary>
        private static ProjectService instance;

        /// <summary>
        /// The last accessed project.
        /// </summary>
        private Project currentProject;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ProjectService Instance
        {
            get
            {
                return instance = instance ?? new ProjectService();
            }
        }

        /// <summary>
        /// Gets the project.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <returns>The corresponding TFS project object.</returns>
        public Project GetProject(IProjectData projectData)
        {
            if (projectData == null)
            {
                throw new ArgumentNullException("projectData");
            }

            if (string.IsNullOrEmpty(projectData.ProjectName))
            {
                throw new ArgumentException(Resources.String004);
            }

            Uri projectCollectionUri;
            if (!Uri.TryCreate(projectData.ProjectCollectionUrl, UriKind.Absolute, out projectCollectionUri))
            {
                throw new ArgumentException(Resources.String005);
            }

            return GetProject(projectCollectionUri, projectData.ProjectName);
        }

        /// <summary>
        /// Gets the project.
        /// </summary>
        /// <param name="projectCollectionUri">The project collection URI.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>The corresponding TFS project object.</returns>
        public Project GetProject(Uri projectCollectionUri, string projectName)
        {
            if (!IsCurrentProject(projectCollectionUri, projectName))
            {
                Project project;

                try
                {
                    var tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(projectCollectionUri);

                    tfs.EnsureAuthenticated();
                    var store = tfs.GetService<WorkItemStore>();
                    project = store.Projects.OfType<Project>().FirstOrDefault(p => p.Name.Equals(projectName));

                    if (tfs.AuthorizedIdentity != null)
                    {
                        ProjectData.CurrentUser = tfs.AuthorizedIdentity.DisplayName;
                    }
                }
                catch (Exception ex)
                {
                    var message = string.Format(CultureInfo.InvariantCulture, Resources.String006, projectCollectionUri.AbsoluteUri, projectName);
                    throw new Exception(message, ex);
                }

                if (project == null)
                {
                    var message = string.Format(CultureInfo.InvariantCulture, Resources.String007, projectCollectionUri.AbsoluteUri, projectName);
                    throw new Exception(message);
                }

                currentProject = project;
            }

            return currentProject;
        }

        /// <summary>
        /// Sets the active project.
        /// </summary>
        /// <param name="project">The project.</param>
        public void SetActiveProject(Project project)
        {
            currentProject = project;
        }

        /// <summary>
        /// Gets the service version.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>The Tfs version.</returns>
        public TfsVersion GetServiceVersion(Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            var tpc = project.Store.TeamProjectCollection;

            var locationService = tpc.GetService<ILocationService>();

            if (null !=
                locationService.LocationForCurrentConnection(
                    ServiceInterfaces.SecurityService, FrameworkServiceIdentifiers.CollectionSecurity))
            {
                return TfsVersion.Tfs2010;
            }

            const string serviceDefinition = "GroupSecurity2";

            return null !=
                   locationService.LocationForCurrentConnection(
                       serviceDefinition, IntegrationServiceIdentifiers.GroupSecurity2) ? TfsVersion.Tfs2008 : TfsVersion.Tfs2005;
        }

        /// <summary>
        /// Determines whether [the last project] matches [the specified project parameters].
        /// </summary>
        /// <param name="projectCollectionUri">The project collection URI.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>
        /// <c>true</c> if [the last project] matches [the specified project parameters]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsCurrentProject(Uri projectCollectionUri, string projectName)
        {
            return
                currentProject != null
                && currentProject.Store.TeamProjectCollection.Uri.AbsoluteUri.Equals(projectCollectionUri.AbsoluteUri, StringComparison.OrdinalIgnoreCase) 
                && currentProject.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
