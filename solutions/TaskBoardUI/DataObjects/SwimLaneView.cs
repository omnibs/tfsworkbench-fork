// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwimLaneView.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the SwimLaneView type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.TaskBoardUI.DataObjects
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    using Core.DataObjects;
    using Core.Helpers;
    using Core.Interfaces;

    using TfsWorkbench.TaskBoardUI.Helpers;

    /// <summary>
    /// Initialises and instance of TfsWorkbench.Core.DataObjects.SwimLaneView
    /// </summary>
    public class SwimLaneView 
    {
        /// <summary>
        /// The bucket states.
        /// </summary>
        private readonly ObservableCollection<StateCollection> bucketStates =
            new ObservableCollection<StateCollection>();

        /// <summary>
        /// The orphans.
        /// </summary>
        private readonly SortableObservableCollection<IWorkbenchItem> orphans =
            new SortableObservableCollection<IWorkbenchItem>();

        /// <summary>
        /// The swim lane rows.
        /// </summary>
        private readonly SortableObservableCollection<SwimLaneRow> swimLaneRows =
            new SortableObservableCollection<SwimLaneRow>();

        /// <summary>
        /// The internal row headers colleciton.
        /// </summary>
        private readonly ObservableCollection<string> rowHeaders = 
            new ObservableCollection<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SwimLaneView"/> class.
        /// </summary>
        /// <param name="viewMap">The view map.</param>
        public SwimLaneView(ViewMap viewMap)
            : this(viewMap, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwimLaneView"/> class.
        /// </summary>
        /// <param name="viewMap">The view map.</param>
        /// <param name="includeAsTab">if set to <c>true</c> [include as tab].</param>
        public SwimLaneView(ViewMap viewMap, bool includeAsTab)
        {
            var customStates = WorkbenchItemHelper.CustomStates;
            if (customStates.Length > 0 && !viewMap.SwimLaneStates.Contains(customStates[0].Name))
            {
                foreach (var customState in customStates)
                {
                    if (!customState.IsBucketState)
                    {
                        viewMap.SwimLaneStates.Insert(viewMap.SwimLaneStates.Count - 1, customState.Name);
                    }
                    else
                    {
                        viewMap.SwimLaneStates.Add(customState.Name);
                    }
                }
            }

            this.IncludeInTabs = includeAsTab;
            this.InitialiseLayout(viewMap);
            this.Orphans.CollectionChanged += this.OnOrphansChanged;
        }

        /// <summary>
        /// Occurs when [layout changed].
        /// </summary>
        public event EventHandler LayoutChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when [items orphaned].
        /// </summary>
        public event EventHandler<ItemsOrphanedEventArgs> ItemsOrphaned;

        /// <summary>
        /// Gets BucketStates.
        /// </summary>
        public ObservableCollection<StateCollection> BucketStates
        {
            get
            {
                return this.bucketStates;
            }
        }

        /// <summary>
        /// Gets Orphans.
        /// </summary>
        public ObservableCollection<IWorkbenchItem> Orphans
        {
            get
            {
                return this.orphans;
            }
        }

        /// <summary>
        /// Gets SwimLaneRows.
        /// </summary>
        public ObservableCollection<SwimLaneRow> SwimLaneRows
        {
            get
            {
                return this.swimLaneRows;
            }
        }

        /// <summary>
        /// Gets the row headers.
        /// </summary>
        /// <value>The row headers.</value>
        public ObservableCollection<string> RowHeaders
        {
            get { return this.rowHeaders; }
        }

        /// <summary>
        /// Gets Title.
        /// </summary>
        public string Title
        {
            get
            {
                return this.ViewMap == null ? string.Empty : this.ViewMap.Title;
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return this.ViewMap == null ? string.Empty : this.ViewMap.Description; }
        }

        /// <summary>
        /// Gets ViewMap.
        /// </summary>
        public ViewMap ViewMap { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [include in tabs].
        /// </summary>
        /// <value><c>true</c> if [include in tabs]; otherwise, <c>false</c>.</value>
        public bool IncludeInTabs { get; private set; }

        /// <summary>
        /// Applies the sort.
        /// </summary>
        public void ApplySort()
        {
            if (this.ViewMap.ParentSorter != null)
            {
                this.swimLaneRows.Sort(this.ViewMap.ParentSorter);
            }

            if (this.ViewMap.ChildSorter == null)
            {
                return;
            }

            foreach (var stateCollection in this.swimLaneRows.SelectMany(r => r.SwimLaneColumns).OfType<StateCollection>())
            {
                stateCollection.Sort(this.ViewMap.ChildSorter);
            }

            this.orphans.Sort(this.ViewMap.ChildSorter);
            foreach (var bucketState in this.BucketStates.OfType<StateCollection>())
            {
                bucketState.Sort(this.ViewMap.ChildSorter);
            }
        }

        /// <summary>
        /// Initialises this instance.
        /// </summary>
        /// <param name="map">The view map.</param>
        public void InitialiseLayout(ViewMap map)
        {
            if (map == null)
            {
                throw new ArgumentNullException("map");
            }

            this.ReleaseResources();

            this.ViewMap = map;

            this.RowHeaders.Add(string.Empty);

            foreach (var state in map.SwimLaneStates)
            {
                this.RowHeaders.Add(state);
            }

            foreach (var state in map.BucketStates)
            {
                this.BucketStates.Add(new StateCollection(state, map.LinkName));
            }

            if (this.LayoutChanged != null)
            {
                this.LayoutChanged(this, EventArgs.Empty);
            }

            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(null));
            }
        }

        /// <summary>
        /// Releases the resources.
        /// </summary>
        public void ReleaseResources()
        {
            var rows = this.SwimLaneRows.ToArray();
            var states = this.BucketStates.ToArray();

            this.SwimLaneRows.Clear();
            this.Orphans.Clear();
            this.RowHeaders.Clear();
            this.BucketStates.Clear();
            this.ViewMap = null;

            foreach (var row in rows)
            {
                row.ReleaseResources();
            }

            foreach (var bucketState in states)
            {
                bucketState.ReleaseResources();
            }
        }

        /// <summary>
        /// Called when [orphans changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The NotifyCollectionChangedEventArgs instance containing the event data.</param>
        private void OnOrphansChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.ItemsOrphaned == null || e.Action != NotifyCollectionChangedAction.Add)
            {
                return;
            }

            this.ItemsOrphaned(this, new ItemsOrphanedEventArgs(this, e.NewItems.OfType<IWorkbenchItem>()));
        }
    }
}