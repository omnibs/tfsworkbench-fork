// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TfsProjectDataLoader.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ProjectDataLoader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;
using TfsWorkbench.Core.DataObjects;
using TfsWorkbench.Core.Helpers;
using TfsWorkbench.Core.Interfaces;
using TfsWorkbench.Core.Properties;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using TfsWorkbench.Core.Services;

namespace TfsWorkbench.TFSDataProvider2010.Helpers
{
    /// <summary>
    /// Initialises and instance of TfsWorkbench.TFSDataProvider.ProjectDataLoader
    /// </summary>
    internal static class TfsProjectDataLoader
    {
        /// <summary>
        /// Tries to get server URI from the team foundation server object.
        /// </summary>
        /// <param name="serviceProvider">The team project collection.</param>
        /// <param name="tfsUri">The TFS URI.</param>
        /// <returns><c>True</c> if the server Uri is resolved; otherwise <c>false</c>.</returns>
        /// <remarks>This function has been added in order to handle the API change between TFS 2010 Beta 2 and LCTP3</remarks>
        public static bool TryGetServerUri(IServiceProvider serviceProvider, out Uri tfsUri)
        {
            tfsUri = null;

            if (serviceProvider != null)
            {
                // Add some reflection to handle the change in API signature.
                var serverPropInfo = serviceProvider.GetType().GetProperty("ApplicationInstance") ??
                                     serviceProvider.GetType().GetProperty("ConfigurationServer");

                if (serverPropInfo != null)
                {
                    var serverInfoInstance = serverPropInfo.GetValue(serviceProvider, null);

                    var hasServerInfo = serverInfoInstance != null;

                    var uriPropInfo = hasServerInfo
                            ? serverInfoInstance.GetType().GetProperty("Uri") 
                            : serviceProvider.GetType().GetProperty("Uri");

                    if (uriPropInfo != null)
                    {
                        tfsUri = uriPropInfo.GetValue(hasServerInfo ? serverInfoInstance : serviceProvider, null) as Uri;
                    }
                }
            }

            return tfsUri != null;
        }

        /// <summary>
        /// Generates the project data.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>A new instance of a project data object.</returns>
        public static IProjectData GenerateDefaultProjectData(Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            var projectDataFromWits = GenerateViewMaps(project);

            var projectData = 
                Factory.BuildProjectData(
                    project.Store.TeamProjectCollection.Uri,
                    GetProjectGuid(project),
                    project.Name,
                    projectDataFromWits.Item1);

            foreach (var itemTypeData in projectDataFromWits.Item2) 
            {
                projectData.ItemTypes.Add(itemTypeData);
            }

            return projectData;
        }

        /// <summary>
        /// Loads the project work items.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="projectData">The project data.</param>
        public static void LoadProjectWorkItems(Project project, string filter, IProjectData projectData)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            var startTime = DateTime.Now;

            // Load the project work items
            var query = GetMainQuery(project, filter);

            var store = project.Store;

            Func<WorkItem, IWorkbenchItem> createItem =
                w => Factory.BuildItem(new WorkItemValueProvider { WorkItem = w });

            var workItemMap = store.Query(query).OfType<WorkItem>().ToDictionary(w => w, createItem);
            
            // Wire up relations
            WireUpParentChildren(project, projectData, workItemMap);

            ServiceManager.Instance.GetService<ILinkManagerService>().SetInitialLinkStatus();

            // Add the work items
            projectData.WorkbenchItems.AddRange(workItemMap.Values.OrderByDescending(wbi => wbi.ChildLinks.Count()));

            System.Diagnostics.Debug.WriteLine(
                Properties.Resources.String010, workItemMap.Count(), DateTime.Now.Subtract(startTime));
        }

        /// <summary>
        /// Loads the project nodes.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="projectData">The project data.</param>
        public static void LoadProjectNodes(Project project, IProjectData projectData)
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

