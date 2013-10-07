// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectMatch.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ProjectMatcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TfsWorkbench.TFSDataProvider2010.Helpers
{
    /// <summary>
    /// The project matcher class.
    /// </summary>
    public class ProjectMatch
    {
        /// <summary>
        /// The work item type matches.
        /// </summary>
        private readonly Collection<WorkItemTypeMatch> workItemTypeMatchs = new Collection<WorkItemTypeMatch>();

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The matcher name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the XSLT resource.
        /// </summary>
        /// <value>The name of the XSLT resource.</value>
        [XmlAttribute("resource")]
        public string WitdTransformResource { get; set; }

        /// <summary>
        /// Gets the work item type matchs.
        /// </summary>
        /// <value>The work item type matchs.</value>
        [XmlArrayItem("TypeMatch")]
        public Collection<WorkItemTypeMatch> WorkItemTypeMatchs
        {
            get
            {
                return this.workItemTypeMatchs;
            }
        }

        /// <summary>
        /// Determines whether the specified project is match.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>
        /// <c>true</c> if the specified project is match; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            return this.WorkItemTypeMatchs.All(wit => wit.IsMatch(project));
        }
    }
}
