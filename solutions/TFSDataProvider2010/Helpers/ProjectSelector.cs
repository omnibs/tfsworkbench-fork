// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectSelector.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ProjectSelector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using TfsWorkbench.TFSDataProvider2010.Properties;

namespace TfsWorkbench.TFSDataProvider2010.Helpers
{
    /// <summary>
    /// The project selector class.
    /// </summary>
    internal class ProjectSelector : IDisposable
    {
        /// <summary>
        /// The team project picker instance.
        /// </summary>
        private TeamProjectPicker teamProjectPicker;

        /// <summary>
        /// Gets the collection end point.
        /// </summary>
        /// <value>The collection end point.</value>
        public Uri CollectionUri { get; private set; }

        /// <summary>
        /// Gets the name of the project.
        /// </summary>
        /// <value>The name of the project.</value>
        public string ProjectName { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Selects the project.
        /// </summary>
        /// <returns><c>True</c> if a project is selected; otherwise <c>false</c>.</returns>
        public bool SelectProject()
        {
            this.CreateProjectPickerDialog();

            var hasSelectedAProject = this.HasSelectedAProject();

            if (hasSelectedAProject)
            {
                this.SetSelectedProjectDetails();
                this.SetActiveProject();
                this.SetSelectionPersistanceData();
            }

            return hasSelectedAProject;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposeManaged"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposeManaged)
        {
            if (disposeManaged && this.teamProjectPicker != null)
            {
                this.teamProjectPicker.Dispose();
                this.teamProjectPicker = null;
            }
        }

        /// <summary>
        /// Creates the project picker dialog.
        /// </summary>
        private void CreateProjectPickerDialog()
        {
            var credentalsProvider = new UICredentialsProvider();
            this.teamProjectPicker = new TeamProjectPicker(TeamProjectPickerMode.SingleProject, false, credentalsProvider);
            this.teamProjectPicker.SetDefaultSelectionProvider(new DefaultSelectionProvider());
        }

        /// <summary>
        /// Determines whether the user [has selected A project].
        /// </summary>
        /// <returns>
        /// <c>true</c> if the user [has selected A project]; otherwise, <c>false</c>.
        /// </returns>
        private bool HasSelectedAProject()
        {
            return this.teamProjectPicker.ShowDialog() == DialogResult.OK;
        }

        /// <summary>
        /// Sets the selected project details.
        /// </summary>
        private void SetSelectedProjectDetails()
        {
            this.ProjectName = this.teamProjectPicker.SelectedProjects.First().Name;
            this.CollectionUri = this.teamProjectPicker.SelectedTeamProjectCollection.Uri;
        }

        /// <summary>
        /// Sets the active project.
        /// </summary>
        private void SetActiveProject()
        {
            var workItemStore = this.GetWorkItemStore();
            var selectedProject = workItemStore.Projects.Cast<Project>().FirstOrDefault(p => p.Name.Equals(this.ProjectName));
            ProjectService.Instance.SetActiveProject(selectedProject);
        }

        /// <summary>
        /// Sets the selection persistance data.
        /// </summary>
        private void SetSelectionPersistanceData()
        {
            Uri tfsUri;
            if (TfsProjectDataLoader.TryGetServerUri(this.teamProjectPicker.SelectedTeamProjectCollection, out tfsUri))
            {
                Settings.Default.LastTfsUrl = tfsUri.AbsoluteUri;
            }

            Settings.Default.LastProjectName = this.ProjectName;
            Settings.Default.LastCollectionGuid = this.teamProjectPicker.SelectedTeamProjectCollection.InstanceId.ToString();
            Settings.Default.LastProjectCollectionUrl = this.CollectionUri.AbsoluteUri;

            Settings.Default.Save();
        }

        /// <summary>
        /// Gets the work item store.
        /// </summary>
        /// <returns>The work item store instance.</returns>
        private WorkItemStore GetWorkItemStore()
        {
            var workItemStore = this.teamProjectPicker.SelectedTeamProjectCollection.GetService<WorkItemStore>();
            workItemStore.RefreshCache();
            return workItemStore;
        }
    }
}