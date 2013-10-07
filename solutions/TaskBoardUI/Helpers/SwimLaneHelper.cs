// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwimLaneHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the StateChangeHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.TaskBoardUI.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.TaskBoardUI.DataObjects;
    using TfsWorkbench.TaskBoardUI.Properties;

    /// <summary>
    /// Initializes instance of StateChangeHelper
    /// </summary>
    internal static class SwimLaneHelper
    {
        /// <summary>
        /// Synchronises the state containers.
        /// </summary>
        /// <param name="candidate">The task board item.</param>
        /// <param name="oldState">The old state.</param>
        /// <param name="newState">The new state.</param>
        public static void SynchroniseStateContainers(IWorkbenchItem candidate, string oldState, string newState)
        {
            IEnumerable<SwimLaneView> views = SwimLaneService.Instance.SwimLaneViews;

            foreach (var view in views.Where(v => v.ViewMap.ChildType.Equals(candidate.GetTypeName())))
            {
                var linkName = view.ViewMap.LinkName;

                Func<IWorkbenchItem, bool> isViewParent =
                    w => w.ChildLinks.Any(l => Equals(l.Child, candidate) && Equals(l.LinkName, linkName));

                var parents = view.SwimLaneRows.Select(r => r.Parent).Where(isViewParent).ToArray();

                if (!parents.Any())
                {
                    // The candidate is either an orphan, a parent or does not exist in this view.
                    continue;
                }

                foreach (var parent in parents)
                {
                    StateCollection stateCollection;
                    bool isBucketState;

                    // Check the old state container and remove if required
                    if (TryGetStateCollection(view, parent, oldState, out stateCollection, out isBucketState))
                    {
                        if (stateCollection.Contains(candidate))
                        {
                            if (isBucketState)
                            {
                                stateCollection.Parent = parent;
                            }

                            stateCollection.Remove(candidate);

                            if (isBucketState)
                            {
                                stateCollection.Parent = null;
                            }
                        }
                    }

                    // Check the new state container and add if required
                    if (TryGetStateCollection(view, parent, newState, out stateCollection, out isBucketState))
                    {
                        if (!stateCollection.Contains(candidate))
                        {
                            if (isBucketState)
                            {
                                stateCollection.Parent = parent;
                            }

                            stateCollection.Add(candidate);

                            if (isBucketState)
                            {
                                stateCollection.Parent = null;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Synchronises the state containers.
        /// </summary>
        /// <param name="removedLink">The old link.</param>
        /// <param name="addedLink">The new link.</param>
        public static void SynchroniseLinkChanges(ILinkItem removedLink, ILinkItem addedLink)
        {
            IEnumerable<SwimLaneView> views = SwimLaneService.Instance.SwimLaneViews;

            Func<SwimLaneView, ILinkItem, bool> isAffectedView =
                (view, link) => view.ViewMap.IsViewLink(link);

            // Clear the removed link.
            foreach (var view in views.Where(v => isAffectedView(v, removedLink)))
            {
                var child = removedLink.Child;
                var containers = view.BucketStates.Where(c => c.Contains(child)).ToList();

                containers.AddRange(
                    view.SwimLaneRows.SelectMany(r => r.SwimLaneColumns).Where(c => c.Contains(child)));

                var wasFound = containers.Any();

                foreach (var container in containers)
                {
                    container.Remove(child);
                }

                if (wasFound && !view.Orphans.Contains(child))
                {
                    view.Orphans.Add(child);
                }
            }

            // Add any missing references.
            foreach (var view in views.Where(v => isAffectedView(v, addedLink)))
            {
                var child = addedLink.Child;

                if (view.Orphans.Contains(child))
                {
                    view.Orphans.Remove(child);
                }

                view.AddChild(child);
            }
        }

        /// <summary>
        /// Adds the item to views.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        public static void AddItemToViews(IWorkbenchItem workbenchItem)
        {
            foreach (var swimLaneView in SwimLaneService.Instance.SwimLaneViews)
            {
                var isParent = workbenchItem.IsParentType(swimLaneView);
                var isChild = workbenchItem.IsChildType(swimLaneView);

                SwimLaneRow existingRow;
                if (isParent && !TryFindRow(swimLaneView, workbenchItem, out existingRow))
                {
                    swimLaneView.AddSwimLaneRow(workbenchItem);

                    var view = swimLaneView;
                    foreach (var child in workbenchItem.ChildLinks.Where(l => l.Child.IsChildType(view)).Select(l => l.Child))
                    {
                        swimLaneView.AddChild(child);
                    }
                }

                if (isChild)
                {
                    swimLaneView.AddChild(workbenchItem);
                }
            }
        }

        /// <summary>
        /// Removes the item from views.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        public static void RemoveItemFromViews(IWorkbenchItem workbenchItem)
        {
            foreach (var swimLaneView in SwimLaneService.Instance.SwimLaneViews)
            {
                var isParent = workbenchItem.IsParentType(swimLaneView);
                var isChild = workbenchItem.IsChildType(swimLaneView);
                if (isParent)
                {
                    swimLaneView.RemoveSwimLaneRow(workbenchItem);
                }

                if (isChild)
                {
                    swimLaneView.RemoveChild(workbenchItem);
                }
            }
        }

        /// <summary>
        /// Syncronises the view.
        /// </summary>
        /// <param name="swimLaneView">The swim lane view.</param>
        /// <param name="workbenchItems">The workbench items.</param>
        public static void SyncroniseViewItems(SwimLaneView swimLaneView, IEnumerable<IWorkbenchItem> workbenchItems)
        {
            // Remove any rows missing from the specified collection
            foreach (var parent in swimLaneView.SwimLaneRows.Select(r => r.Parent).Where(p => !workbenchItems.Contains(p)).ToArray())
            {
                swimLaneView.RemoveSwimLaneRow(parent, true);
            }

            // Remove any orphans not included in the specfied colleciton
            foreach (var orphan in swimLaneView.Orphans.Where(o => !workbenchItems.Contains(o)).ToArray())
            {
                swimLaneView.RemoveChild(orphan);
            }

            // Remove any row children not included in the specified item collection
            foreach (var child in swimLaneView.SwimLaneRows.SelectMany(r => r.SwimLaneColumns.SelectMany(c => c)).Where(c => !workbenchItems.Contains(c)).ToArray())
            {
                swimLaneView.RemoveChild(child);
            }

            // Remove any bucket items not included in the specfied item collection
            foreach (var bucketChild in swimLaneView.BucketStates.SelectMany(bs => bs).Where(w => !workbenchItems.Contains(w)).ToArray())
            {
                swimLaneView.RemoveChild(bucketChild);
            }

            SwimLaneRow row;

            // Add any missing row items.
            foreach (var workbenchItem in
                workbenchItems.Where(w => w.IsParentType(swimLaneView) && !TryFindRow(swimLaneView, w, out row)).ToArray())
            {
                // Add row
                swimLaneView.AddSwimLaneRow(workbenchItem);
            }

            // Add child items.
            foreach (var workbenchItem in workbenchItems.Where(w => w.IsChildType(swimLaneView)))
            {
                swimLaneView.AddChild(workbenchItem);
            }

            // Remove any mis-placed orphans
            foreach (var workbenchItem in workbenchItems.Where(wbi => wbi.IsOrphan(swimLaneView)))
            {
                var item = workbenchItem;
                foreach (var column in 
                    swimLaneView.SwimLaneRows
                        .SelectMany(slr => slr.SwimLaneColumns)
                        .Where(slc => slc.Contains(item)))
                {
                    column.Remove(workbenchItem);
                }
            }

            // Finally, sort the rows
            swimLaneView.ApplySort();
        }

        /// <summary>
        /// Orphans the items.
        /// </summary>
        /// <param name="swimLaneView">The swim lane view.</param>
        /// <param name="itemsToOrphan">The orphaned items.</param>
        public static void OrphanItems(SwimLaneView swimLaneView, IEnumerable<IWorkbenchItem> itemsToOrphan)
        {
            Func<ILinkItem, bool> isParentLink = link => swimLaneView.ViewMap.IsViewLink(link);

            foreach (var parentLink in itemsToOrphan.SelectMany(o => o.ParentLinks).Where(isParentLink).ToArray())
            {
                ServiceManager.Instance.GetService<ILinkManagerService>().RemoveLink(parentLink);
            }
        }

        public static string GetItemStatus(IWorkbenchItem workbenchItem)
        {
            var body = workbenchItem.GetBody().Trim().ToLowerInvariant();
            if (body.EndsWith("]"))
            {
                foreach (var customState in WorkbenchItemHelper.CustomStates)
                {
                    if (body.EndsWith("[" + customState.ToLowerInvariant() + "]"))
                    {
                        return customState;
                    }
                }
            }

            // else
            return workbenchItem.GetState();
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="swimLaneView">The swim lane view.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        private static void AddChild(this SwimLaneView swimLaneView, IWorkbenchItem workbenchItem)
        {
            var itemState = workbenchItem.GetState();

            if (workbenchItem.IsOrphan(swimLaneView))
            {
                // Add to orphans
                if (!swimLaneView.Orphans.Contains(workbenchItem))
                {
                    swimLaneView.Orphans.Add(workbenchItem);
                }

                return;
            }

            var filterService = ServiceManager.Instance.GetService<IFilterService>();

            var hasIncludedParent =
                workbenchItem.ParentLinks
                    .Where(l => swimLaneView.ViewMap.IsViewLink(l) && !filterService.IsExcluded(l.Parent))
                    .Any();

            if (!hasIncludedParent)
            {
                return;
            }

            if (workbenchItem.IsBucketState(swimLaneView))
            {
                // Add to bucket container
                var targetBucket = swimLaneView.BucketStates.FirstOrDefault(b => Equals(b.State, itemState));
                if (targetBucket != null && !targetBucket.Contains(workbenchItem))
                {
                    targetBucket.Add(workbenchItem);
                }

                return;
            }

            if (!workbenchItem.IsSwimLaneState(swimLaneView))
            {
                // Item not rendered in view
                return;
            }

            itemState = GetItemStatus(workbenchItem);

            // Add to swim lane row(s).
            foreach (var stateCollection in FindParentRows(swimLaneView, workbenchItem))
            {
                var stateContainer = stateCollection[itemState];
                if (!stateContainer.Contains(workbenchItem))
                {
                    stateContainer.Add(workbenchItem);
                }
            }
        }

        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="swimLaneView">The swim lane view.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        private static void RemoveChild(this SwimLaneView swimLaneView, IWorkbenchItem workbenchItem)
        {
            // Remove from orphans
            if (swimLaneView.Orphans.Contains(workbenchItem))
            {
                swimLaneView.Orphans.Remove(workbenchItem);
            }

            // Find all collections that contain the item.
            var collections =
                swimLaneView.BucketStates
                .Union(swimLaneView.SwimLaneRows.SelectMany(slr => slr.SwimLaneColumns))
                .Where(sc => sc.Contains(workbenchItem))
                .ToArray();

            foreach (var stateCollection in collections)
            {
                stateCollection.Remove(workbenchItem);
            }
        }

        /// <summary>
        /// Adds the swim lane row.
        /// </summary>
        /// <param name="swimLaneView">The swim lane view.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        private static void AddSwimLaneRow(this SwimLaneView swimLaneView, IWorkbenchItem workbenchItem)
        {
            var row = new SwimLaneRow(
                workbenchItem,
                swimLaneView.ViewMap.SwimLaneStates,
                swimLaneView.ViewMap.LinkName,
                swimLaneView.ViewMap.ChildType,
                Settings.Default.SwimLaneRowHeight);

            swimLaneView.SwimLaneRows.Add(row);
        }

        /// <summary>
        /// Removes the swim lane row.
        /// </summary>
        /// <param name="swimLaneView">The swim lane view.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="andChildren">if set to <c>true</c> [and children].</param>
        private static void RemoveSwimLaneRow(this SwimLaneView swimLaneView, IWorkbenchItem workbenchItem, bool andChildren = false)
        {
            var rowToRemove = swimLaneView.SwimLaneRows.FirstOrDefault(r => Equals(r.Parent, workbenchItem));

            if (rowToRemove == null)
            {
                return;
            }

            swimLaneView.SwimLaneRows.Remove(rowToRemove);

            var viewChildren = workbenchItem.ChildLinks
                .Where(linkItem => 
                    Equals(linkItem.LinkName, swimLaneView.ViewMap.LinkName) 
                    && linkItem.Child.IsChildType(swimLaneView))
                    .Select(l => l.Child)
                    .ToArray();

            foreach (var viewChild in viewChildren)
            {
                if (andChildren)
                {
                    swimLaneView.RemoveChild(viewChild);
                }
                else
                {
                    swimLaneView.Orphans.Add(viewChild);
                }
            }

            rowToRemove.ReleaseResources();
        }

        /// <summary>
        /// Determines whether the candidate [is parent type] in [the specified swim lane view].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="swimLaneView">The swim lane view.</param>
        /// <returns>
        /// <c>true</c> if the candidate [is parent type] in [the specified swim lane view]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsParentType(this IWorkbenchItem candidate, SwimLaneView swimLaneView)
        {
            return swimLaneView.ViewMap.ParentTypes.Contains(candidate.GetTypeName());
        }

        /// <summary>
        /// Determines whether the candidate [is child type] in [the specified swim lane view].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="swimLaneView">The swim lane view.</param>
        /// <returns>
        /// <c>true</c> if the candidate [is child type] in [the specified swim lane view]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsChildType(this IWorkbenchItem candidate, SwimLaneView swimLaneView)
        {
            return Equals(swimLaneView.ViewMap.ChildType, candidate.GetTypeName());
        }

        /// <summary>
        /// Determines whether the candidate [is bucket state] in [the specified swim lane view].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="swimLaneView">The swim lane view.</param>
        /// <returns>
        /// <c>true</c> if the candidate [is bucket state] in [the specified swim lane view]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsBucketState(this IWorkbenchItem candidate, SwimLaneView swimLaneView)
        {
            var state = candidate.GetState();

            return swimLaneView.ViewMap.BucketStates.Any(bs => Equals(bs, state));
        }

        /// <summary>
        /// Determines whether the candidate [is orphan] in [the specified swim lane view].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="swimLaneView">The swim lane view.</param>
        /// <returns>
        /// <c>true</c> if the candidate [is orphan] in [the specified swim lane view]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsOrphan(this IWorkbenchItem candidate, SwimLaneView swimLaneView)
        {
            Func<IWorkbenchItem, bool> isParentType =
                w => swimLaneView.ViewMap.ParentTypes.Contains(w.GetTypeName());

            var viewParents =
                candidate.ParentLinks.Where(
                    l => 
                        isParentType(l.Parent) 
                        && Equals(l.LinkName, swimLaneView.ViewMap.LinkName));

            return candidate.IsChildType(swimLaneView) && !viewParents.Any();
        }

        /// <summary>
        /// Determines whether the candidate [is swim lane state] in [the specified swim lane view].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="swimLaneView">The swim lane view.</param>
        /// <returns>
        /// <c>true</c> if the candidate [is swim lane state] in [the specified swim lane view]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsSwimLaneState(this IWorkbenchItem candidate, SwimLaneView swimLaneView)
        {
            var state = candidate.GetState();

            return swimLaneView.ViewMap.SwimLaneStates.Any(sls => Equals(sls, state));
        }

        /// <summary>
        /// Tries to find the row.
        /// </summary>
        /// <param name="swimLaneView">The swim lane view.</param>
        /// <param name="rowParent">The row parent.</param>
        /// <param name="swimLaneRow">The swim lane row.</param>
        /// <returns><c>True</c> if row found; otherwuse <c>false</c>.</returns>
        private static bool TryFindRow(this SwimLaneView swimLaneView, IWorkbenchItem rowParent, out SwimLaneRow swimLaneRow)
        {
            swimLaneRow = swimLaneView.SwimLaneRows.FirstOrDefault(r => Equals(r.Parent, rowParent));

            return swimLaneRow != null;
        }

        /// <summary>
        /// Finds the parent rows.
        /// </summary>
        /// <param name="swimLaneView">The swim lane view.</param>
        /// <param name="rowChild">The row child.</param>
        /// <returns>An enumerable list of the existing row parnets.</returns>
        private static IEnumerable<SwimLaneRow> FindParentRows(SwimLaneView swimLaneView, IWorkbenchItem rowChild)
        {
            var viewParents = rowChild.ParentLinks.Where(swimLaneView.ViewMap.IsViewLink).Select(l => l.Parent).ToArray();

            var rows =
                swimLaneView.SwimLaneRows
                    .Where(r => viewParents.Contains(r.Parent));

            var output = new Collection<SwimLaneRow>(rows.ToList());

            return output;
        }

        /// <summary>
        /// Tries the get state collection.
        /// </summary>
        /// <param name="view">The view object.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="state">The state.</param>
        /// <param name="stateCollection">The state collection.</param>
        /// <param name="isBucketState">if set to <c>true</c> [is bucket state].</param>
        /// <returns><c>True</c> if a state collection id matched; otherwise <c>false</c></returns>
        private static bool TryGetStateCollection(SwimLaneView view, IWorkbenchItem parent, string state, out StateCollection stateCollection, out bool isBucketState)
        {
            stateCollection = null;
            isBucketState = false;

            if (view.ViewMap.BucketStates.Contains(state))
            {
                isBucketState = true;
                stateCollection = view.BucketStates.FirstOrDefault(b => b.State.Equals(state));
            }

            if (!isBucketState && view.ViewMap.SwimLaneStates.Contains(state))
            {
                SwimLaneRow row;
                if (TryFindRow(view, parent, out row))
                {
                    stateCollection = row[state];
                }
            }

            return stateCollection != null;
        }
    }
}
