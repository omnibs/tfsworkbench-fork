// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectMatchCollection.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ProjectMatchCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace TfsWorkbench.TFSDataProvider2010.Helpers
{
    /// <summary>
    /// The project match collection class.
    /// </summary>
    [Serializable]
    [DesignerCategory(@"code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.workbench/ProjectMatch")]
    [XmlRoot(Namespace = "http://schemas.workbench/ProjectMatch", IsNullable = false)]
    public class ProjectMatchCollection
    {
        /// <summary>
        /// The project matchers collection.
        /// </summary>
        private readonly Collection<ProjectMatch> matchers = new Collection<ProjectMatch>();

        /// <summary>
        /// Gets the matchers.
        /// </summary>
        /// <value>The matchers.</value>
        [XmlArray("ProjectMatchers")]
        [XmlArrayItem("Match")]
        public Collection<ProjectMatch> Matchers
        {
            get
            {
                return this.matchers;
            }
        }
    }
}
