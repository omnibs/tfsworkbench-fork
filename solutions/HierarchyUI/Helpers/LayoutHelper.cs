// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the LayoutHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.HierarchyUI.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    using Core.DataObjects;
    using Core.Helpers;
    using Core.Interfaces;
    using HierarchyObjects;

    /// <summary>
    /// The layout helper class.
    /// </summary>
    internal static class LayoutHelper
    {
        /// <summary>
        /// The connector element brush colour.
        /// </summary>
        private static SolidColorBrush connectorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD2D2D2"));

        /// <summary>
        /// Gets the connector brush.
        /// </summary>
        /// <value>The connector brush.</value>
        public static SolidColorBrush ConnectorBrush
        {
            get
            {
                return connectorBrush;
            }
        }

        /// <summary>
        /// Clears the canvas.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public static void ClearCanvas(Canvas canvas)
        {
            var hierarchyItemNodes = canvas.Children.OfType<HierarchyItemNode>().ToArray();

            var callback = (SendOrPostCallback)delegate
                {
                    foreach (var itemNode in hierarchyItemNodes)
                    {
                        itemNode.ReleaseResources(canvas);
                    }
                };

            canvas.Children.Clear();

            canvas.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, callback, null);
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="canvas">The canvas.</param>
        /// <param name="orientation">The orientation.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="selectedViews">The selected views.</param>
        /// <param name="isExcludingEmptyViews">if set to <c>true</c> [is excluding empty views].</param>
        /// <param name="selectedStates">The selected states.</param>
        public static void RenderView(
            IProjectData projectData, 
            Canvas canvas, 
            Orientation orientation, 
            Func<IWorkbenchItem, bool> filter, 
            IEnumerable<SelectedValue> selectedViews, 
            bool isExcludingEmptyViews, 
            IEnumerable<SelectedValue> selectedStates)
        {
            if (projectData == null)
            {
                throw new ArgumentNullException("projectData");
            }

            if (canvas == null)
            {
                throw new ArgumentNullException("canvas");
            }

            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            // Initialise item type view map
            var selectedViewMaps = projectData.ViewMaps
                .Where(vm => selectedViews.Any(sv => sv.IsSelected && sv.Text.Equals(vm.Title)));

            var allParents = selectedViewMaps.SelectMany(vm => vm.ParentTypes).Distinct();

            var itemViewMap = allParents.ToDictionary(
                p => p, p => selectedViewMaps.Where(vm => vm.ParentTypes.Contains(p)));

            var rootItems = projectData.WorkbenchItems.Where(filter).ToArray();

            var hierarchy = BuildHierarchy(projectData, rootItems, itemViewMap, null, isExcludingEmptyViews, selectedStates.Where(s => !s.IsSelected).Select(s => s.Text));

            RenderControls(canvas, hierarchy, orientation);

            canvas.UpdateLayout();

            ResizeCanvasToContent(canvas);
        }

        /// <summary>
        /// Sizes the canvas to it's content.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public static void ResizeCanvasToContent(Panel canvas)
        {
            var frameworkElements = canvas.Children.OfType<FrameworkElement>();

            var hasChildren = frameworkElements.Any();

            var maxWidth = hasChildren ? frameworkElements.Max(c => VisualTreeHelper.GetOffset(c).X + c.ActualWidth) : 0;
            var maxHeight = hasChildren ? frameworkElements.Max(c => VisualTreeHelper.GetOffset(c).Y + c.ActualHeight) : 0;

            canvas.Width = maxWidth < 0 ? 0 : maxWidth;
            canvas.Height = maxHeight < 0 ? 0 : maxHeight;
        }

        /// <summary>
        /// Moves the elements to top left.
        /// </summary>
        /// <param name="layoutCanvas">The layout canvas.</param>
        public static void MoveElementsToTopLeft(Panel layoutCanvas)
        {
            var frameworkElements = layoutCanvas.Children.OfType<FrameworkElement>();
            if (!frameworkElements.Any())
            {
                return;
            }

            var minTop = frameworkElements.OfType<ContentControl>().Min(c => VisualTreeHelper.GetOffset(c).Y);
            var minLeft = frameworkElements.OfType<ContentControl>().Min(c => VisualTreeHelper.GetOffset(c).X);

            // Move the rendered elements to the top left of the canvas
            foreach (var element in frameworkElements)
            {
                var elementAsLine = element as Line;

                if (elementAsLine != null)
                {
                    elementAsLine.Y1 -= minTop;
                    elementAsLine.Y2 -= minTop;
                    elementAsLine.X1 -= minLeft;
                    elementAsLine.X2 -= minLeft;
                    continue;
                }

                element.SetValue(Canvas.TopProperty, (double)element.GetValue(Canvas.TopProperty) - minTop);
                element.SetValue(Canvas.LeftProperty, (double)element.GetValue(Canvas.LeftProperty) - minLeft);

                HierarchyElementBase hierarchyElement;
                if (!TryGetHierarchyElement(element, out hierarchyElement))
                {
                    continue;
                }

                var newEntryPoint = new Point(
                    hierarchyElement.EntryPoint.X - minLeft,
                    hierarchyElement.EntryPoint.Y - minTop);

                var newExitPoint = new Point(
                    hierarchyElement.ExitPoint.X - minLeft,
                    hierarchyElement.ExitPoint.Y - minTop);

                hierarchyElement.EntryPoint = newEntryPoint;
                hierarchyElement.ExitPoint = newExitPoint;
            }
        }

        /// <summary>
        /// Removes the associated visual.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        public static void RemoveAssociatedVisual(Canvas canvas, IWorkbenchItem workbenchItem)
        {
            IEnumerable<HierarchyItemNode> existingVisuals;
            if (!TryGetAssociatedVisuals(canvas, workbenchItem, out existingVisuals))
            {
                return;
            }

            foreach (var hierarchyItem in existingVisuals.Select(hin => hin.HierarchyItem))
            {
                hierarchyItem.ReleaseResources(canvas);

                var parentView = hierarchyItem.Parent as HierarchyView;

                if (parentView == null)
                {
                    continue;
                }

                parentView.HierarchyItems.Remove(hierarchyItem);
            }
        }

        /// <summary>
        /// Tries to get the associated visuals.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="visuals">The visuals.</param>
        /// <returns><c>True</c> if associated visuals are found; otherwise <c>false</c>.</returns>
        public static bool TryGetAssociatedVisuals(Canvas canvas, IWorkbenchItem workbenchItem, out IEnumerable<HierarchyItemNode> visuals)
        {
            visuals =
                canvas.Children.OfType<HierarchyItemNode>().Where(
                    hin => Equals(hin.HierarchyItem.WorkbenchItem, workbenchItem)).ToArray();

            return visuals.Any();
        }

        /// <summary>
        /// Tries the get hierarchy element.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <param name="hierarchyElement">The hierarchy element.</param>
        /// <returns><c>True</c> if the specified framework element is a hierarchy node; otherwise <c>flase</c>.</returns>
        private static bool TryGetHierarchyElement(FrameworkElement frameworkElement, out HierarchyElementBase hierarchyElement)
        {
            var elementAsItemNode = frameworkElement as HierarchyItemNode;

            if (elementAsItemNode != null)
            {
                hierarchyElement = elementAsItemNode.HierarchyItem;
            }
            else
            {
                var elementAsViewNode = frameworkElement as HierarchyViewNode;

                hierarchyElement = elementAsViewNode != null ? elementAsViewNode.HierarchyView : null;
            }

            return hierarchyElement != null;
        }

        /// <summary>
        /// Renders the controls.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="hierarchy">The hierarchy.</param>
        /// <param name="orientation">The orientation.</param>
        private static void RenderControls(Canvas canvas, IEnumerable<HierarchyItem> hierarchy, Orientation orientation)
        {
            var offset = new Point();

            hierarchy.Aggregate(offset, (current, hierarchyItem) => hierarchyItem.Render(canvas, current, orientation));
        }

        /// <summary>
        /// Builds the hierarchy.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="workbenchItems">The workbench items.</param>
        /// <param name="itemViewMaps">The item view maps.</param>
        /// <param name="parentView">The parent view.</param>
        /// <param name="isExcludingEmptyViews">if set to <c>true</c> [is excluding empty views].</param>
        /// <param name="excludedChildStates">The excluded child states.</param>
        /// <returns>
        /// An enumerable list of the hierarchy items.
        /// </returns>
        private static IEnumerable<HierarchyItem> BuildHierarchy(
            IProjectData projectData, 
            IEnumerable<IWorkbenchItem> workbenchItems, 
            IDictionary<string, IEnumerable<ViewMap>> itemViewMaps, 
            HierarchyView parentView, 
            bool isExcludingEmptyViews,
            IEnumerable<string> excludedChildStates)
        {
            var output = new List<HierarchyItem>();

            foreach (var workbenchItem in workbenchItems)
            {
                var hierarchyItem = new HierarchyItem(parentView) { ProjectData = projectData, WorkbenchItem = workbenchItem };

                output.Add(hierarchyItem);

                // If already rendered as a parent, skip child views.
                if (IsParentInTree(parentView, workbenchItem))
                {
                    continue;
                }

                var itemType = workbenchItem.GetTypeName();

                IEnumerable<ViewMap> viewMaps;
                if (!itemViewMaps.TryGetValue(itemType, out viewMaps))
                {
                    continue;
                }

                foreach (var viewMap in viewMaps)
                {
                    var childItems = GetChildItems(workbenchItem, viewMap, excludedChildStates);

                    if (isExcludingEmptyViews && !childItems.Any())
                    {
                        continue;
                    }

                    var hierarchyView = new HierarchyView(hierarchyItem) { ViewMap = viewMap };

                    foreach (var item in BuildHierarchy(projectData, childItems, itemViewMaps, hierarchyView, isExcludingEmptyViews, excludedChildStates))
                    {
                        hierarchyView.HierarchyItems.Add(item);
                    }

                    hierarchyItem.HierarchyViews.Add(hierarchyView);
                }
            }

            return output;
        }

        /// <summary>
        /// Determines whether [the specified parent view] [is parent in tree].
        /// </summary>
        /// <param name="parentElement">The parent view.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>
        /// <c>true</c> if [the specified parent view] [is parent in tree]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsParentInTree(HierarchyElementBase parentElement, IWorkbenchItem workbenchItem)
        {
            if (parentElement == null)
            {
                return false;
            }

            var parent = parentElement.Parent;
            while (parent != null)
            {
                var parentItem = parent as HierarchyItem;

                if (parentItem != null && Equals(parentItem.WorkbenchItem, workbenchItem))
                {
                    return true;
                }

                parent = parent.Parent;
            }

            return false;
        }

        /// <summary>
        /// Gets the child items that match the specifed viewmap.
        /// </summary>
        /// <param name="parentItem">The parent item.</param>
        /// <param name="viewMap">The view map.</param>
        /// <param name="excludedStates">The excluded states.</param>
        /// <returns>
        /// An enumerable of child items that match the specifed viewmap.
        /// </returns>
        private static IEnumerable<IWorkbenchItem> GetChildItems(IWorkbenchItem parentItem, ViewMap viewMap, IEnumerable<string> excludedStates = null)
        {
            excludedStates = excludedStates ?? new string[] { };

            var links =
                parentItem.ChildLinks.Where(
                    l =>
                    Equals(l.LinkName, viewMap.LinkName) &&
                    Equals(l.Child.GetTypeName(), viewMap.ChildType) &&
                    !excludedStates.Contains(l.Child.GetState()));

            var workbenchItems = links.Select(l => l.Child).ToList();

            workbenchItems.Sort(viewMap.ChildSorter);

            return workbenchItems;
        }
    }
}
