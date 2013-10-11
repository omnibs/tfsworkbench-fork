// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwimLaneService.cs" company="None">
//   None
// </copyright>
// <summary>
//   The swim lane service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.TaskBoardUI.Helpers
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading;
    using System.Windows.Threading;

    using TfsWorkbench.Core.DataObjects;
    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.TaskBoardUI.DataObjects;

    /// <summary>
    /// The swim lane service class.
    /// </summary>
    public class SwimLaneService : ISwimLaneService
    {
        /// <summary>
        /// The swim lane view collection.
        /// </summary>
        private readonly ObservableCollection<SwimLaneView> swimLaneViews = new ObservableCollection<SwimLaneView>();

        /// <summary>
        /// The swim lane service instance.
        /// </summary>
        private static SwimLaneService instance;

        /// <summary>
        /// The project data instance.
        /// </summary>
        private IProjectData projectData;

        /// <summary>
        /// Prevents a default instance of the <see cref="SwimLaneService"/> class from being created.
        /// </summary>
        private SwimLaneService()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ISwimLaneService Instance
        {
            get
            {
                return instance = instance ?? new SwimLaneService();
            }
        }

        /// <summary>
        /// Gets the swim lanes views.
        /// </summary>
        /// <value>The swim lanes views.</value>
        public ObservableCollection<SwimLaneView> SwimLaneViews
        {
            get
            {
                return this.swimLaneViews;
            }
        }

        /// <summary>
        /// Generates the views.
        /// </summary>
        /// <param name="projectDataCandidate">The project data.</param>
        public void Initialise(IProjectData projectDataCandidate)
        {
            this.ReleaseResources();

            this.projectData = projectDataCandidate;

            if (this.projectData == null)
            {
                return;
            }

            this.SetEventSubscription(true);

            foreach (var swimLaneView in projectDataCandidate.ViewMaps.Where(vm => !vm.IsNotSwimLane).Select(this.GenerateSwimLaneView))
            {
                this.SwimLaneViews.Add(swimLaneView);
                swimLaneView.ItemsOrphaned += OnItemsOrphaned;
            }
        }

        /// <summary>
        /// Releases the resources.
        /// </summary>
        public void ReleaseResources()
        {
            this.SetEventSubscription(false);

            foreach (var swimLanesView in this.SwimLaneViews)
            {
                swimLanesView.ReleaseResources();
                swimLanesView.ItemsOrphaned -= OnItemsOrphaned;
            }

            this.SwimLaneViews.Clear();
        }

        /// <summary>
        /// Called when [item state changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.ItemStateChangeEventArgs"/> instance containing the event data.</param>
        private static void OnItemStateChanged(object sender, ItemStateChangeEventArgs e)
        {
            SwimLaneHelper.SynchroniseStateContainers(e.WorkbenchItem, e.OldValue, e.NewValue);
        }

        /// <summary>
        /// Called when [link changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.ItemLinkChangeEventArgs"/> instance containing the event data.</param>
        private static void OnLinkChanged(object sender, ItemLinkChangeEventArgs e)
        {
            SwimLaneHelper.SynchroniseLinkChanges(e.OldValue, e.NewValue);
        }

        /// <summary>
        /// Called when [items orphaned].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.TaskBoardUI.DataObjects.ItemsOrphanedEventArgs"/> instance containing the event data.</param>
        private static void OnItemsOrphaned(object sender, ItemsOrphanedEventArgs e)
        {
            SwimLaneHelper.OrphanItems(e.SwimLaneView, e.OrphanedItems);
        }

        /// <summary>
        /// Called when [collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TfsWorkbench.Core.EventArgObjects.RepositoryChangedEventArgs&lt;IWorkbenchItem&gt;"/> instance containing the event data.</param>
        private void OnCollectionChanged(object sender, RepositoryChangedEventArgs<IWorkbenchItem> e)
        {
            Action execute = () =>
                {
                    switch (e.Action)
                    {
                        case ChangeActionOption.Add:
                            foreach (var item in e.Context)
                            {
                                SwimLaneHelper.AddItemToViews(item);
                            }

                            break;
                        case ChangeActionOption.Remove:
                            foreach (var item in e.Context)
                            {
                                SwimLaneHelper.RemoveItemFromViews(item);
                            }

                            break;
                        case ChangeActionOption.Clear:
                            foreach (var item in e.Context)
                            {
                                SwimLaneHelper.RemoveItemFromViews(item);
                            }

                            break;
                        case ChangeActionOption.Refresh:
                            var workbenchItems = this.projectData.WorkbenchItems.ToArray();
                            foreach (var swimLaneView in this.SwimLaneViews)
                            {
                                SwimLaneHelper.SyncroniseViewItems(swimLaneView, workbenchItems);
                            }

                            break;
                        default:
                            return;
                    }
                };

            if (this.projectData.Dispatcher == null || Thread.CurrentThread.Equals(this.projectData.Dispatcher.Thread))
            {
                execute();
            }
            else
            {
                this.projectData.Dispatcher.BeginInvoke(DispatcherPriority.Normal, execute);
            }
        }

        /// <summary>
        /// Sets the event subscription.
        /// </summary>
        /// <param name="isSubscribed">if set to <c>true</c> [is subscribed].</param>
        private void SetEventSubscription(bool isSubscribed)
        {
            if (this.projectData == null)
            {
                return;
            }

            if (isSubscribed)
            {
                foreach (var viewMap in this.projectData.ViewMaps)
                {
                    viewMap.LayoutUpdated += this.OnViewMapLayoutUpdated;
                }

                this.projectData.ViewMaps.CollectionChanged += this.OnViewMapCollectionChanged;
                this.projectData.WorkbenchItems.ItemStateChanged += OnItemStateChanged;
                this.projectData.WorkbenchItems.LinkChanged += OnLinkChanged;
                this.projectData.WorkbenchItems.CollectionChanged += this.OnCollectionChanged;
            }
            else
            {
                foreach (var viewMap in this.projectData.ViewMaps)
                {
                    viewMap.LayoutUpdated -= this.OnViewMapLayoutUpdated;
                }

                this.projectData.ViewMaps.CollectionChanged -= this.OnViewMapCollectionChanged;
                this.projectData.WorkbenchItems.ItemStateChanged -= OnItemStateChanged;
                this.projectData.WorkbenchItems.LinkChanged -= OnLinkChanged;
                this.projectData.WorkbenchItems.CollectionChanged -= this.OnCollectionChanged;
            }
        }

        /// <summary>
        /// Generates the swim lane view.
        /// </summary>
        /// <param name="viewMap">The view map.</param>
        /// <returns>A new swim lane view instance.</returns>
        private SwimLaneView GenerateSwimLaneView(ViewMap viewMap)
        {
            if (this.projectData == null)
            {
                return null;
            }

            var swimLaneView = new SwimLaneView(viewMap);

            SwimLaneHelper.SyncroniseViewItems(swimLaneView, this.projectData.WorkbenchItems.ToArray());

            return swimLaneView;
        }

        /// <summary>
        /// Called when [view map layout updated].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ViewMapEventArgs"/> instance containing the event data.</param>
        private void OnViewMapLayoutUpdated(object sender, ViewMapEventArgs e)
        {
            var isNotNotSwimLane = e.Context.IsNotSwimLane;

            var swimLaneView = this.SwimLaneViews.FirstOrDefault(slv => Equals(slv.ViewMap, e.Context));

            if (swimLaneView == null && isNotNotSwimLane)
            {
                return;
            }

            if (swimLaneView != null && isNotNotSwimLane)
            {
                this.SwimLaneViews.Remove(swimLaneView);
                swimLaneView.ReleaseResources();
                return;
            }

            if (swimLaneView == null)
            {
                swimLaneView = this.GenerateSwimLaneView(e.Context);
                this.SwimLaneViews.Add(swimLaneView);
            }

            swimLaneView.InitialiseLayout(e.Context);
            SwimLaneHelper.SyncroniseViewItems(swimLaneView, this.projectData.WorkbenchItems.ToArray());
        }

        /// <summary>
        /// Called when [view map collection changed].
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event args. </param>
        private void OnViewMapCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var viewMap in e.NewItems.OfType<ViewMap>().Where(vm => !vm.IsNotSwimLane))
                    {
                        viewMap.LayoutUpdated += this.OnViewMapLayoutUpdated;
                        var swimLaneView = this.GenerateSwimLaneView(viewMap);
                        swimLaneView.ItemsOrphaned += OnItemsOrphaned;
                        this.SwimLaneViews.Add(swimLaneView);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    var viewsToRemove =
                        this.SwimLaneViews.Where(slv => e.OldItems.OfType<ViewMap>().Any(vm => Equals(slv.ViewMap, vm))).
                            ToArray();

                    foreach (var swimLaneView in viewsToRemove)
                    {
                        swimLaneView.ItemsOrphaned -= OnItemsOrphaned;
                        swimLaneView.ViewMap.LayoutUpdated -= this.OnViewMapLayoutUpdated;
                        this.SwimLaneViews.Remove(swimLaneView);
                        swimLaneView.ReleaseResources();
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    foreach (var swimLaneView in this.SwimLaneViews)
                    {
                        swimLaneView.ItemsOrphaned -= OnItemsOrphaned;
                        swimLaneView.ViewMap.LayoutUpdated -= this.OnViewMapLayoutUpdated;
                        swimLaneView.ReleaseResources();
                    }

                    this.SwimLaneViews.Clear();
                    break;

                default:
                    return;
            }
        }
    }
}