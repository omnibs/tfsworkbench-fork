// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectSetup.cs" company="None">
//   None
// </copyright>
// <summary>
//   The project data setup.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI.DataObjects
{
    using System.Collections.Specialized;
    using System.Linq;

    using Core.Interfaces;

    /// <summary>
    /// The project data setup.
    /// </summary>
    internal class ProjectSetup : DurationStructureBase
    {
        /// <summary>
        /// The project node item.
        /// </summary>
        private IProjectNode projectNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectSetup"/> class.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        public ProjectSetup(string projectName)
        {
            this.Name = projectName;

            this.Releases = new ReleaseCollection();

            this.WorkStreams = new WorkStreamCollection();

            this.Teams = new TeamCollection();

            this.Releases.CollectionChanged += this.OnReleasesChanged;
        }

        /// <summary>
        /// Gets the releases.
        /// </summary>
        /// <value>The releases.</value>
        public ReleaseCollection Releases { get; private set; }

        /// <summary>
        /// Gets the work streams.
        /// </summary>
        /// <value>The work streams.</value>
        public WorkStreamCollection WorkStreams { get; private set; }

        /// <summary>
        /// Gets the teams.
        /// </summary>
        /// <value>The teams.</value>
        public TeamCollection Teams { get; private set; }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>The name of the project.</value>
        public IProjectNode ProjectNode
        {
            get
            {
                return this.projectNode;
            }

            set
            {
                this.UpdateWithNotification("Name", value, ref this.projectNode);
            }
        }

        /// <summary>
        /// Called when [releases changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnReleasesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var minDate = this.Releases.Min(r => r.StartDate);
            var maxDate = this.Releases.Max(r => r.EndDate);

            if (minDate.HasValue)
            {
                if (!this.StartDate.HasValue || this.StartDate.Value.Ticks > minDate.Value.Ticks)
                {
                    this.StartDate = minDate.Value;
                }
            }

            if (maxDate.HasValue)
            {
                if (!this.EndDate.HasValue || this.EndDate.Value.Ticks < maxDate.Value.Ticks)
                {
                    this.EndDate = maxDate.Value;
                }
            }
        }
    }
}