            if (!projectData.ProjectNodes.ContainsKey(Settings.Default.IterationPathFieldName))
            {
                projectData.ProjectNodes.Add(Settings.Default.IterationPathFieldName, iterationRoot);
            }
            else
            {
                projectData.ProjectNodes[Settings.Default.IterationPathFieldName] = iterationRoot;
            }

            if (!projectData.ProjectNodes.ContainsKey(Settings.Default.AreaPathFieldName))
            {
                projectData.ProjectNodes.Add(Settings.Default.AreaPathFieldName, areaRoot);
            }
            else
            {
                projectData.ProjectNodes[Settings.Default.AreaPathFieldName] = areaRoot;
            }
        }

        /// <summary>
        /// Loads the item types.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="projectData">The project data.</param>
        public static void LoadWorkItemTypeData(Project project, IProjectData projectData)
        {
            foreach (var workItemType in project.WorkItemTypes.OfType<WorkItemType>())
            {
                var witd = workItemType.Export(false);

                var fields = workItemType.FieldDefinitions.OfType<FieldDefinition>();
                
                var statesNodes = witd.SelectNodes(@"//STATE/@value");

                var controlAttributes = witd.SelectNodes(@"//Control/@FieldName");

                if (statesNodes == null || controlAttributes == null)
                {
                    continue;
                }

                ItemTypeData typeData;
                if (!projectData.ItemTypes.TryGetValue(workItemType.Name, out typeData))
                {
                    typeData = new ItemTypeData(workItemType.Name);

                    projectData.ItemTypes.Add(typeData);
                }

                foreach (var state in statesNodes.OfType<XmlAttribute>()
                    .Select(e => e.Value).Where(state => !typeData.States.Contains(state)))
                {
                    typeData.States.Add(state);
                }

                var formFields = controlAttributes.OfType<XmlAttribute>().Select(xa => xa.Value);
                
                var doubleFields = fields
                    .Where(fd => fd.FieldType == FieldType.Double)
                    .Select(fd => fd.ReferenceName);

                var integerFields = fields
                    .Where(fd => fd.FieldType == FieldType.Integer)
                    .Select(fd => fd.ReferenceName);

                var dateFields = fields
                    .Where(fd => fd.FieldType == FieldType.DateTime)
                    .Select(fd => fd.ReferenceName);

                var editFields = fields
                    .Where(fd => fd.IsEditable)
                    .Select(fd => fd.ReferenceName);

                var defaultFields = Properties.Settings.Default.DefaultFields.OfType<string>().ToArray();

                foreach (var fieldTypeData in from field in fields
                                              let refName = field.ReferenceName
                                              where !typeData.Fields.Any(f => f.ReferenceName.Equals(refName))
                                              let isMatch = (Func<string, bool>)(name => string.Equals(refName, name, StringComparison.OrdinalIgnoreCase))
                                              select new FieldTypeData
                                                  {
                                                      DisplayName = field.Name ?? refName, 
                                                      ReferenceName = refName, 
                                                      IsDisplayField = formFields.Any(isMatch) || defaultFields.Any(isMatch), 
                                                      IsDouble = doubleFields.Any(isMatch), 
                                                      IsInteger = integerFields.Any(isMatch),
                                                      IsDate = dateFields.Any(isMatch),
                                                      IsEditable = editFields.Any(isMatch)
                                                  })
                {
                    typeData.Fields.Add(fieldTypeData);
                }
            }
        }

        /// <summary>
        /// Loads the links types.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="projectData">The project data.</param>
        public static void LoadLinkTypes(Project project, IProjectData projectData)
        {
            foreach (var linkName in project.Store.WorkItemLinkTypes.Select(l => l.ReferenceName).OrderBy(l => l))
            {
                projectData.LinkTypes.Add(linkName);
            }
        }

        /// <summary>
        /// Gets the project GUID.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>The Project Guid.</returns>
        public static Guid? GetProjectGuid(Project project)
        {
            if (project == null)
            {
                return null;
            }

            // Project guid is not publically exposed, so extract from url.
            var guidString = project.Uri.AbsoluteUri.Substring(project.Uri.AbsoluteUri.LastIndexOf("/")).Replace("/", string.Empty);

            Guid projectGuidOutput;
            return Guid.TryParse(guidString, out projectGuidOutput) ? (Guid?)projectGuidOutput : null;
        }

        /// <summary>
        /// Gets the item count.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The work item count.</returns>
        public static int GetItemCount(Project project, string filter)
        {
            var workItemStore = project.Store;

            var query = GetMainQuery(project, filter);

            return workItemStore.QueryCount(query);
        }

        /// <summary>
        /// Gets the revisions.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="revisionDate">The revision date.</param>
        /// <returns>A list of revised item ids.</returns>
        public static IEnumerable<WorkItem> GetRevisions(Project project, string filter, DateTime revisionDate)
        {
            var workItemStore = project.Store;

            var filterWithTime = string.Concat(
                string.IsNullOrEmpty(filter) ? string.Empty : string.Concat(filter, " AND "),
                "[System.ChangedDate] > '",
                revisionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                "'");

            var query = GetMainQuery(project, filterWithTime);

            var wiqlQuery = new Query(workItemStore, query, null, false);

            var revisedItems = wiqlQuery.RunQuery().OfType<WorkItem>();

            return revisedItems;
        }

        /// <summary>
        /// Wires up parents and children. Returns a list of parent items.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="projectData">The project data.</param>
        /// <param name="workItemMap">The work item map.</param>
        private static void WireUpParentChildren(Project project, IProjectData projectData, IEnumerable<KeyValuePair<WorkItem, IWorkbenchItem>> workItemMap)
        {
            IEnumerable<ILinkItem> linksToAdd;

            var tfsVersion = ProjectService.Instance.GetServiceVersion(project);

            switch (tfsVersion)
            {
                case TfsVersion.Unknown:
                    throw new ArgumentException(Properties.Resources.String011);
                case TfsVersion.Tfs2010:
                    linksToAdd = GetTfs2010LinkData(project, projectData, workItemMap.Select(kvp => kvp.Value));
                    break;
                case TfsVersion.Tfs2008:
                case TfsVersion.Tfs2005:
                    linksToAdd = GetNonTfs2010LinkData(projectData.ViewMaps, workItemMap);
                    break;
                default:
                    return;
            }

            var linkManagerService = ServiceManager.Instance.GetService<ILinkManagerService>();
            foreach (var linkItem in linksToAdd)
            {
                linkManagerService.AddLinkWithoutRasingChangeEvent(linkItem);
            }
        }

        /// <summary>
        /// Gets the non TFS2010 link data.
        /// </summary>
        /// <param name="viewMaps">The view maps.</param>
        /// <param name="workItemMap">The work item map.</param>
        /// <returns>A list of the links to add.</returns>
        private static IEnumerable<ILinkItem> GetNonTfs2010LinkData(IEnumerable<ViewMap> viewMaps, IEnumerable<KeyValuePair<WorkItem, IWorkbenchItem>> workItemMap)
        {
            var infoMap = workItemMap.Select(
                kvp => new
                    {
                        kvp.Key.Id, 
                        TypeName = kvp.Key.Type.Name, 
                        WorkItem = kvp.Key, 
                        WorkbenchItem = kvp.Value
                    }).ToArray();

            var linksToAdd = new List<ILinkItem>();

            Func<int, IWorkbenchItem> getWorkbenchItem = id =>
                {
                    var instance = infoMap.FirstOrDefault(map => map.Id.Equals(id));
                    return instance == null ? null : instance.WorkbenchItem;
                };

            Func<int, string> getItemType = id =>
            {
                var instance = infoMap.FirstOrDefault(map => map.Id.Equals(id));
                return instance == null ? null : instance.TypeName;
            };

            Parallel.ForEach(
                infoMap,
                map =>
                    {
                        var workItem = map.WorkItem;
                        var parentInViews = viewMaps.Where(vm => vm.ParentTypes.Contains(map.TypeName));

                        if (!parentInViews.Any())
                        {
                            return;
                        }

                        var relatedLinks =
                            workItem.Links.OfType<RelatedLink>()
                            .Where(rl => rl.LinkTypeEnd == null || rl.LinkTypeEnd.IsForwardLink)
                            .ToArray();

                        if (!relatedLinks.Any())
                        {
                            return;
                        }

                        var workbenchItem = map.WorkbenchItem;

                        var linkInfoArray =
                            relatedLinks
                                .Select(
                                    rl =>
                                    new
                                        {
                                            rl.RelatedWorkItemId,
                                            ChildType = getItemType(rl.RelatedWorkItemId),
                                            LinkName = rl.LinkTypeEnd == null ? string.Empty : rl.LinkTypeEnd.LinkType.ReferenceName
                                        })
                                .ToArray();

                        foreach (var viewMap in parentInViews)
                        {
                            var linkName = viewMap.LinkName ?? string.Empty;
                            var childType = viewMap.ChildType;

                            linksToAdd.AddRange(
                                linkInfoArray
                                    .Where(li => li.LinkName.Equals(linkName) && childType.Equals(li.ChildType))
                                    .Select(li => Factory.BuildLinkItem(linkName, workbenchItem, getWorkbenchItem(li.RelatedWorkItemId))));
                        }
                    });

            return linksToAdd;
        }

        /// <summary>
        /// Gets the TFS2010 link data.
        /// </summary>
        /// <param name="project">The TFS project.</param>
        /// <param name="projectData">The project data.</param>
        /// <param name="workbenchItems">The workbench items.</param>
        /// <returns>An enumerable of links to add.</returns>
        private static IEnumerable<ILinkItem> GetTfs2010LinkData(Project project, IProjectData projectData, IEnumerable<IWorkbenchItem> workbenchItems)
        {
            // Get all the link info for the workspace.
            var wiql = string.Format(
                CultureInfo.InvariantCulture,
                Properties.Settings.Default.LinqQuery,
                project.Name,
                projectData.ProjectIterationPath,
                projectData.ProjectAreaPath);

            var query = new Query(project.Store, wiql);
            var results = query.RunLinkQuery().Where(wili => wili.SourceId != 0 && wili.TargetId != 0 && wili.LinkTypeId != 0);
            var itemMap = workbenchItems.ToDictionary(item => item.GetId(), item => item);

            Func<int, IWorkbenchItem> getWorkbenchItem = id =>
                {
                    IWorkbenchItem output;
                    return itemMap.TryGetValue(id, out output) ? output : null;
                };

            Func<int, string> getLinkType = id =>
                {
                    var linkTypeEnd = project.Store.WorkItemLinkTypes.LinkTypeEnds.FirstOrDefault(lte => lte.Id == id);

                    return linkTypeEnd != null && linkTypeEnd.IsForwardLink ? linkTypeEnd.LinkType.ReferenceName : null;
                };

            return results
                .Select(r => new
                    {
                        LinkType = getLinkType(r.LinkTypeId),
                        Parent = getWorkbenchItem(r.SourceId),
                        Child = getWorkbenchItem(r.TargetId)
                    })
                    .Where(o => o.LinkType != null && o.Parent != null && o.Child != null)
                    .Select(o => Factory.BuildLinkItem(o.LinkType, o.Parent, o.Child))
                    .ToArray();
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
        /// <returns>A project data instance</returns>
        private static Tuple<IEnumerable<ViewMap>, IEnumerable<ItemTypeData>> GenerateViewMaps(Project project)
        {
            var viewMaps = new List<ViewMap>();
            var workItemTypes = new List<ItemTypeData>();

            var workItemTypeMap = project.WorkItemTypes.Cast<WorkItemType>()
                .ToDictionary(w => w.Name, w => w.Export(false));

            var serialiser = new SerializerInstance<ProjectData>();

            var transform = TransformPicker(project);

            foreach (var witd in workItemTypeMap.Values)
            {
                var sb = new StringBuilder();

                using (var writer = XmlWriter.Create(sb, transform.OutputSettings))
                {
                    var navigator = witd.CreateNavigator();

                    using (var reader = navigator.ReadSubtree())
                    {
                        reader.MoveToContent();
                        transform.Transform(reader, writer);
                        reader.Close();
                    }

                    writer.Close();
                }

                var projectData = serialiser.Deserialize(sb.ToString());

                viewMaps.AddRange(projectData.ViewMaps);
                workItemTypes.AddRange(projectData.ItemTypes);
            }

            // Resolve the child states
            foreach (var viewMap in viewMaps)
            {
                // By default consider Core.Properties.Settings.Default.ExclusionState as a bucket state.
                var allStateNodes = workItemTypeMap[viewMap.ChildType].SelectNodes("//STATE/@value");

                if (allStateNodes == null)
                {
                    continue;
                }

                var allStates = allStateNodes.OfType<XmlNode>().Select(n => n.InnerText);
                var bucketStates = viewMap.BucketStates;
                var allSwimlaneStates = allStates.Where(s => !bucketStates.Contains(s));

                UpdateStateColours(viewMap, allStates);

                UpdateSwimLaneStates(viewMap, allSwimlaneStates);
            }

            return new Tuple<IEnumerable<ViewMap>, IEnumerable<ItemTypeData>>(viewMaps, workItemTypes);
        }

        /// <summary>
        /// Updates the swim lane states.
        /// </summary>
        /// <param name="viewMap">The view map.</param>
        /// <param name="swimLaneStates">The swim lane nodes.</param>
        private static void UpdateSwimLaneStates(ViewMap viewMap, IEnumerable<string> swimLaneStates)
        {
            if (viewMap == null || swimLaneStates == null)
            {
                return;
            }

            var existingStates = viewMap.SwimLaneStates;

            UpdateCollection(existingStates, swimLaneStates, (es, s) => es.Equals(s));
        }

        /// <summary>
        /// Updates the state colours.
        /// </summary>
        /// <param name="viewMap">The view map.</param>
        /// <param name="allStates">All state nodes.</param>
        private static void UpdateStateColours(ViewMap viewMap, IEnumerable<string> allStates)
        {
            if (viewMap == null || allStates == null)
            {
                return;
            }

            var newStateItemColours = allStates.Select(Factory.BuildStateColour);

            var existingStateItemColours = viewMap.StateItemColours;

            UpdateCollection(existingStateItemColours, newStateItemColours, (es, ns) => es.Value.Equals(ns.Value));
        }

        /// <summary>
        /// Updates the collection.
        /// </summary>
        /// <typeparam name="T">The collection elememt type.</typeparam>
        /// <param name="targetCollection">The target collection.</param>
        /// <param name="newItems">The new items.</param>
        /// <param name="isMatch">The item matching predicate.</param>
        private static void UpdateCollection<T>(ICollection<T> targetCollection, IEnumerable<T> newItems, Func<T, T, bool> isMatch)
        {
            var itemsToAdd = newItems.Where(ni => !targetCollection.Any(ti => isMatch(ni, ti))).ToArray();

            var itemsToRemove = targetCollection.Where(ti => !newItems.Any(ni => isMatch(ti, ni))).ToArray();

            foreach (var item in itemsToAdd)
            {
                targetCollection.Add(item);
            }

            foreach (var item in itemsToRemove)
            {
                targetCollection.Remove(item);
            }
        }

        /// <summary>
        /// Picks the Transform based on the specified project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>A compiled xslt object</returns>
        private static XslCompiledTransform TransformPicker(Project project)
        {
            var projectMatcher = ResourceHelper.GetProjectMatcher();

            var projectMatch = projectMatcher.Matchers.FirstOrDefault(m => m.IsMatch(project));

            if (projectMatch == null)
            {
                throw new ArgumentException(Properties.Resources.String012);
            }

            return ResourceHelper.GetTransform(projectMatch.WitdTransformResource);
        }

        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The work item querry.</returns>
        private static string GetMainQuery(Project project, string filter)
        {
            return string.Format(
                Properties.Settings.Default.MainQuery,
                project.Name,
                string.IsNullOrEmpty(filter) ? string.Empty : " AND ",
                filter);
        }
    }
}
