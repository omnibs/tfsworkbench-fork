// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectClassificationHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ProjectClassificationHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using TfsWorkbench.Core.Interfaces;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using TfsWorkbench.TFSDataProvider2012.Properties;

namespace TfsWorkbench.TFSDataProvider2012.Helpers
{
    /// <summary>
    /// Initializes instance of ProjectClassificationHelper
    /// </summary>
    internal static class ProjectClassificationHelper
    {
        /// <summary>
        /// The iteration node name.
        /// </summary>
        private const string IterationNodeName = "Iteration";

        /// <summary>
        /// The project model hierarchy structure name.
        /// </summary>
        private const string ProjectModelHierarchy = "ProjectModelHierarchy";

        /// <summary>
        /// The project life cycle structure name.
        /// </summary>
        private const string ProjectLifeCycle = "ProjectLifecycle";

        /// <summary>
        /// Begins the apply changes.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="projectData">The project data.</param>
        public static void ApplyChanges(Project project, IProjectData projectData)
        {
            var css = project.Store.GetService<ICommonStructureService>();

            var projectInfo = css.GetProjectFromName(project.Name);

            var projectUri = projectInfo.Uri;

            // Iterate the existing classifation nodes
            foreach (var info in css.ListStructures(projectUri))
            {
                var structureType = info.StructureType;
                if (string.IsNullOrEmpty(structureType))
                {
                    continue;
                }

                IProjectNode localRootNode = null;

                if (structureType.Equals(ProjectModelHierarchy))
                {
                    // Handle area nodes
                    localRootNode = projectData.ProjectNodes[Core.Properties.Settings.Default.AreaPathFieldName];
                }

                if (structureType == ProjectLifeCycle)
                {
                    // Handle iteraiton node
                    localRootNode = projectData.ProjectNodes[Core.Properties.Settings.Default.IterationPathFieldName];
                }

                if (localRootNode == null)
                {
                    continue;
                }

                AddMissingChildNodes(css, localRootNode, css.GetNodesXml(new[] { info.Uri }, true));
                var nodesToDelete = GetUnwantedChildNodeUris(css.GetNodesXml(new[] { info.Uri }, true), localRootNode);

                css.ClearProjectInfoCache();

                if (nodesToDelete.Any())
                {
                    css.DeleteBranches(nodesToDelete.ToArray(), info.Uri);
                }
            }

            Thread.Sleep(2000);

            const int MaxLoopCount = 20;
            var loopCount = 0;
            while (!HasPropergated(project, projectData))
            {
                loopCount++;

                if (loopCount > MaxLoopCount)
                {
                    throw new Exception(Resources.String001);
                }

                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// Renames the node.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="originalPath">The original path.</param>
        /// <param name="newName">The new name.</param>
        public static void RenameIterationNode(Project project, string originalPath, string newName)
        {
            var css = project.Store.GetService<ICommonStructureService>();

            var comparrisonPath = GetComparrisonPath(originalPath, IterationNodeName);
            var nodeInfo = css.GetNodeFromPath(comparrisonPath);
            css.RenameNode(nodeInfo.Uri, newName);

            Thread.Sleep(2000);

            project.Store.RefreshCache();
            project.Store.SyncToCache();
        }

        /// <summary>
        /// Deletes the IterationNodeName node.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="nodePath">The node path.</param>
        public static void DeleteIterationNode(Project project, string nodePath)
        {
            var css = project.Store.GetService<ICommonStructureService>();

            var comparrisonPath = GetComparrisonPath(nodePath, IterationNodeName);
            var deletedNodeInfo = css.GetNodeFromPath(comparrisonPath);
            var rootNode = css.GetNodeFromPath(string.Concat("\\", project.Name, "\\", IterationNodeName));
            css.DeleteBranches(new[] { deletedNodeInfo.Uri }, rootNode.Uri);

            Thread.Sleep(2000);

            project.Store.RefreshCache();
            project.Store.SyncToCache();
        }

        /// <summary>
        /// Determines whether the specified project nodes have propergated.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="data">The project data.</param>
        /// <returns>
        /// <c>true</c> if the specified project has propergated; otherwise, <c>false</c>.
        /// </returns>
        private static bool HasPropergated(Project project, IProjectData data)
        {
            project.Store.RefreshCache();
            project.Store.SyncToCache();

            var projectName = project.Name;

            var localProject = project.Store
                    .GetService<WorkItemStore>()
                    .Projects.OfType<Project>()
                    .FirstOrDefault(p => p.Name.Equals(projectName));

            var iterationNodes = localProject.IterationRootNodes.OfType<Node>();
            var areaNodes = localProject.AreaRootNodes.OfType<Node>();

            var leafPaths = GetAllLeafPaths(data.ProjectNodes[Core.Properties.Settings.Default.IterationPathFieldName])
                .Where(p => !p.Equals(localProject.Name));

            if (!leafPaths.All(p => PathExists(p, iterationNodes)))
            {
                return false;
            }

            leafPaths = GetAllLeafPaths(data.ProjectNodes[Core.Properties.Settings.Default.AreaPathFieldName])
                .Where(p => !p.Equals(localProject.Name));

            return leafPaths.All(p => PathExists(p, areaNodes));
        }

        /// <summary>
        /// Gets all leaf paths.
        /// </summary>
        /// <param name="projectNode">The project node.</param>
        /// <returns>A list of all leaf node paths.</returns>
        private static IEnumerable<string> GetAllLeafPaths(IProjectNode projectNode)
        {
            var leaves = new List<string>();

            if (projectNode.Children.Any())
            {
                leaves.Add(projectNode.Path);
            }
            else
            {
                foreach (var child in projectNode.Children)
                {
                    leaves.AddRange(GetAllLeafPaths(child));
                }
            }

            return leaves;
        }

        /// <summary>
        /// Determines whether [the specified path] [has matching node].
        /// </summary>
        /// <param name="path">The node path.</param>
        /// <param name="nodes">The nodes.</param>
        /// <returns>
        /// <c>true</c> if [the specified path] [has matching node]; otherwise, <c>false</c>.
        /// </returns>
        private static bool PathExists(string path, IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.Path.Equals(path))
                {
                    return true;
                }

                var children = node.ChildNodes.OfType<Node>();

                if (children.Any() && PathExists(path, children))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Syncs the nodes.
        /// </summary>
        /// <param name="css">The Common Structure Service.</param>
        /// <param name="projectNode">The project node.</param>
        /// <param name="structure">The structure.</param>
        private static void AddMissingChildNodes(ICommonStructureService css, IProjectNode projectNode, XmlNode structure)
        {
            // Find the uri for this node
            string nodeUri;
            if (!TryGetUri(projectNode, structure, out nodeUri))
            {
                throw new Exception("Error: Matching node uri not found in project classification structure.");
            }

            // Add any missing nodes.
            foreach (var child in projectNode.Children)
            {
                string childUri;
                if (!TryGetUri(child, structure, out childUri))
                {
                    childUri = css.CreateNode(child.Name, nodeUri);

                    // Refresh the node structure
                    structure = css.GetNodesXml(new[] { childUri }, true);
                }

                AddMissingChildNodes(css, child, structure);
            }
        }

        /// <summary>
        /// Gets the unwanted child node uris.
        /// </summary>
        /// <param name="structureNode">The structure node.</param>
        /// <param name="rootProjectNode">The root project node.</param>
        /// <returns>A list of unwanted child nodes.</returns>
        private static IEnumerable<string> GetUnwantedChildNodeUris(XmlNode structureNode, IProjectNode rootProjectNode)
        {
            var nodesToDelete = new List<string>();

            // Find any unwanted nodes.
            var childNodes = structureNode.SelectNodes(".//Children/Node");
            if (childNodes != null)
            {
                foreach (var node in childNodes.OfType<XmlNode>())
                {
                    var nodeIdAttribute = node.SelectSingleNode("@NodeID");

                    if (nodeIdAttribute == null)
                    {
                        throw new ArgumentException(Resources.String002);
                    }

                    var nodeUri = nodeIdAttribute.Value;

                    if (!IsRequired(node, rootProjectNode))
                    {
                        nodesToDelete.Add(nodeUri);
                    }
                    else
                    {
                        nodesToDelete.AddRange(GetUnwantedChildNodeUris(node, rootProjectNode));
                    }
                }
            }

            return nodesToDelete.AsEnumerable();
        }

        /// <summary>
        /// Determines whether the specified node is unwanted.
        /// </summary>
        /// <param name="node">The classification node.</param>
        /// <param name="rootProjectNode">The root project node.</param>
        /// <returns>
        /// <c>true</c> if the specified node is unwanted; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsRequired(XmlNode node, IProjectNode rootProjectNode)
        {
            return 
                GetComparrisonPath(node).Equals(rootProjectNode.Path) || rootProjectNode.Children.Any(child => IsRequired(node, child));
        }

        /// <summary>
        /// Tries the get URI.
        /// </summary>
        /// <param name="projectNode">The project node.</param>
        /// <param name="structure">The structure.</param>
        /// <param name="uri">The URI of the classification node.</param>
        /// <returns><c>True</c> if the matching uri is found; otherwise <c>false</c>.</returns>
        private static bool TryGetUri(IProjectNode projectNode, XmlNode structure, out string uri)
        {
            uri = default(string);

            string structureTypeName;
            if (!TryGetStructureTypeName(structure, out structureTypeName))
            {
                return false;
            }

            var comparrisonUri = GetComparrisonPath(projectNode, structureTypeName);

            var nodeId = structure.SelectSingleNode(string.Format("//Node[@Path = '{0}']/@NodeID", comparrisonUri));

            if (nodeId == null)
            {
                return false;
            }

            uri = nodeId.Value;

            return true;
        }

        /// <summary>
        /// Tries the name of the get structure type.
        /// </summary>
        /// <param name="structure">The structure.</param>
        /// <param name="structureTypeName">Name of the structure type.</param>
        /// <returns><c>True</c> if the type name is handled; otherwise <c>false</c>.</returns>
        private static bool TryGetStructureTypeName(XmlNode structure, out string structureTypeName)
        {
            structureTypeName = null;

            var structureType = GetStructureTypeFromNode(structure);

            if (structureType == ProjectLifeCycle)
            {
                structureTypeName = IterationNodeName;
            }

            if (structureType == ProjectModelHierarchy)
            {
                structureTypeName = GetModalHierarchyTypeName(structure);
            }

            return structureTypeName != null;
        }

        /// <summary>
        /// Gets the name of the modal hierarchy type.
        /// </summary>
        /// <param name="structure">The structure.</param>
        /// <returns>The model hierarchy node name attributte value.</returns>
        /// <exception cref="ArgumentException" />
        private static string GetModalHierarchyTypeName(XmlNode structure)
        {
            var nameAttribute = structure.SelectSingleNode("//@Name[1]");

            if (nameAttribute == null)
            {
                throw new ArgumentException(Resources.String002);
            }

            return nameAttribute.Value;
        }

        /// <summary>
        /// Gets the structure type from the specfied node.
        /// </summary>
        /// <param name="structure">The structure.</param>
        /// <returns>The node structure type attribute value.</returns>
        /// <exception cref="ArgumentException" />
        private static string GetStructureTypeFromNode(XmlNode structure)
        {
            var structureTypeAttribute = structure.SelectSingleNode("//@StructureType[1]");

            if (structureTypeAttribute == null)
            {
                throw new ArgumentException(Resources.String002);
            }

            return structureTypeAttribute.Value;
        }

        /// <summary>
        /// Gets the comparrison path.
        /// </summary>
        /// <param name="projectNode">The project node.</param>
        /// <param name="structureTypeName">The structure type name.</param>
        /// <returns>The comparrison path.</returns>
        private static string GetComparrisonPath(IProjectNode projectNode, string structureTypeName)
        {
            return GetComparrisonPath(projectNode.Path, structureTypeName);
        }

        /// <summary>
        /// Gets the comparrison path.
        /// </summary>
        /// <param name="path">The node path.</param>
        /// <param name="structureTypeName">Name of the structure type.</param>
        /// <returns>The comparison path.</returns>
        private static string GetComparrisonPath(string path, string structureTypeName)
        {
            var firstChildSeperaterIndex = path.IndexOf("\\");
            var projectName = firstChildSeperaterIndex == -1
                ? path
                : path.Substring(0, firstChildSeperaterIndex);

            return path.Replace(
                projectName,
                string.Concat("\\", projectName, "\\", structureTypeName));
        }

        /// <summary>
        /// Gets the comparrison path.
        /// </summary>
        /// <param name="node">The classification node.</param>
        /// <returns>The comparrison node uri.</returns>
        private static string GetComparrisonPath(XmlNode node)
        {
            string structureTypeName;
            var nodePath = node.SelectSingleNode("@Path");

            if (nodePath == null || string.IsNullOrEmpty(nodePath.Value) || !TryGetStructureTypeName(node, out structureTypeName))
            {
                throw new Exception(Resources.String003);
            }

            var classificationPath = nodePath.Value;
            var firstIndexOfChildSeperater = classificationPath.IndexOf("\\", 1);
            var projectName = classificationPath.Substring(1, firstIndexOfChildSeperater - 1);

            return classificationPath.Replace(
                string.Concat("\\", projectName, "\\", structureTypeName), 
                projectName);
        }

        /// <summary>
        /// Gets a service instance from the WorkItemStore.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="store">The store.</param>
        /// <returns><c>Service</c> instance if found; otherwise <c>Null</c>.</returns>
        private static T GetService<T>(this WorkItemStore store)
        {
            var output = default(T);

            // TFS API changes between 2010 Beta 2 and RC mean we have to use reflection to find the correct property
            const string TeamProjectCollection = "TeamProjectCollection";
            const string TeamFoundationServer = "TeamFoundationServer";

            var storeType = store.GetType();
            var propInfo = storeType.GetProperty(TeamProjectCollection)
                            ?? storeType.GetProperty(TeamFoundationServer);

            if (propInfo != null)
            {
                var serviceProvider = propInfo.GetValue(store, null) as IServiceProvider;

                if (serviceProvider != null)
                {
                    output = (T)serviceProvider.GetService(typeof(T));
                }
            }

            return output;
        }
    }
}
