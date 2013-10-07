// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectData.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IProjectData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Threading;

    using DataObjects;

    /// <summary>
    /// The project data interface
    /// </summary>
    public interface IProjectData
    {
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>The name of the project.</value>
        string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the TFS URL.
        /// </summary>
        /// <value>The TFS URL.</value>
        string ProjectCollectionUrl { get; set; }

        /// <summary>
        /// Gets or sets the project iteration path.
        /// </summary>
        /// <value>The project iteration path.</value>
        string ProjectIterationPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [hide volume warning].
        /// </summary>
        /// <value><c>true</c> if [hide volume warning]; otherwise, <c>false</c>.</value>
        bool HideVolumeWarning { get; set; }

        /// <summary>
        /// Gets or sets the dispatcher.
        /// </summary>
        /// <value>The dispatcher.</value>
        Dispatcher Dispatcher { get; set; }

        /// <summary>
        /// Gets the view maps.
        /// </summary>
        /// <value>The view maps.</value>
        ObservableCollection<ViewMap> ViewMaps { get; }

        /// <summary>
        /// Gets the top level items.
        /// </summary>
        /// <value>The top level items.</value>
        IWorkbenchItemRepository WorkbenchItems { get; }

        /// <summary>
        /// Gets the project nodes.
        /// </summary>
        /// <value>The project nodes.</value>
        IDictionary<string, IProjectNode> ProjectNodes { get; }

        /// <summary>
        /// Gets the item types.
        /// </summary>
        /// <value>The item types.</value>
        ItemTypeDataCollection ItemTypes { get; }

        /// <summary>
        /// Gets the link types.
        /// </summary>
        /// <value>The link types.</value>
        ICollection<string> LinkTypes { get; }

        /// <summary>
        /// Gets or sets the project area path.
        /// </summary>
        /// <value>The project area path.</value>
        string ProjectAreaPath { get; set; }

        /// <summary>
        /// Gets or sets the project collection GUID.
        /// </summary>
        /// <value>The project collection GUID.</value>
        Guid? ProjectGuid { get; set; }

        /// <summary>
        /// Gets or sets the web access URL.
        /// </summary>
        /// <value>The web access URL.</value>
        string WebAccessUrl { get; set; }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        string Filter { get; set; }
    }
}