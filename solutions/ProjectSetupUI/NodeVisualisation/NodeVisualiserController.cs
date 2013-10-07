// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeVisualiserController.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the NodeVisualiserController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI.NodeVisualisation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Core.Helpers;
    using Core.Interfaces;

    using DataObjects;

    using Helpers;

    using Properties;

    using TfsWorkbench.Core.EventArgObjects;

    using UIElements;

    /// <summary>
    /// Initializes instance of NodeVisualiserController
    /// </summary>
    internal class NodeVisualiserController
    {
        /// <summary>
        /// The depth to type map.
        /// </summary>
        private readonly Dictionary<int, string> depthMap;

        /// <summary>
        /// The visualise control.
        /// </summary>
        private readonly NodeVisualiser visualiser;

        /// <summary>
        /// Private field to indicate if a layout update is queued.
        /// </summary>
        private bool hasPendingLayoutReset;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeVisualiserController"/> class.
        /// </summary>
        /// <param name="visualiser">The visualiser.</param>
        public NodeVisualiserController(NodeVisualiser visualiser)
        {
            this.visualiser = visualiser;

            this.SetupCommandBindings();

            this.depthMap = new Dictionary<int, string>
                {
                    { Settings.Default.ReleaseDepth, Settings.Default.ReleaseType },
                    { Settings.Default.SprintDepth, Settings.Default.SprintType },
                    { Settings.Default.TeamDepth, Settings.Default.TeamType }
                };
        }

        /// <summary>
        /// Visualises the project layout.
        /// </summary>
        public void VisualiseProjectLayout()
        {
            this.visualiser.PART_LayoutCanvas.Children.Clear();

            var projectData = this.visualiser.ProjectData;

            if (projectData == null)
            {
                return;
            }

            if (SetupControllerHelper.IsValidScrumProject(projectData))
            {
                this.AssociateWorkbenchItem(this.visualiser.RootNode);
            }

            NodeLayoutHelper.RenderProjectNodeVisuals(this.visualiser.PART_LayoutCanvas, this.visualiser.NodeFilter, this.visualiser.RootNode);
        }

        /// <summary>
        /// Called when [collectionchanged].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.RepositoryChangedEventArgs&lt;IWorkbenchItem&gt;"/> instance containing the event data.</param>
        public void OnCollectionchanged(object sender, RepositoryChangedEventArgs<IWorkbenchItem> e)
        {
            Action execute = () =>
                {
                    switch (e.Action)
                    {
                        case ChangeActionOption.Add:
                            if (e.Context.Any(w => this.depthMap.ContainsValue(w.GetTypeName()))) 
                            {
                                this.EnqueueLayoutReset();
                            }

                            break;
                        case ChangeActionOption.Remove:
                            if (e.Context.Any(w => this.FindAssociatedNode(this.visualiser.RootNode, w) != null))
                            {
                                this.EnqueueLayoutReset();
                            }

                            break;
                        case ChangeActionOption.Clear:
                        case ChangeActionOption.Refresh:
                            this.EnqueueLayoutReset();

                            break;
                        default:
                            return;
                    }
                };

            if (Thread.CurrentThread.Equals(this.visualiser.Dispatcher.Thread))
            {
                execute();
            }
            else
            {
                this.visualiser.Dispatcher.BeginInvoke(DispatcherPriority.Background, execute);
            }
        }

        /// <summary>
        /// Determines whether the specified sender [can execute alter node].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanExecuteAlterNode(object sender, CanExecuteRoutedEventArgs e)
        {
            var node = e.Parameter as ProjectNodeVisual;
            e.CanExecute = node != null && node.Parent != null;
        }

        /// <summary>
        /// Enqueues the layout reset.
        /// </summary>
        private void EnqueueLayoutReset()
        {
            if (this.hasPendingLayoutReset)
            {
                return;
            }

            this.hasPendingLayoutReset = true;

            var callback = new EventHandler(
                (o, a) =>
                    {
                        this.visualiser.ResetLayout();
                        this.hasPendingLayoutReset = false;

                        ((DispatcherTimer)o).Stop();
                    });

            var timer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(500),
                DispatcherPriority.SystemIdle,
                callback,
                this.visualiser.Dispatcher);

            timer.Start();
        }

        /// <summary>
        /// Finds the associated node.
        /// </summary>
        /// <param name="projectNodeVisual">The project node visual.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>The matched node if found; otherwise <c>null</c>.</returns>
        private ProjectNodeVisual FindAssociatedNode(ProjectNodeVisual projectNodeVisual, IWorkbenchItem workbenchItem)
        {
            if (projectNodeVisual.WorkbenchItem != null && projectNodeVisual.WorkbenchItem.Equals(workbenchItem))
            {
                return projectNodeVisual;
            }

            return 
                projectNodeVisual.Children.OfType<ProjectNodeVisual>()
                .Select(child => this.FindAssociatedNode(child, workbenchItem))
                .FirstOrDefault(match => match != null);
        }

        /// <summary>
        /// Associates the workbench item.
        /// </summary>
        /// <param name="projectNode">The project node.</param>
        private void AssociateWorkbenchItem(ProjectNodeVisual projectNode)
        {
            var workbenchItems =
                this.visualiser.ProjectData.WorkbenchItems.Where(
                    wi =>
                    this.depthMap.Values.Contains(wi.GetTypeName()) &&
                    !wi.GetState().Equals(Core.Properties.Settings.Default.ExclusionState)).ToArray();

            // Create a list of map objects to reduce data provider round trips.
            var validItems = workbenchItems
                        .Select(wi => new { Path = wi[Core.Properties.Settings.Default.IterationPathFieldName].ToString(), Type = wi.GetTypeName(), Item = wi })
                        .ToArray();

            Func<string, string, IWorkbenchItem> findMatchedItem = (path, type) =>
            {
                var match = validItems.FirstOrDefault(vi => vi.Path.Equals(path) && vi.Type.Equals(type));
                return match == null ? null : match.Item;
            };

            this.AssociateWorkbenchItem(projectNode, findMatchedItem);
        }

        /// <summary>
        /// Associates the workbench item.
        /// </summary>
        /// <param name="projectNode">The project node.</param>
        /// <param name="findMatchedItem">The method used to find a matched item.</param>
        private void AssociateWorkbenchItem(ProjectNodeVisual projectNode, Func<string, string, IWorkbenchItem> findMatchedItem)
        {
            if (findMatchedItem == null)
            {
                throw new ArgumentNullException("findMatchedItem");
            }

            string depthType;

            if (this.depthMap.TryGetValue(projectNode.Depth, out depthType))
            {
                projectNode.WorkbenchItem = findMatchedItem(projectNode.Path, depthType);
            }

            foreach (var child in projectNode.Children.OfType<ProjectNodeVisual>())
            {
                this.AssociateWorkbenchItem(child, findMatchedItem);
            }
        }

        /// <summary>
        /// Sets up the command bindings.
        /// </summary>
        private void SetupCommandBindings()
        {
            this.visualiser.CommandBindings.Add(
                new CommandBinding(
                    LocalCommandLibrary.FilterOnNodeCommand,
                    this.FilterOnNode));

            this.visualiser.CommandBindings.Add(
                new CommandBinding(
                    LocalCommandLibrary.AddChildNodeCommand,
                    this.AddChildNode));

            this.visualiser.CommandBindings.Add(
                new CommandBinding(
                    LocalCommandLibrary.RenameNodeCommand,
                    this.RenameNode,
                    CanExecuteAlterNode));

            this.visualiser.CommandBindings.Add(
                new CommandBinding(
                    LocalCommandLibrary.DeleteNodeCommand,
                    this.DeleteNode,
                    CanExecuteAlterNode));

            this.visualiser.CommandBindings.Add(
                new CommandBinding(
                    LocalCommandLibrary.NewAssociatedItemCommand,
                    this.NewAssociatedItem,
                    this.CanExecuteNewAssociatedItem));
        }

        /// <summary>
        /// Determines whether the specified sender [can execute new associated item].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanExecuteNewAssociatedItem(object sender, CanExecuteRoutedEventArgs e)
        {
            var projectNode = e.Parameter as ProjectNodeVisual;

            var projectData = this.visualiser.ProjectData;

            var loadedPath = projectData.ProjectIterationPath;

            e.CanExecute = 
                SetupControllerHelper.IsValidScrumProject(this.visualiser.ProjectData) 
                && projectNode != null 
                && projectNode.WorkbenchItem == null 
                && this.depthMap.Keys.Contains(projectNode.Depth)
                && projectNode.Path.StartsWith(loadedPath);
        }

        /// <summary>
        /// Creates a new associated item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void NewAssociatedItem(object sender, ExecutedRoutedEventArgs e)
        {
            var projectNode = e.Parameter as ProjectNodeVisual;
            var dataProvider = this.visualiser.DataProvider;
            var projectData = this.visualiser.ProjectData;
            string itemType;

            if (projectNode == null 
                || dataProvider == null 
                || projectData == null 
                || !this.depthMap.TryGetValue(projectNode.Depth, out itemType))
            {
                return;
            }

            this.CreateAssociatedItem(projectData, projectNode);

            NodeLayoutHelper.ClearVisuals(this.visualiser.PART_LayoutCanvas, projectNode);

            this.VisualiseProjectLayout();
        }

        /// <summary>
        /// Gets the items under the specified path.
        /// </summary>
        /// <param name="path">The iteration path.</param>
        /// <returns>An array of loaded items that are under the specified path.</returns>
        private IEnumerable<IWorkbenchItem> GetItemsUnderPath(string path)
        {
            var projectData = this.visualiser.ProjectData;
            if (projectData == null)
            {
                return null;
            }

            var affectedItems =
                projectData.WorkbenchItems.Where(
                    wi => wi[Core.Properties.Settings.Default.IterationPathFieldName].ToString().StartsWith(path)).ToArray();

            return affectedItems;
        }

        /// <summary>
        /// Deletes the node.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void DeleteNode(object sender, ExecutedRoutedEventArgs args)
        {
            var targetNode = args.Parameter as ProjectNodeVisual;
            if (targetNode == null)
            {
                return;
            }

            var message = string.Format(
                CultureInfo.InvariantCulture,
                "Deletion cannot be undone and includes all child nodes.{0}{0}Confirm node '{1}' deletion?",
                Environment.NewLine,
                targetNode.Path);

            if (MessageBox.Show(message, "Confirm Delete Node", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.Cancel)
            {
                return;
            }

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            timer.Tick += (s, e) =>
                {
                    // Find all loaded workbench items that will be affected.
                    var projectData = this.visualiser.ProjectData;

                    var affectedItems = this.GetItemsUnderPath(targetNode.Path);

                    this.visualiser.DataProvider.DeleteProjectNode(projectData, targetNode.Path);

                    targetNode.Parent.Children.Remove(targetNode);
                    targetNode.Parent.SourceNode.Children.Remove(targetNode.SourceNode);
                    NodeLayoutHelper.ClearVisuals(this.visualiser.PART_LayoutCanvas, targetNode);

                    foreach (var affectedItem in affectedItems)
                    {
                        if (affectedItem.ValueProvider.IsNew)
                        {
                            projectData.WorkbenchItems.Remove(affectedItem);
                        }
                        else
                        {
                            affectedItem.ValueProvider.SyncToLatest();
                        }
                    }

                    CommandLibrary.ApplicationMessageCommand.Execute(string.Concat("Node '", targetNode.Path, "' deleted."), this.visualiser);
                    CommandLibrary.DisableUserInputCommand.Execute(false, this.visualiser);

                    ((DispatcherTimer)s).Stop();
                };

            CommandLibrary.ApplicationMessageCommand.Execute("Deleting node...", this.visualiser);
            CommandLibrary.DisableUserInputCommand.Execute(true, this.visualiser);

            timer.Start();
        }

        /// <summary>
        /// Renames the node.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void RenameNode(object sender, ExecutedRoutedEventArgs args)
        {
            var targetNode = args.Parameter as ProjectNodeVisual;
            if (targetNode == null)
            {
                return;
            }

            var originalPath = targetNode.Path;
            var originalName = targetNode.Name;

            var affectedItems = this.GetItemsUnderPath(originalPath);

            if (affectedItems != null && affectedItems.Any(wi => wi.ValueProvider.IsDirty || wi.ValueProvider.IsNew))
            {
                var message = string.Concat(
                    "Renaming this node will invalidate associated items with unsaved changes.",
                    Environment.NewLine,
                    Environment.NewLine,
                    "Continue and discard the unsaved changes?");

                if (MessageBox.Show(message, "Undo Changes", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            Action saveCallback = () =>
            {
                var projectData = this.visualiser.ProjectData;
                var dataProvider = this.visualiser.DataProvider;

                try
                {
                    dataProvider.RenameProjectNode(projectData, originalPath, targetNode.Name);
                }
                catch (Exception ex)
                {
                    if (CommandLibrary.ApplicationExceptionCommand.CanExecute(ex, this.visualiser))
                    {
                        CommandLibrary.ApplicationExceptionCommand.Execute(ex, this.visualiser);
                        return;
                    }

                    throw;
                }

                // Pause for classification node propergation.
                Thread.Sleep(2000);
                this.SyncAffectedWorkbenchItems(originalPath);

                NodeLayoutHelper.ClearVisuals(this.visualiser.PART_LayoutCanvas, targetNode);

                if (this.visualiser.NodeFilter.Equals(originalPath))
                {
                    this.visualiser.NodeFilter = targetNode.Path;
                }
                else
                {
                    this.VisualiseProjectLayout();
                }

                CommandLibrary.ApplicationMessageCommand.Execute("Node renamed", this.visualiser);
            };

            Action cancelCallback = () =>
            {
                targetNode.Name = originalName;
            };

            var editNodeControl = new EditNodeControl
            {
                ParentNode = targetNode.Parent,
                ChildNode = targetNode,
                SaveCallback = saveCallback,
                CancelCallback = cancelCallback
            };

            CommandLibrary.ShowDialogCommand.Execute(editNodeControl, this.visualiser);
        }

        /// <summary>
        /// Synchronises the workbench items that reference the old path.
        /// </summary>
        /// <param name="oldPath">The project node.</param>
        private void SyncAffectedWorkbenchItems(string oldPath)
        {
            var projectData = this.visualiser.ProjectData;
            if (projectData == null)
            {
                return;
            }

            var affectedItems = this.GetItemsUnderPath(oldPath);

            foreach (var affectedItem in affectedItems)
            {
                if (affectedItem.ValueProvider.IsNew)
                {
                    projectData.WorkbenchItems.Remove(affectedItem);
                }
                else
                {
                    affectedItem.ValueProvider.SyncToLatest();
                }
            }
        }

        /// <summary>
        /// Adds the child node.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void AddChildNode(object sender, ExecutedRoutedEventArgs e)
        {
            var parentNode = e.Parameter as ProjectNodeVisual;
            if (parentNode == null)
            {
                return;
            }

            var childNode = new ProjectNodeVisual(Factory.BuildProjectNode(parentNode.SourceNode, "New Node"), parentNode);

            parentNode.Children.Add(childNode);

            Action saveCallback = () =>
                {
                    var projectData = this.visualiser.ProjectData;
                    var dataProvider = this.visualiser.DataProvider;

                    dataProvider.SaveProjectNodes(projectData);

                    this.CreateAssociatedItem(projectData, childNode);

                    this.visualiser.ResetLayout();
                };

            Action cancelCallback = () =>
                {
                    parentNode.Children.Remove(childNode);
                    parentNode.SourceNode.Children.Remove(childNode.SourceNode);
                };

            var editNodeControl = new EditNodeControl
                {
                    ParentNode = parentNode,
                    ChildNode = childNode,
                    SaveCallback = saveCallback,
                    CancelCallback = cancelCallback
                };

            CommandLibrary.ShowDialogCommand.Execute(editNodeControl, this.visualiser);
        }

        /// <summary>
        /// Creates the associated item.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="nodeVisual">The node visual.</param>
        private void CreateAssociatedItem(IProjectData projectData, ProjectNodeVisual nodeVisual)
        {
            if (!SetupControllerHelper.IsValidScrumProject(projectData))
            {
                return;
            }

            var depth = nodeVisual.Depth;

            IWorkbenchItem newItem = null;

            if (Settings.Default.ReleaseDepth.Equals(depth))
            {
                newItem = SetupControllerHelper.CreateTopLevelWorkbenchItem(Settings.Default.ReleaseType);
            }

            if (Settings.Default.SprintDepth.Equals(depth))
            {
                SetupControllerHelper.TryCreateChildWorkbenchItem(nodeVisual, Settings.Default.ReleaseType, Settings.Default.SprintType, out newItem);
            }

            if (Settings.Default.TeamDepth.Equals(depth))
            {
                SetupControllerHelper.TryCreateChildWorkbenchItem(nodeVisual, Settings.Default.SprintType, Settings.Default.TeamType, out newItem);
            }

            if (newItem != null)
            {
                projectData.WorkbenchItems.Add(newItem);
                nodeVisual.WorkbenchItem = newItem;
                newItem[Core.Properties.Settings.Default.IterationPathFieldName] = nodeVisual.Path;
                CommandLibrary.EditItemCommand.Execute(newItem, this.visualiser);
            }
        }

        /// <summary>
        /// Filters the on node.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void FilterOnNode(object sender, ExecutedRoutedEventArgs e)
        {
            var node = e.Parameter as ProjectNodeVisual;
            if (node == null)
            {
                return;
            }

            this.visualiser.NodeFilter = node.Path;
        }
    }
}