// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectData.cs" company="None">
//   None
// </copyright>
// <summary>
//   The relationship collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Threading;
    using System.Xml.Serialization;

    using Interfaces;

    using TfsWorkbench.Core.Services;

    /// <summary>
    /// The relationship collection.
    /// </summary>
    [XmlRoot(ElementName = "ProjectData", Namespace = "http://schemas.workbench/ProjectData")]
    public class ProjectData : IProjectData
    {
        /// <summary>
        /// The internal view maps collection.
        /// </summary>
        private readonly ObservableCollection<ViewMap> viewMaps = new ObservableCollection<ViewMap>();

        /// <summary>
        /// The project node map field.
        /// </summary>
        private readonly Dictionary<string, IProjectNode> projectNodeMap = new Dictionary<string, IProjectNode>();

        /// <summary>
        /// The project node map field.
        /// </summary>
        private readonly ItemTypeDataCollection itemTypeMap = new ItemTypeDataCollection();

        /// <summary>
        /// The project link types.
        /// </summary>
        private readonly Collection<string> linkTypes = new Collection<string>();

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>The name of the project.</value>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the TFS URL.
        /// </summary>
        /// <value>The TFS URL.</value>
        public string ProjectCollectionUrl { get; set; }

        /// <summary>
        /// Gets or sets the project collection GUID.
        /// </summary>
        /// <value>The project collection GUID.</value>
        public Guid? ProjectGuid { get; set; }

        /// <summary>
        /// Gets or sets the web access URL.
        /// </summary>
        /// <value>The web access URL.</value>
        public string WebAccessUrl { get; set; }

        /// <summary>
        /// Gets or sets the project iteration path.
        /// </summary>
        /// <value>The project iteration path.</value>
        public string ProjectIterationPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [hide volume warning].
        /// </summary>
        /// <value><c>true</c> if [hide volume warning]; otherwise, <c>false</c>.</value>
        [XmlAttribute("hideVolumeWarning")]
        public bool HideVolumeWarning { get; set; }

        /// <summary>
        /// Gets or sets the project area path.
        /// </summary>
        /// <value>The project area path.</value>
        public string ProjectAreaPath { get; set; }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets the dispatcher.
        /// </summary>
        /// <value>The dispatcher.</value>
        [XmlIgnore]
        public Dispatcher Dispatcher
        {
            get; set;
        }

        /// <summary>
        /// Gets the view maps.
        /// </summary>
        /// <value>The view maps.</value>
        public ObservableCollection<ViewMap> ViewMaps
        {
            get { return this.viewMaps; }
        }

        /// <summary>
        /// Gets the item type data.
        /// </summary>
        /// <value>The item type data.</value>
        public ItemTypeDataCollection ItemTypes
        {
            get { return this.itemTypeMap; }
        }

        /// <summary>
        /// Gets the top level items.
        /// </summary>
        /// <value>The top level items.</value>
        [XmlIgnore]
        public IWorkbenchItemRepository WorkbenchItems
        {
            get
            {
                return ServiceManager.Instance.GetService<IWorkbenchItemRepository>();
            }
        }

        /// <summary>
        /// Gets the project nodes.
        /// </summary>
        /// <value>The project nodes.</value>
        [XmlIgnore]
        public IDictionary<string, IProjectNode> ProjectNodes
        {
            get { return this.projectNodeMap; }
        }

        /// <summary>
        /// Gets the link types.
        /// </summary>
        /// <value>The link types.</value>
        [XmlIgnore]
        public ICollection<string> LinkTypes
        {
            get { return this.linkTypes; }
        }
    }
}