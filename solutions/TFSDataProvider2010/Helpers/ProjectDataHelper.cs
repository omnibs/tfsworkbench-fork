// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectDataHelper.cs" company="EMC Consulting">
//   EMC Consulting 2009
// </copyright>
// <summary>
//   Defines the ProjectDataHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Emcc.TeamSystem.TaskBoard.TFSDataProvider
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Xsl;

    using Core.DataObjects;
    using Core.Helpers;
    using Core.Interfaces;

    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    using Properties;

    /// <summary>
    /// Initialises and instance of Emcc.TeamSystem.TaskBoard.TFSDataProvider.ProjectDataHelper
    /// </summary>
    internal static class ProjectDataHelper
    {
        /// <summary>
        /// The internal xsl transform instance.
        /// </summary>
        private static XslCompiledTransform internalXslTransform;

        /// <summary>
        /// Gets the XSL transform.
        /// </summary>
        /// <value>The XSL transform.</value>
        private static XslCompiledTransform XslTransform
        {
            get
            {
                if (internalXslTransform == null)
                {
                    internalXslTransform = new XslCompiledTransform();

                    var assembly = Assembly.GetExecutingAssembly();

                    var assemblyName = assembly.GetName().Name;

                    var streamName = string.Concat(assemblyName, ".Resources.WitdToViewMap.xslt");
                    var stream = assembly.GetManifestResourceStream(streamName);

                    if (stream == null)
                    {
                        throw new FileNotFoundException(string.Concat("Unable to load the xslt resource file: ", streamName));
                    }

                    internalXslTransform.Load(new XmlTextReader(stream));
                }

                return internalXslTransform;
            }
        }

        /// <summary>
        /// Generates the project data.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>A new instance of a project data object.</returns>
        public static ProjectData GenerateDefaultProjectData(Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            var projectData = new ProjectData
                {
                    ProjectCollectionUrl = project.Store.TeamFoundationServer.Uri.AbsoluteUri,
                    ProjectName = project.Name
                };

            foreach (var viewMap in GenerateViewMaps(project))
            {
                projectData.ViewMaps.Add(viewMap);
            }

            return projectData;
        }

        /// <summary>
        /// Loads the project work items.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="projectData">The project data.</param>
        public static void LoadProjectWorkItems(Project project, string filter, ProjectData projectData)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            // Load the project work items
            var query = string.Format(
                "SELECT [System.Id] FROM [WorkItems] WHERE [System.TeamProject] = '{0}'{1}{2}",
                projectData.ProjectName,
                string.IsNullOrEmpty(filter) ? string.Empty : " AND ",
                filter);

            var store = project.Store;

            Func<WorkItem, ITaskBoardItem> createItem =
                w => Factory.BuildItem(new WorkItemValueProvider { WorkItem = w });

            var workItemMap = store.Query(query).OfType<WorkItem>().ToDictionary(w => w, createItem);

            Func<int, ITaskBoardItem> getItemById = id => workItemMap.Values.FirstOrDefault(tbi => tbi[Core.Properties.Settings.Default.IdFieldName].Equals(id));

            // Wire up relations
            foreach (var viewMap in projectData.ViewMaps)
            {
                var map = viewMap;
                var linkTypeName = string.IsNullOrEmpty(map.LinkName) ? null : string.Concat(map.LinkName, "-Forward");
                var parentMap = workItemMap.Where(kvp => kvp.Key.Type.Name.Equals(map.ParentType));

                // Add the child items for this view
                foreach (var pair in parentMap)
                {
                    var workItem = pair.Key;
                    var taskBoardItem = pair.Value;

                    Func<RelatedLink, bool> linkPredicate;
                    
                    if (string.IsNullOrEmpty(linkTypeName))
                    {
                        linkPredicate = rl => rl.LinkTypeEnd == null;
                    }
                    else
                    {
                        linkPredicate = rl => rl.LinkTypeEnd.ImmutableName.Equals(linkTypeName);
                    }

                    var relatedLinks = workItem.Links.OfType<RelatedLink>().Where(linkPredicate);

                    foreach (var relatedLink in relatedLinks)
                    {
                        var linkedItem = getItemById(relatedLink.RelatedWorkItemId);

                        if (linkedItem == null)
                        {
                            continue;
                        }

                        if (linkedItem[Core.Properties.Settings.Default.TypeFieldName].Equals(map.ChildType))
                        {
                            taskBoardItem.Children.Add(linkedItem);

                            if (!linkedItem.LinkNames.Contains(map.LinkName))
                            {
                                linkedItem.LinkNames.Add(map.LinkName);
                            }
                        }
                    }
                }

                // If we have any child items that do not have a parent, add them to the orphans collection
                var orphans =
                    workItemMap.Values.Where(
                        tbi => tbi[Core.Properties.Settings.Default.TypeFieldName].Equals(map.ChildType) && !tbi.LinkNames.Contains(map.LinkName));

                foreach (var orphan in orphans)
                {
                    projectData.AddOrphanItem(orphan);
                }
            }

            foreach (var parentItem in workItemMap.Values)
            {
                projectData.AddTopLevelItem(parentItem);
            }
        }

        /// <summary>
        /// Loads the project nodes.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="projectData">The project data.</param>
        public static void LoadProjectNodes(Project project, ProjectData projectData)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            var iterationRoot = Factory.BuildProjectNode(null, projectData.ProjectName);
            var areaRoot = Factory.BuildProjectNode(null, projectData.ProjectName);

            foreach (var node in project.IterationRootNodes.OfType<Node>())
            {
                var projectNode = Factory.BuildProjectNode(iterationRoot, node.Name);
                BuildChildNodes(node, projectNode);
            }

            foreach (var node in project.AreaRootNodes.OfType<Node>())
            {
                var projectNode = Factory.BuildProjectNode(areaRoot, node.Name);
                BuildChildNodes(node, projectNode);
            }

            if (!projectData.ProjectNodes.ContainsKey(Core.Properties.Settings.Default.IterationPathFieldName))
            {
                projectData.ProjectNodes.Add(Core.Properties.Settings.Default.IterationPathFieldName, iterationRoot);
            }
            else
            {
                projectData.ProjectNodes[Core.Properties.Settings.Default.IterationPathFieldName] = iterationRoot;
            }

            if (!projectData.ProjectNodes.ContainsKey(Core.Properties.Settings.Default.AreaPathFieldName))
            {
                projectData.ProjectNodes.Add(Core.Properties.Settings.Default.AreaPathFieldName, areaRoot);
            }
            else
            {
                projectData.ProjectNodes[Core.Properties.Settings.Default.AreaPathFieldName] = areaRoot;
            }
        }

        /// <summary>
        /// Builds the child nodes.
        /// </summary>
        /// <param name="node">The tfs node.</param>
        /// <param name="projectNode">The project node.</param>
        private static void BuildChildNodes(Node node, IProjectNode projectNode)
        {
            foreach (var childNode in node.ChildNodes.OfType<Node>())
            {
                var childProjectNode = Factory.BuildProjectNode(projectNode, childNode.Name);

                BuildChildNodes(childNode, childProjectNode);
            }
        }

        /// <summary>
        /// Generates the view maps.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>A collection of view map objects.</returns>
        private static IEnumerable<ViewMap> GenerateViewMaps(Project project)
        {
            var output = new List<ViewMap>();

            var workItemTypeMap = project.WorkItemTypes.Cast<WorkItemType>()
                .ToDictionary(w => w.Name, w => w.Export(false));

            var serialiser = new SerializerInstance<ProjectData>();

            foreach (var witd in workItemTypeMap.Values)
            {
                var sb = new StringBuilder();

                using (var writer = XmlWriter.Create(sb, XslTransform.OutputSettings))
                {
                    var navigator = witd.CreateNavigator();

                    if (writer == null || navigator == null)
                    {
                        throw new ArgumentException("Witd xml is not valid");
                    }

                    using (var reader = navigator.ReadSubtree())
                    {
                        reader.MoveToContent();
                        XslTransform.Transform(reader, writer);
                        reader.Close();
                    }

                    writer.Close();
                }

                var projectData = serialiser.Deserialize(sb.ToString());

                output.AddRange(projectData.ViewMaps);
            }

            // Resolve the child states
            foreach (var viewMap in output)
            {
                // By default consider "Deleted" as a bucket state.
                var swimLaneNodes = workItemTypeMap[viewMap.ChildType].SelectNodes("//STATE[@value != 'Deleted']/@value");
                var bucketNodes = workItemTypeMap[viewMap.ChildType].SelectNodes("//STATE[@value = 'Deleted']/@value");

                if (swimLaneNodes != null)
                {
                    foreach (var state in swimLaneNodes.Cast<XmlNode>().Select(n => n.InnerText))
                    {
                        viewMap.SwimLaneStates.Add(state);
                    }
                }

                if (bucketNodes != null)
                {
                    foreach (var state in bucketNodes.Cast<XmlNode>().Select(n => n.InnerText))
                    {
                        viewMap.BucketStates.Add(state);
                    }
                }
            }

            return output;
        }
    }
}
