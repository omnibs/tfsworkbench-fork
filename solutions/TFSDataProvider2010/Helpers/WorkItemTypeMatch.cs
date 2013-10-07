// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkItemTypeMatch.cs" company="None">
//   None
// </copyright>
// <summary>
//   The work item type match class.
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
    /// The work item type match class.
    /// </summary>
    [Serializable]
    public class WorkItemTypeMatch
    {
        /// <summary>
        /// The expected field name colltion.
        /// </summary>
        private readonly Collection<string> expectedFieldNames = new Collection<string>();

        /// <summary>
        /// Gets or sets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        [XmlAttribute(AttributeName = "name")]
        public string TypeName { get; set; }

        /// <summary>
        /// Gets the expected field names.
        /// </summary>
        /// <value>The expected field names.</value>
        [XmlArrayItem("Field")]
        public Collection<string> ExpectedFieldNames
        {
            get
            {
                return this.expectedFieldNames;
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
            var workItemType = project.WorkItemTypes.OfType<WorkItemType>().FirstOrDefault(wit => wit.Name.Equals(this.TypeName));

            if (workItemType != null)
            {
                return
                    this.ExpectedFieldNames.All(
                        fn =>
                        workItemType.FieldDefinitions.OfType<FieldDefinition>().Any(fd => fd.ReferenceName.Equals(fn)));
            }

            return false;
        }
    }
}