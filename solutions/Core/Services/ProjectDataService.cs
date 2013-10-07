// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectDataService.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ProjectDataHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Reflection;
    using System.Text;

    using TfsWorkbench.Core.DataObjects;
    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Properties;
    using TfsWorkbench.Core.WorkbenchItemGenerators;

    /// <summary>
    /// Initializes instance of ProjectDataHelper
    /// </summary>
    internal class ProjectDataService : IProjectDataService
    {
        /// <summary>
        /// The search providers collection.
        /// </summary>
        private readonly ICollection<IHighlightProvider> highlightProviders = new Collection<IHighlightProvider>();

        /// <summary>
        /// The xml serialiser.
        /// </summary>
        private readonly SerializerInstance<ProjectData> serialiser = new SerializerInstance<ProjectData>();

        /// <summary>
        /// The current project data instance.
        /// </summary>
        private IProjectData currentProjectData;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDataService"/> class.
        /// </summary>
        public ProjectDataService()
            : this(ServiceManager.Instance.GetService<ILinkManagerService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDataService"/> class.
        /// </summary>
        /// <param name="linkManagerService">The link manager service.</param>
        public ProjectDataService(ILinkManagerService linkManagerService)
        {
            if (linkManagerService == null)
            {
                throw new ArgumentNullException("linkManagerService");
            }

            this.LinkManagerService = linkManagerService;
        }

        /// <summary>
        /// Occurs when [project data changed].
        /// </summary>
        public event EventHandler<ProjectDataChangedEventArgs> ProjectDataChanged;

        /// <summary>
        /// Gets the link manager service.
        /// </summary>
        /// <value>The link manager service.</value>
        public ILinkManagerService LinkManagerService { get; private set; }

        /// <summary>
        /// Gets or sets the current project data.
        /// </summary>
        /// <value>The current project data.</value>
        public IProjectData CurrentProjectData
        {
            get
            {
                return this.currentProjectData;
            }

            set
            {
                if (this.currentProjectData == value)
                {
                    return;
                }

                this.currentProjectData = value;

                if (this.ProjectDataChanged != null)
                {
                    this.ProjectDataChanged(this, new ProjectDataChangedEventArgs(this.currentProjectData, value));
                }
            }
        }

        /// <summary>
        /// Gets or sets the current data provider.
        /// </summary>
        /// <value>The current data provider.</value>
        public IDataProvider CurrentDataProvider { get; set; }

        /// <summary>
        /// Gets the search providers.
        /// </summary>
        /// <value>The search providers.</value>
        public ICollection<IHighlightProvider> HighlightProviders
        {
            get
            {
                return this.highlightProviders;
            }
        }

        /// <summary>
        /// Clears all items.
        /// </summary>
        public void ClearAllCurrentProjectData()
        {
            var projectData = this.CurrentProjectData;

            if (projectData == null)
            {
                return;
            }

            projectData.WorkbenchItems.Clear();
            this.ClearAll(projectData);
            this.LinkManagerService.ClearAllLinks();
        }

        /// <summary>
        /// Clears all.
        /// </summary>
        /// <param name="projectDataToClear">The project data to clear.</param>
        public void ClearAll(IProjectData projectDataToClear)
        {
            if (projectDataToClear == null)
            {
                return;
            }

            foreach (var viewMap in projectDataToClear.ViewMaps)
            {
                viewMap.OnLayoutUpdated();
            }

            foreach (var projectNode in projectDataToClear.ProjectNodes.Values)
            {
                projectNode.ClearChildren();
            }

            projectDataToClear.ProjectNodes.Clear();
            projectDataToClear.LinkTypes.Clear();
            projectDataToClear.Filter = string.Empty;
        }

        /// <summary>
        /// Loads the Project Data.
        /// </summary>
        /// <param name="path">The source file path.</param>
        /// <returns>The loaded project data object.</returns>
        public IProjectData LoadProjectLayoutData(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            ProjectData output;

            using (var fs = new FileStream(path, FileMode.Open))
            {
                XmlValidationHelper.ValidateSourceStream(fs, GetSchemaStream());
                using (var sr = new StreamReader(fs))
                {
                    output = this.serialiser.Deserialize(sr.ReadToEnd());
                    sr.BaseStream.Close();
                }
            }

            return output;
        }

        /// <summary>
        /// Saves the project data.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="path">The output path.</param>
        public void SaveProjectLayoutData(IProjectData projectData, string path)
        {
            var localProjectData = projectData as ProjectData;

            if (localProjectData == null)
            {
                return;
            }

            var directoryPath = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                sw.Write(this.serialiser.Serialize(localProjectData));
                sw.Flush();
            }
        }

        /// <summary>
        /// Gets the default file path.
        /// </summary>
        /// <param name="projectCollectionUri">The project collection URI.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>
        /// The default data file path for the specified parameters.
        /// </returns>
        public string DefaultFilePath(Uri projectCollectionUri, string projectName)
        {
            if (projectCollectionUri == null)
            {
                throw new ArgumentNullException("projectCollectionUri");
            }

            var dataFileName = string.Concat(
                projectCollectionUri.Host, projectCollectionUri.AbsolutePath.Replace("/", "."), ".", projectName, ".xml");

            var dataFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                Settings.Default.ProjectFileSubDirectory);

            dataFilePath = Path.Combine(dataFilePath, dataFileName);

            return dataFilePath;
        }

        /// <summary>
        /// Generates the path filter.
        /// </summary>
        /// <param name="iterationPath">The iteration path.</param>
        /// <param name="areaPath">The area path.</param>
        /// <returns>The filter string.</returns>
        public string GeneratePathFilter(string iterationPath, string areaPath)
        {
            return this.GeneratePathFilter(iterationPath, areaPath, null);
        }

        /// <summary>
        /// Generates the path filter.
        /// </summary>
        /// <param name="iterationPath">The iteration path.</param>
        /// <param name="areaPath">The area path.</param>
        /// <param name="additionalFilter">The addtional filter.</param>
        /// <returns>The filter string.</returns>
        public string GeneratePathFilter(string iterationPath, string areaPath, string additionalFilter)
        {
            var sb = new StringBuilder();

            Func<string> appendAnd = () => sb.Length != 0 ? " AND " : string.Empty;

            if (!string.IsNullOrEmpty(iterationPath))
            {
                sb.AppendFormat("[{0}] UNDER '{1}'", Settings.Default.IterationPathFieldName, iterationPath);
            }

            if (!string.IsNullOrEmpty(areaPath))
            {
                sb.AppendFormat("{0}[{1}] UNDER '{2}'", appendAnd(), Settings.Default.AreaPathFieldName, areaPath);
            }

            if (!string.IsNullOrEmpty(additionalFilter))
            {
                sb.AppendFormat("{0}{1}", appendAnd(), additionalFilter);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates the new item.
        /// </summary>
        /// <param name="itemTypeName">Name of the item type.</param>
        /// <returns>A new item instance.</returns>
        public IWorkbenchItem CreateNewItem(string itemTypeName)
        {
            var itemCreator = new WorkbenchItemCreator(this, itemTypeName);

            return itemCreator.Create();
        }

        /// <summary>
        /// Creates the new child.
        /// </summary>
        /// <param name="childCreationParameters">The child creation parameters.</param>
        /// <returns>A new child item.</returns>
        public IWorkbenchItem CreateNewChild(IChildCreationParameters childCreationParameters)
        {
            var childWorkbenchItemCreator = new WorkbenchItemChildCreator(this, childCreationParameters);

            return childWorkbenchItemCreator.Create();
        }

        /// <summary>
        /// Creates the duplicate.
        /// </summary>
        /// <param name="sourceItem">The source item.</param>
        /// <returns>A duplicate instance of the source item.</returns>
        public IWorkbenchItem CreateDuplicate(IWorkbenchItem sourceItem)
        {
            var duplciateWorkbenchItemCreator = new WorkbenchItemDuplicator(this, sourceItem);

            return duplciateWorkbenchItemCreator.Create();
        }

        /// <summary>
        /// Gets the schema stream.
        /// </summary>
        /// <returns>
        /// The scheam stream.
        /// </returns>
        private static Stream GetSchemaStream()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var schemaResourceStreamLocation = string.Concat(assembly.GetName().Name, ".Resources.ProjectData.xsd");
            var schemaStream = assembly.GetManifestResourceStream(schemaResourceStreamLocation);

            return schemaStream;
        }
    }
}